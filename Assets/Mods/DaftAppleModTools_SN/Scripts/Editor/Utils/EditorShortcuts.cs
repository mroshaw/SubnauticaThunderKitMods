using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DaftAppleGames.Editor
{
    public static class EditorShortcuts
    {
        private static readonly string DnSpyPath = "D:\\Dev\\dnSpy-net-win64";
        private static readonly string TextPadPath = "C:\\Program Files\\TextPad";

        private static readonly string SnGamePath = "E:\\Games\\Steam\\steamapps\\common\\Subnautica";
        private static readonly string SnBepInExPath = Path.Combine(SnGamePath, "BepInEx");
        private static readonly string SnBepInExPluginPath = Path.Combine(SnBepInExPath, "plugins");

        private static readonly string LogBasePath =
            Path.Combine(
                $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}Low",
                "Unknown Worlds");
        private static readonly string SnLogPath = Path.Combine(LogBasePath, "Subnautica\\Player.log");
        private static readonly string SnGameAssemblyPath =
            Path.Combine(SnGamePath, "Subnautica_Data\\Managed\\Assembly-CSharp.dll");

        [MenuItem("Tools/Shortcuts/Run DnSpy (SN)")]
        private static void RunDnsSpySn()
        {
            LaunchProcess("dnSpy.exe", DnSpyPath, SnGameAssemblyPath);
        }

        [MenuItem("Tools/Shortcuts/Open Subnautica Folder")]
        private static void OpenSnFolder()
        {
            OpenExplorer(SnBepInExPluginPath);
        }

        [MenuItem("Tools/Shortcuts/Open SN Player Log")]
        private static void OpenSnLog()
        {
            OpenLog(SnLogPath);
        }

        private static void OpenLog(string logPath)
        {
            LaunchProcess("textpad.exe", TextPadPath, logPath, true);
        }

        private static void LaunchProcess(string processName, string processPath, string arguments,
            bool allowMultiple = false)
        {
            if (!allowMultiple)
            {
                // Check if it's already running
                Process[] running = Process.GetProcessesByName("dnSpy");
                if (running.Length > 0)
                {
                    Debug.Log($"Process {processName} is already running.");
                    return;
                }
            }

            string fullPath = Path.Combine(processPath, processName);
            ProcessStartInfo newProcess = new ProcessStartInfo
            {
                FileName = fullPath,
                Arguments = arguments
            };
            Process.Start(newProcess);
            Debug.Log($"Process {processName} started.");
        }

        private static void OpenExplorer(string folderPath)
        {
            Process.Start("explorer.exe", "/select," + folderPath);
        }
    }
}