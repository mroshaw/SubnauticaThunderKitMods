using DaftAppleGames.ModTools.Extensions;
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
            SaveMyEyesFlareController flareController = __instance.EnsureComponent<SaveMyEyesFlareController>();
        }
    }
}
