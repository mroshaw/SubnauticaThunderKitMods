using DaftAppleGames.VehicleEnhancements_SN.Enhancements;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
    [HarmonyPatch(typeof(uGUI))]
    internal static class UGuiPatches
    {
        internal const string EnhancedIndicatorsPrefabName = "EnhancedIndicators";

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(uGUI __instance)
        {
            uGUI_SeamothHUD seamothHud = __instance.GetComponentInChildren<uGUI_SeamothHUD>(true);
            if (seamothHud && seamothHud.root)
            {
                AddEnhancedIndicators(seamothHud.root.transform, EnhancedVehicle.Seamoth);
            }

            uGUI_ExosuitHUD exosuitHud = __instance.GetComponentInChildren<uGUI_ExosuitHUD>(true);
            if (exosuitHud && exosuitHud.root)
            {
                AddEnhancedIndicators(exosuitHud.root.transform, EnhancedVehicle.PrawnSuit);
            }
        }

        internal static bool ConfigureEnhancedIndicators(GameObject indicators, Transform hudRoot, EnhancedVehicle vehicle)
        {
            SpeedometerController speedometer = indicators.GetComponent<SpeedometerController>();
            HsiController hsi = indicators.GetComponent<HsiController>();
            TimeAndWeatherController time = indicators.GetComponent<TimeAndWeatherController>();
            ReversingAudioController audio = indicators.GetComponent<ReversingAudioController>();
            if (!speedometer || !hsi || !time || !audio)
            {
                ModDebugLog.LogError("Enhanced indicator prefab is missing a required controller.");
                return false;
            }

            speedometer.Configure(vehicle);
            hsi.Configure(vehicle);
            time.Configure(vehicle);
            audio.Configure(vehicle);

            RectTransform hudContent = hudRoot.parent as RectTransform;
            if (hudContent)
            {
                VehicleHudLayout layout = indicators.GetComponent<VehicleHudLayout>();
                if (!layout)
                {
                    layout = indicators.AddComponent<VehicleHudLayout>();
                }

                layout.Configure(hudContent);
            }

            return true;
        }

        internal static GameObject AddEnhancedIndicators(Transform hudRoot, EnhancedVehicle vehicle)
        {
            if (hudRoot.Find(EnhancedIndicatorsPrefabName))
            {
                return null;
            }

            GameObject indicators = ModAssetUtils.GetPrefabInstanceFromAssetBundle(EnhancedIndicatorsPrefabName, false);
            if (!indicators)
            {
                ModDebugLog.LogError($"Could not instantiate '{EnhancedIndicatorsPrefabName}'.");
                return null;
            }

            indicators.SetActive(false);
            indicators.name = EnhancedIndicatorsPrefabName;
            indicators.transform.SetParent(hudRoot, false);

            if (!ConfigureEnhancedIndicators(indicators, hudRoot, vehicle))
            {
                Object.Destroy(indicators);
                return null;
            }

            if (vehicle != EnhancedVehicle.Cyclops)
            {
                indicators.SetActive(true);
            }

            return indicators;
        }
    }
}
