using DaftAppleGames.VehicleEnhancements_SN.Hsi;
using DaftAppleGames.VehicleEnhancements_SN.Speedometer;
using DaftAppleGames.VehicleEnhancements_SN.TimeAndWeather;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    internal static class CyclopsHelmHudPatches
    {
        private const string RightHudCanvasName = "Canvas_RightHUD";
        private const string LeftHudCanvasName = "Canvas_LeftHUD";
        private const string SpeedometerName = "Speedometer";
        private const string HsiName = "Horizontal Situation Indicator";
        private const string TimeAndWeatherName = "Time And Weather";

        [HarmonyPatch(nameof(CyclopsHelmHUDManager.StartPiloting))]
        [HarmonyPostfix]
        private static void StartPilotingPostfix(CyclopsHelmHUDManager __instance)
        {
            if (!__instance || !__instance.subRoot)
            {
                return;
            }

            RectTransform rightCanvas = null;
            RectTransform leftCanvas = null;
            Canvas[] canvases = __instance.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name == RightHudCanvasName)
                {
                    rightCanvas = canvas.transform as RectTransform;
                }
                else if (canvas.name == LeftHudCanvasName)
                {
                    leftCanvas = canvas.transform as RectTransform;
                }
            }

            if (!rightCanvas || !leftCanvas)
            {
                VehicleEnhancementsPlugin_SN.ModDebugLog.LogError(
                    $"Could not find the Cyclops helm canvases '{RightHudCanvasName}' and '{LeftHudCanvasName}'.");
                return;
            }

            GameObject indicators = UGuiPatches.AddEnhancedIndicators(
                __instance.transform,
                EnhancedVehicle.Cyclops);
            if (!indicators)
            {
                return;
            }

            RectTransform speedometer = indicators.transform.Find(SpeedometerName) as RectTransform;
            RectTransform hsi = indicators.transform.Find(HsiName) as RectTransform;
            RectTransform timeAndWeather = indicators.transform.Find(TimeAndWeatherName) as RectTransform;
            SpeedometerController speedometerController = indicators.GetComponent<SpeedometerController>();
            HsiController hsiController = indicators.GetComponent<HsiController>();
            TimeAndWeatherController timeController = indicators.GetComponent<TimeAndWeatherController>();
            if (!speedometer || !hsi || !timeAndWeather || !speedometerController ||
                !hsiController || !timeController)
            {
                VehicleEnhancementsPlugin_SN.ModDebugLog.LogError(
                    "Enhanced indicator prefab is missing a Cyclops HUD panel.");
                Object.Destroy(indicators);
                return;
            }

            indicators.SetActive(true);

            ConfigurePanel(
                speedometer,
                rightCanvas,
                speedometerController.CyclopsLocalPosition,
                speedometerController.CyclopsLocalScale);
            ConfigurePanel(
                hsi,
                rightCanvas,
                hsiController.CyclopsLocalPosition,
                hsiController.CyclopsLocalScale);
            ConfigurePanel(
                timeAndWeather,
                leftCanvas,
                timeController.CyclopsLocalPosition,
                timeController.CyclopsLocalScale);

            __instance.gameObject.AddComponent<CyclopsHudVisibility>().Configure(
                indicators,
                __instance.subRoot);
        }

        private static void ConfigurePanel(
            RectTransform panel,
            RectTransform canvas,
            Vector3 localPosition,
            Vector3 localScale)
        {
            panel.SetParent(canvas, false);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.localPosition = localPosition;
            panel.localRotation = Quaternion.identity;
            panel.localScale = localScale;
        }
    }
}
