# MergeAssemblies pipeline job

Add **MergeAssemblies** immediately after **StageAssemblies** (or **StageAssembliesExt**) in a ThunderKit build pipeline, before deployment or ZIP packaging. This is an opt-in job: the existing shared build pipelines have not been changed.

For Save My Eyes, configure:

| Field | Value |
| --- | --- |
| Executable Path | `ilrepack`, or `C:/Users/mrosh/.dotnet/tools/ilrepack.exe` |
| Staging Path | `<ManifestPluginStaging>` |
| Primary Assembly | `SaveMyEyesUltimateEdition.dll` |
| Dependencies | `DaftAppleModTools_SN.Core.dll` |

The primary filename is the assembly name from the mod's `.asmdef`, which can differ from the manifest or mod folder name. For other mods, change this field to their compiled DLL filename. Each dependency must be a DLL filename in the same staging directory.

Install the external tool with `dotnet tool install -g dotnet-ilrepack`. If Unity was already running when the tool was installed, use the full executable path or restart Unity so that it sees the updated PATH.

Keep both assembly definitions in the manifest's AssemblyDefinitions list and retain the existing mod-to-ModTools `.asmdef` reference. ThunderKit needs to compile and stage both inputs before merging. No generated `.csproj` edits are required.

The job runs ILRepack with `/internalize /ndebug /skipconfig`. It obtains dependency search directories from Unity's compilation references for both input assemblies. Unity, BepInEx, Harmony, Nautilus and other external dependencies are resolved but are not merged unless explicitly listed as inputs.

Output is written to a unique temporary directory within staging. Before replacement, the job checks that the output is a valid assembly, preserves the primary assembly identity, and has no remaining references to the embedded dependency assemblies. A failed merge or validation throws and stops the pipeline with the staged inputs intact. A successful merge replaces the staged mod DLL, removes the embedded dependency DLLs and stale PDB/MDB files, then removes the temporary directory. Build-cache originals are unchanged. Debug symbols are deliberately omitted.

Only opt in for helper types that should be private to the mod. Static state is copied independently into each merged mod. Unity asset bundles that refer to ModTools MonoBehaviours or ScriptableObjects by their original assembly name need separate handling; this job does not rewrite serialized asset references. Likewise, types exchanged with other mods must retain a shared assembly identity.

The job processes the current manifest context. If a pipeline builds several manifests, run it once in each relevant manifest context and provide that mod's primary assembly filename.

Packaging contains one mod DLL after merging. Existing deployment jobs may copy over an old installation without removing its old ModTools DLL; inspect that installation separately. This job only removes dependencies from staging.
