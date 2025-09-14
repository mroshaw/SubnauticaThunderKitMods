using BepInEx;
using BepInEx.Logging;
using DaftAppleGames.SubnauticaCheater.Config;
using HarmonyLib;
using Nautilus.Handlers;

namespace DaftAppleGames.SubnauticaCheater
{
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class SubnauticaCheaterPlugin : BaseUnityPlugin
    {
        // Mod  details
        private const string MyGuid = "com.mrosh.SubnauticaCheater";
        private const string PluginName = "SubnauticaCheater";
        private const string VersionString = "1.1.0";

        // Config file / UI initialisation
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            // Apply all of our patches
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

            // Sets up our static Log, so it can be used elsewhere in code.
            Log = Logger;
        }
    }
}