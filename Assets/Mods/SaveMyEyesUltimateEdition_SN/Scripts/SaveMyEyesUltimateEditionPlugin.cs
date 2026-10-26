using BepInEx;
using HarmonyLib;
using DaftAppleGames.ModTools;
using Nautilus.Handlers;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN
{
    [BepInDependency("com.snmodding.nautilus")]
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class SaveMyEyesUltimateEditionPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.savemyeyesultimateedesdition";
        private const string PluginName = "Save My Eyes: Ultimate Edition SN";
        internal const string VersionString = "0.0.4";
        
        private static readonly Harmony harmony = new Harmony(MyGuid);
        
        // Registration must precede component attachment during patch execution.
#if !UNITY_EDITOR
        internal static readonly ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        internal static ModLog ModDebugLog;
#else
        internal static readonly ModConfigFile ConfigFile;
        internal static ModLog ModDebugLog = new ModLog(null, true);
#endif

        
        private void Awake()
        {
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);
            
            harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
        }
    }
}
