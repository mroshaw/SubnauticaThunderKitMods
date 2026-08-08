using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using DaftAppleGames.ModTools;
using UnityEngine;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    // Mod supports "Teleporting" a creature, and forcing a "Swim To" behaviour
    public enum RecallMoveMethod
    {
        Teleport,
        SwimTo
    };

    [BepInPlugin(MyGuid, PluginName, VersionString)] public class CuddlefishRecallPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.cuddlefishrecallmodsn";
        private const string PluginName = "Cuddlefish Recall Mod SN";
        private const string VersionString = "1.5.1";

        private const string AssetBundleName = "cuddlefishrecallassetbundle";
        private const string PingIndicatorTextureName = "CuddlefishPingIcon.png";

        // Config file / UI initialisation
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        internal static ModAssetBundleUtils ModAssetUtils;

        // Mod Debug Log
        internal static ModLog ModDebugLog;

        private static bool recallPingTypeRegistered;
        private static PingType recallPingType;

        // New input system
        public static GameInput.Button _recallButton = EnumHandler.AddEntry<GameInput.Button>("RecallAllCuddlefish")
            .CreateInput("Recall All Cuddlefish")
            .WithKeyboardBinding(GameInputHandler.Paths.Keyboard.R)
            .WithCategory("Cuddlefish Recall");

        internal static PingType GetRecallPingType()
        {
            if (!recallPingTypeRegistered)
            {
                Sprite pingIndicatorSprite =
                        (Sprite)ModAssetUtils.GetObjectFromAssetBundle<Sprite>(PingIndicatorTextureName);

                recallPingType = EnumHandler.AddEntry<PingType>("CuddlefishRecall")
                    .WithIcon(pingIndicatorSprite);
                recallPingTypeRegistered = true;
            }

            return recallPingType;
        }

        private void Awake()
        {
            // Initialise Logger
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);

            // Initialise AssetBundle
            ModAssetUtils =
                new ModAssetBundleUtils(AssetBundleName, Assembly.GetExecutingAssembly(), true, ModDebugLog);

            // Patch in our MOD
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
        }
    }
}