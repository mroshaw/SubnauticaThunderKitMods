using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch(typeof(LightAnimator))]
    internal static class LightAnimatorPatches
    {
        [HarmonyPatch(nameof(LightAnimator.Awake))]
        [HarmonyPrefix]
        private static void AwakePrefix(LightAnimator __instance, out SaveMyEyesLightController __state)
        {
            __state = __instance.GetComponentInParent<SaveMyEyesLightController>();
            Light light = __instance.GetComponent<Light>();
            if (__state && light)
            {
                __state.PrepareCurrentIntensity(light);
            }
        }

        [HarmonyPatch(nameof(LightAnimator.Awake))]
        [HarmonyPostfix]
        private static void AwakePostfix(LightAnimator __instance, SaveMyEyesLightController __state)
        {
            if (__state && __instance.lightComponent)
            {
                __state.CaptureAndApplyCurrentIntensity(__instance.lightComponent);
            }
        }

        [HarmonyPatch(nameof(LightAnimator.Update))]
        [HarmonyPrefix]
        private static void UpdatePrefix(LightAnimator __instance, out SaveMyEyesLightController __state)
        {
            __state = __instance.GetComponentInParent<SaveMyEyesLightController>();
            if (__state && __instance.lightComponent)
            {
                __state.PrepareCurrentIntensity(__instance.lightComponent);
            }
        }

        [HarmonyPatch(nameof(LightAnimator.Update))]
        [HarmonyPostfix]
        private static void UpdatePostfix(LightAnimator __instance, SaveMyEyesLightController __state)
        {
            if (__state && __instance.lightComponent)
            {
                __state.CaptureAndApplyCurrentIntensity(__instance.lightComponent);
            }
        }
    }
}
