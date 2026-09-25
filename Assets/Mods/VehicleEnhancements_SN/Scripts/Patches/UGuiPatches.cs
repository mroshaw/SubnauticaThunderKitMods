using DaftAppleGames.VehicleEnhancements_SN.Hsi;
using DaftAppleGames.VehicleEnhancements_SN.Reversing;
using DaftAppleGames.VehicleEnhancements_SN.Speedometer;
using DaftAppleGames.VehicleEnhancements_SN.TimeAndWeather;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
    [HarmonyPatch(typeof(uGUI))]
    internal static class UGuiPatches
    {
        private const string EnhancedIndicatorsPrefabName = "EnhancedIndicators";

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(uGUI __instance)
        {
            if (!__instance)
            {
                return;
            }

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

            if (__instance.hud)
            {
                Transform cyclopsHudRoot = __instance.hud.transform;
                GameObject cyclopsIndicators = AddEnhancedIndicators(cyclopsHudRoot, EnhancedVehicle.Cyclops);
                if (cyclopsIndicators)
                {
                    cyclopsHudRoot.gameObject.AddComponent<CyclopsHudVisibility>().Configure(cyclopsIndicators);
                }
            }
        }

        private static GameObject AddEnhancedIndicators(Transform hudRoot, EnhancedVehicle vehicle)
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

            SpeedometerController speedometer = indicators.GetComponent<SpeedometerController>();
            HsiController hsi = indicators.GetComponent<HsiController>();
            TimeAndWeatherController time = indicators.GetComponent<TimeAndWeatherController>();
            ReversingAudioController audio = indicators.GetComponent<ReversingAudioController>();
            if (!speedometer || !hsi || !time || !audio)
            {
                ModDebugLog.LogError("Enhanced indicator prefab is missing a required controller.");
                Object.Destroy(indicators);
                return null;
            }

            speedometer.Configure(vehicle);
            hsi.Configure(vehicle);
            time.Configure(vehicle);
            audio.Configure(vehicle);

            RectTransform hudContent = hudRoot.parent as RectTransform;
            if (hudContent)
            {
                indicators.AddComponent<VehicleHudLayout>().Configure(hudContent);
            }

            if (vehicle != EnhancedVehicle.Cyclops)
            {
                indicators.SetActive(true);
            }

            return indicators;
        }
    }
}
