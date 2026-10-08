using DaftAppleGames.ModTools.Extensions;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    /// <summary>
    /// Attaches the flare controller and applies its settings around game light updates.
    /// </summary>
    [HarmonyPatch(typeof(Flare))]
    internal static class FlarePatches
    {
        /// <summary>
        /// Attaches the controller before the flare is initialized.
        /// </summary>
        [HarmonyPatch(nameof(Flare.Awake))]
        [HarmonyPrefix]
        public static void AwakePrefix(Flare __instance)
        {
            __instance.EnsureComponent<SaveMyEyesFlareController>();
        }

        /// <summary>
        /// Restores the unscaled intensity before the game updates the flare light.
        /// </summary>
        [HarmonyPatch(nameof(Flare.UpdateLight))]
        [HarmonyPrefix]
        public static void UpdateLightPrefix(Flare __instance, out SaveMyEyesFlareController __state)
        {
            __state = __instance.GetComponent<SaveMyEyesFlareController>();
            __state.PrepareCurrentIntensity();
        }

        /// <summary>
        /// Captures the game intensity and applies the configured flare multiplier.
        /// </summary>
        [HarmonyPatch(nameof(Flare.UpdateLight))]
        [HarmonyPostfix]
        public static void UpdateLightPostfix(SaveMyEyesFlareController __state)
        {
            __state.CaptureAndApplyCurrentIntensity();
        }
    }
}
