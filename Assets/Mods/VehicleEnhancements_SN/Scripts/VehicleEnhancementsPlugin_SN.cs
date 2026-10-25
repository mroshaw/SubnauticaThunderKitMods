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
        private const string VersionString = "1.1.0";
        private const string AssetBundleName = "enhancedvehiclesassetbundle";

        internal static ModAssetBundleUtils ModAssetUtils;
        private static FMODAsset _thisVehicleIsReversingFmodAsset;
        private static FMODAsset _thisPrawnSuitIsReversingFmodAsset;
        private static FMODAsset _thisSeaMothIsReversingFmodAsset;
        private static FMODAsset _thisCyclopsIsReversingFmodAsset;
        internal static FMODAsset ReversingBeepsFmodAsset { get; private set; }

#if !UNITY_EDITOR
        internal static readonly ModConfigFile ConfigFile = new ModConfigFile();
        internal static ModLog ModDebugLog;
#else
        internal static readonly ModConfigFile ConfigFile = new ModConfigFile();
        internal static ModLog ModDebugLog = new ModLog(null, true);
#endif

        private static readonly Harmony Harmony = new Harmony(MyGuid);

        private void Awake()
        {
#if !UNITY_EDITOR
            ConfigFile.Load();
#endif
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);
#if !UNITY_EDITOR
            OptionsPanelHandler.RegisterModOptions(new VehicleEnhancementsOptions(ConfigFile));
#endif
            ModAssetUtils = new ModAssetBundleUtils(AssetBundleName, Assembly.GetExecutingAssembly(), true, ModDebugLog);
            ReversingBeepsFmodAsset = RegisterCustomSound("ReversingBeeps.wav");
            _thisVehicleIsReversingFmodAsset = RegisterCustomSound("ThisVehicleIsReversing.wav");
            _thisPrawnSuitIsReversingFmodAsset = RegisterCustomSound("ThisPrawnSuitIsReversing.wav");
            _thisSeaMothIsReversingFmodAsset = RegisterCustomSound("ThisSeaMothIsReversing.wav");
            _thisCyclopsIsReversingFmodAsset = RegisterCustomSound("ThisCyclopsIsReversing.wav");
            Harmony.PatchAll();
            ModDebugLog.LogInfo($"{PluginName} {VersionString} loaded.");
        }

        internal static FMODAsset GetReversingVoiceAsset(ReversingVoice voice)
        {
            switch (voice)
            {
                case ReversingVoice.ThisVehicleIsReversing: return _thisVehicleIsReversingFmodAsset;
                case ReversingVoice.ThisPrawnSuitIsReversing: return _thisPrawnSuitIsReversingFmodAsset;
                case ReversingVoice.ThisSeaMothIsReversing: return _thisSeaMothIsReversingFmodAsset;
                case ReversingVoice.ThisCyclopsIsReversing: return _thisCyclopsIsReversingFmodAsset;
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
