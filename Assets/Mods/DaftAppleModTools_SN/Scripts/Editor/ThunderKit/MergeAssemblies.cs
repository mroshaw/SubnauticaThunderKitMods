using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using ThunderKit.Core.Attributes;
using ThunderKit.Core.Paths;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace ThunderKit.Core.Pipelines.Jobs
{
    /// <summary>
    /// Embeds selected staged dependencies in a mod assembly using the ILRepack dotnet tool.
    /// </summary>
    [PipelineSupport(typeof(Pipeline)), ManifestProcessor]
    public class MergeAssemblies : PipelineJob
    {
        [SerializeField, Required, Tooltip("ILRepack executable name on PATH, or its full path.")]
        private string executablePath = "ilrepack";

        [SerializeField, Required, PathReferenceResolver]
        private string stagingPath = "<ManifestPluginStaging>";

        [SerializeField, Required, Tooltip("Mod DLL filename, including .dll. This determines the merged assembly identity.")]
        private string primaryAssembly = "SaveMyEyesUltimateEdition.dll";

        [SerializeField, Required, Tooltip("Dependency DLL filenames to embed and remove from staging after a successful merge.")]
        private string[] dependencies = { "DaftAppleModTools_SN.Core.dll" };

        /// <summary>
        /// Merges and validates the staged assemblies before replacing the mod DLL.
        /// </summary>
        public override async Task Execute(Pipeline pipeline)
        {
            string stageDirectory = Path.GetFullPath(PathReference.ResolvePath(stagingPath, pipeline, this));
            string primaryPath = GetAssemblyPath(stageDirectory, primaryAssembly);
            HashSet<string> inputPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { primaryPath };
            List<string> dependencyPaths = new List<string>();
            if (dependencies == null || dependencies.Length == 0)
            {
                throw new InvalidOperationException("MergeAssemblies requires at least one dependency.");
            }

            foreach (string dependency in dependencies)
            {
                string dependencyPath = GetAssemblyPath(stageDirectory, dependency);
                if (!inputPaths.Add(dependencyPath))
                {
                    throw new InvalidOperationException($"Duplicate merge input: {dependency}");
                }
                dependencyPaths.Add(dependencyPath);
            }

            // A separate output preserves staged inputs when ILRepack or validation fails.
            string mergeDirectory = Path.Combine(stageDirectory, "ILRepack-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(mergeDirectory);
            string outputPath = Path.Combine(mergeDirectory, primaryAssembly);
            try
            {
                string arguments = BuildArguments(primaryPath, dependencyPaths, outputPath, stageDirectory);
                pipeline.Log(LogLevel.Information, $"Merging dependencies into {primaryAssembly}", arguments);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    WorkingDirectory = stageDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());
                    string output = await outputTask;
                    string error = await errorTask;
                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException($"ILRepack exited with code {process.ExitCode}.\n{output}\n{error}");
                    }
                    pipeline.Log(LogLevel.Information, "ILRepack completed", output, error);
                }

                ValidateOutput(primaryPath, dependencyPaths, outputPath);
                File.Replace(outputPath, primaryPath, null);
                DeleteSymbols(primaryPath);
                foreach (string dependencyPath in dependencyPaths)
                {
                    File.Delete(dependencyPath);
                    DeleteSymbols(dependencyPath);
                }
                pipeline.Log(LogLevel.Information, $"Staged merged {primaryAssembly}; removed embedded dependency DLLs and stale symbols.");
            }
            finally
            {
                // Only files produced in this job's unique output directory are removed.
                foreach (string file in Directory.GetFiles(mergeDirectory))
                {
                    File.Delete(file);
                }
                Directory.Delete(mergeDirectory);
            }
        }

        private static string GetAssemblyPath(string directory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName ||
                !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Merge inputs must be DLL filenames without directories.");
            }
            string path = Path.Combine(directory, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Run MergeAssemblies after StageAssemblies. Staged assembly not found.", path);
            }
            return path;
        }

        private static string BuildArguments(string primaryPath, List<string> dependencyPaths, string outputPath,
            string stageDirectory)
        {
            StringBuilder arguments = new StringBuilder("/internalize /ndebug /skipconfig /out:");
            arguments.Append(Quote(outputPath));
            HashSet<string> searchDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { stageDirectory };
            HashSet<string> inputAssemblyNames = new HashSet<string>(StringComparer.Ordinal)
            {
                Path.GetFileNameWithoutExtension(primaryPath)
            };
            foreach (string dependencyPath in dependencyPaths)
            {
                inputAssemblyNames.Add(Path.GetFileNameWithoutExtension(dependencyPath));
            }
            foreach (UnityEditor.Compilation.Assembly assembly in CompilationPipeline.GetAssemblies())
            {
                if (!inputAssemblyNames.Contains(assembly.name))
                {
                    continue;
                }
                foreach (string reference in assembly.allReferences)
                {
                    searchDirectories.Add(Path.GetDirectoryName(Path.GetFullPath(reference)));
                }
            }
            foreach (string directory in searchDirectories)
            {
                arguments.Append(" /lib:").Append(Quote(directory));
            }
            arguments.Append(' ').Append(Quote(primaryPath));
            foreach (string dependencyPath in dependencyPaths)
            {
                arguments.Append(' ').Append(Quote(dependencyPath));
            }
            return arguments.ToString();
        }

        private static string Quote(string path)
        {
            // All arguments are file/directory paths, without embedded quotes or trailing separators.
            if (path.IndexOf('"') >= 0)
            {
                throw new ArgumentException("ILRepack paths cannot contain quotation marks.");
            }
            return "\"" + path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + "\"";
        }

        private static void ValidateOutput(string primaryPath, List<string> dependencyPaths, string outputPath)
        {
            AssemblyName originalName = AssemblyName.GetAssemblyName(primaryPath);
            AssemblyName mergedName = AssemblyName.GetAssemblyName(outputPath);
            if (originalName.FullName != mergedName.FullName)
            {
                throw new InvalidOperationException("ILRepack changed the primary assembly identity.");
            }

            // Inspect metadata without loading executable mod code into the Unity Editor domain.
            Assembly mergedAssembly = Assembly.ReflectionOnlyLoad(File.ReadAllBytes(outputPath));
            foreach (AssemblyName reference in mergedAssembly.GetReferencedAssemblies())
            {
                foreach (string dependencyPath in dependencyPaths)
                {
                    if (reference.Name == AssemblyName.GetAssemblyName(dependencyPath).Name)
                    {
                        throw new InvalidOperationException($"Merged assembly still references {reference.Name}.");
                    }
                }
            }
        }

        private static void DeleteSymbols(string assemblyPath)
        {
            File.Delete(Path.ChangeExtension(assemblyPath, ".pdb"));
            File.Delete(assemblyPath + ".mdb");
        }
    }
}
