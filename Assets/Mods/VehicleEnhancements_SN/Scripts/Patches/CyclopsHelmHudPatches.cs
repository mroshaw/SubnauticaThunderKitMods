using DaftAppleGames.VehicleEnhancements_SN.Enhancements;
using HarmonyLib;
using UnityEngine;
using Nautilus.Utility;
using TMPro;
using UnityEngine.UI;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
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

            if (!leftCanvas || !upperCenterCanvas || !rightCanvas)
            {
                VehicleEnhancementsPlugin_SN.ModDebugLog.LogError(
                    "Could not find the Cyclops helm HUD canvases.");
                Object.Destroy(indicators);
                return;
            }

            indicators.SetActive(true);

            ApplyHolographicUi(indicators);
            
            ConfigurePanel(
                speedometer,
                leftCanvas, upperCenterCanvas, rightCanvas,
                speedometerController.CyclopsLocalRotation,
                VehicleHudElementLayout.Element.Speedometer);
            ConfigurePanel(
                hsi,
                leftCanvas, upperCenterCanvas, rightCanvas,
                hsiController.CyclopsLocalRotation,
                VehicleHudElementLayout.Element.Hsi);
            ConfigurePanel(
                timeAndWeather,
                leftCanvas, upperCenterCanvas, rightCanvas,
                timeController.CyclopsLocalRotation,
                VehicleHudElementLayout.Element.TimeAndWeather);
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

        private static void ConfigurePanel(
            RectTransform panel,
            RectTransform leftCanvas,
            RectTransform centerCanvas,
            RectTransform rightCanvas,
            Vector3 localRotation,
            VehicleHudElementLayout.Element element)
        {
            panel.SetParent(leftCanvas, false);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.localRotation = Quaternion.Euler(localRotation);
            panel.gameObject.AddComponent<VehicleHudElementLayout>().Configure(EnhancedVehicle.Cyclops, element,
                leftCanvas, centerCanvas, rightCanvas);
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
