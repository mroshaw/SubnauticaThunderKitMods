using BepInEx;
using HarmonyLib;
using DaftAppleGames.ModTools;
using Nautilus.Handlers;

namespace DaftAppleGames.AutoLockerLabels_SN
{
    [BepInDependency(
        LockerLabelModGuid,
        BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MyGuid, PluginName, VersionString)] public class AutoLockerLabelsPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.autolockerlabels";
        private const string PluginName = "AutoLockerLabels SN";
        private const string VersionString = "1.0.0";
        
        internal const string LockerLabelModGuid = "mod.0ctop3dus.lockerlabel";
        
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        
        // Config file / Log initialisation
#if !UNITY_EDITOR
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        internal static ModLog ModDebugLog;
#else
        internal static readonly ModConfigFile ConfigFile;
        internal static ModLog ModDebugLog = new ModLog(null, true);
#endif

        // Save data
        internal static AutoLockerLabelSaveData SaveData { get; private set; }
        
        private void Awake()
        {
            // Initialise Logger
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);
            
            // Initialise save data
            SaveData =
                SaveDataHandler.RegisterSaveDataCache<AutoLockerLabelSaveData>();
            
            // Patch in our MOD
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
        }
    }
}
