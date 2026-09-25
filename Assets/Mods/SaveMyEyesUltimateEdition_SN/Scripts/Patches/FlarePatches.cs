using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    /// <summary>
    /// Patches for flares. Management of all registered flares and configuration of properties
    /// is in the SaveMyEyesFromFlares static class
    /// </summary>
    [HarmonyPatch(typeof(Flare))]
    internal static class FlarePatches
    {
        [HarmonyPatch(nameof(Flare.Awake))]
        [HarmonyPrefix]
        public static void AwakePrefix(Flare __instance)
        {
            SaveMyEyesFromFlares.Register(__instance);
        }

        [HarmonyPatch("UpdateLight")]
        [HarmonyPrefix]
        public static void UpdateLightPrefix(Flare __instance)
        {
            SaveMyEyesFromFlares.PrepareCurrentFlareIntensity(__instance);
        }

        [HarmonyPatch("UpdateLight")]
        [HarmonyPostfix]
        public static void UpdateLightPostfix(Flare __instance)
        {
            SaveMyEyesFromFlares.ConfigureCurrentFlareIntensity(__instance);
        }
        
        [HarmonyPatch(nameof(Flare.OnDestroy))]
        [HarmonyPrefix]
        public static void OnDestroyPrefix(Flare __instance)
        {
            SaveMyEyesFromFlares.Unregister(__instance);
        }

    }
}
