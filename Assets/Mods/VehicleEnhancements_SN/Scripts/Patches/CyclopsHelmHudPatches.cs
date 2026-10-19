using DaftAppleGames.VehicleEnhancements_SN.Enhancements;
using HarmonyLib;
using UnityEngine;
using Nautilus.Utility;
using TMPro;
using UnityEngine.UI;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
    internal enum CyclopsHudLocation
    {
        Left,
        UpperCenter,
        Right
    }

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))] internal static class CyclopsHelmHudPatches
    {
        private const string RightHudCanvasName = "Canvas_RightHUD";
        private const string LeftHudCanvasName = "Canvas_LeftHUD";
        private const string UpperCenterHudCanvasName = "Canvas_CenterHUD";
        private const string SpeedometerName = "Speedometer";
        private const string HsiName = "Horizontal Situation Indicator";
        private const string TimeAndWeatherName = "Time And Weather";

        [HarmonyPatch(nameof(CyclopsHelmHUDManager.StartPiloting))]
        [HarmonyPostfix]
        private static void StartPilotingPostfix(CyclopsHelmHUDManager __instance)
        {
            if (!__instance.subRoot)
            {
                return;
            }

            Transform existingIndicators = __instance.transform.Find(UGuiPatches.EnhancedIndicatorsPrefabName);
            if (existingIndicators)
            {
                existingIndicators.gameObject.SetActive(true);
                return;
            }

            RectTransform rightCanvas = null;
            RectTransform leftCanvas = null;
            RectTransform upperCenterCanvas = null;
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
                else if (canvas.name == UpperCenterHudCanvasName)
                {
                    upperCenterCanvas = canvas.transform as RectTransform;
                }
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

            RectTransform speedometerCanvas = GetHudCanvas(
                speedometerController.CyclopsHudLocation, leftCanvas, upperCenterCanvas, rightCanvas);
            RectTransform hsiCanvas = GetHudCanvas(
                hsiController.CyclopsHudLocation, leftCanvas, upperCenterCanvas, rightCanvas);
            RectTransform timeCanvas = GetHudCanvas(
                timeController.CyclopsHudLocation, leftCanvas, upperCenterCanvas, rightCanvas);
            if (!speedometerCanvas || !hsiCanvas || !timeCanvas)
            {
                VehicleEnhancementsPlugin_SN.ModDebugLog.LogError(
                    "Could not find a selected Cyclops helm HUD canvas.");
                Object.Destroy(indicators);
                return;
            }

            indicators.SetActive(true);

            ApplyHolographicUi(indicators);
            
            ConfigurePanel(
                speedometer,
                speedometerCanvas,
                speedometerController.CyclopsLocalPosition,
                speedometerController.CyclopsLocalRotation,
                speedometerController.CyclopsLocalScale);
            ConfigurePanel(
                hsi,
                hsiCanvas,
                hsiController.CyclopsLocalPosition,
                hsiController.CyclopsLocalRotation,
                hsiController.CyclopsLocalScale);
            ConfigurePanel(
                timeAndWeather,
                timeCanvas,
                timeController.CyclopsLocalPosition,
                timeController.CyclopsLocalRotation,
                timeController.CyclopsLocalScale);
        }

        [HarmonyPatch(nameof(CyclopsHelmHUDManager.StopPiloting))]
        [HarmonyPostfix]
        private static void StopPilotingPostfix(CyclopsHelmHUDManager __instance)
        {
            Transform indicators = __instance.transform.Find(UGuiPatches.EnhancedIndicatorsPrefabName);
            if (indicators)
            {
                indicators.gameObject.SetActive(false);
            }
        }

        private static RectTransform GetHudCanvas(
            CyclopsHudLocation location,
            RectTransform leftCanvas,
            RectTransform upperCenterCanvas,
            RectTransform rightCanvas)
        {
            switch (location)
            {
                case CyclopsHudLocation.Left:
                    return leftCanvas;
                case CyclopsHudLocation.UpperCenter:
                    return upperCenterCanvas;
                case CyclopsHudLocation.Right:
                    return rightCanvas;
                default:
                    return null;
            }
        }

        private static void ConfigurePanel(
            RectTransform panel,
            RectTransform canvas,
            Vector3 localPosition,
            Vector3 localRotation,
            Vector3 localScale)
        {
            panel.SetParent(canvas, false);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.localPosition = localPosition;
            panel.localRotation = Quaternion.Euler(localRotation);
            panel.localScale = localScale;
        }
        
        /// <summary>
        /// Iterate over UI elements and apply the Holographic UI shader. Only relevant for Cyclops
        /// </summary>
        private static void ApplyHolographicUi(GameObject indicators)
        {
            Material holographicMaterial = MaterialUtils.HolographicUIMaterial;
            if (holographicMaterial)
            {
                Graphic[] graphics = indicators.GetComponentsInChildren<Graphic>(true);
                foreach (Graphic graphic in graphics)
                {
                    if (graphic is TMP_Text)
                    {
                        continue;
                    }

                    graphic.material = holographicMaterial;
                }
            }
        }
        
    }
}
