using System.Reflection;
using BepInEx;
using DaftAppleGames.ModTools;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class VehicleEnhancementsPlugin_SN : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.vehicleenhancements.sn";
        private const string PluginName = "Vehicle Enhancements SN";
        private const string VersionString = "0.1.0";
        private const string AssetBundleName = "enhancedvehiclesassetbundle";

        internal static ModAssetBundleUtils ModAssetUtils;
        internal static FMODAsset ReversingBeepsFmodAsset;
        internal static FMODAsset ThisVehicleIsReversingFmodAsset;
        internal static FMODAsset ThisPrawnSuitIsReversingFmodAsset;
        internal static FMODAsset ThisSeaMothIsReversingFmodAsset;
        internal static FMODAsset ThisCyclopsIsReversingFmodAsset;

#if !UNITY_EDITOR
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        internal static ModLog ModDebugLog;
#else
        internal static readonly ModConfigFile ConfigFile = new ModConfigFile();
        internal static ModLog ModDebugLog = new ModLog(null, true);
#endif

        private static readonly Harmony Harmony = new Harmony(MyGuid);

        private void Awake()
        {
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);
            ModAssetUtils = new ModAssetBundleUtils(AssetBundleName, Assembly.GetExecutingAssembly(), true, ModDebugLog);
            ReversingBeepsFmodAsset = RegisterCustomSound("ReversingBeeps.wav");
            ThisVehicleIsReversingFmodAsset = RegisterCustomSound("ThisVehicleIsReversing.wav");
            ThisPrawnSuitIsReversingFmodAsset = RegisterCustomSound("ThisPrawnSuitIsReversing.wav");
            ThisSeaMothIsReversingFmodAsset = RegisterCustomSound("ThisSeaMothIsReversing.wav");
            ThisCyclopsIsReversingFmodAsset = RegisterCustomSound("ThisCyclopsIsReversing.wav");
            Harmony.PatchAll();
            ModDebugLog.LogInfo($"{PluginName} {VersionString} loaded.");
        }

        internal static FMODAsset GetReversingVoiceAsset(ReversingVoice voice)
        {
            switch (voice)
            {
                case ReversingVoice.ThisVehicleIsReversing: return ThisVehicleIsReversingFmodAsset;
                case ReversingVoice.ThisPrawnSuitIsReversing: return ThisPrawnSuitIsReversingFmodAsset;
                case ReversingVoice.ThisSeaMothIsReversing: return ThisSeaMothIsReversingFmodAsset;
                case ReversingVoice.ThisCyclopsIsReversing: return ThisCyclopsIsReversingFmodAsset;
                default: return null;
            }
        }

        private static FMODAsset RegisterCustomSound(string clipName)
        {
            AudioClip clip = ModAssetUtils.GetObjectFromAssetBundle<AudioClip>(clipName, false) as AudioClip;
            if (!clip)
            {
                ModDebugLog.LogError($"Could not load reversing audio clip '{clipName}'.");
                return null;
            }

            ModAudioUtils.RegisterSound(clipName, AudioUtils.BusPaths.PlayerSFXs, ModAssetUtils, ModDebugLog,
                minDistance: 2.0f, maxDistance: 30.0f, loop: true);
            return AudioUtils.GetFmodAsset(clipName);
        }
    }
}
