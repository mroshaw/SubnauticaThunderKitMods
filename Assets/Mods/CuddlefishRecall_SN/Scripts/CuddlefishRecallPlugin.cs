using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    // Mod supports "Teleporting" a creature, and forcing a "Swim To" behaviour
    public enum RecallMoveMethod
    {
        Teleport,
        SwimTo
    };

    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class CuddlefishRecallPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.cuddlefishrecallmodsn";
        private const string PluginName = "Cuddlefish Recall Mod SN";
        private const string VersionString = "1.3.0";

        // Config file / UI initialisation
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        private void Awake()
        {
            // Patch in our MOD
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }
    }
}