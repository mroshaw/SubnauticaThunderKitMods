using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    /// <summary>
    /// Patches for the Water Filtration system
    /// </summary>
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    internal static class BaseFiltrationMachineGeometryPatches
    {
        [HarmonyPatch(nameof(BaseFiltrationMachineGeometry.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(BaseFiltrationMachineGeometry __instance)
        {
            __instance.gameObject.EnsureComponent<SaveMyEyesWaterFiltrationController>();
        }

        [HarmonyPatch("UpdateVisuals")]
        [HarmonyPostfix]
        public static void UpdateVisualsPostfix(BaseFiltrationMachineGeometry __instance)
        {
            SaveMyEyesWaterFiltrationController controller =
                __instance.GetComponent<SaveMyEyesWaterFiltrationController>();
            controller.SetBeamsActive(__instance.cachedScanning);
        }
    }
}
