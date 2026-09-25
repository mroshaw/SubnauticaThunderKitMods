using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    /// <summary>
    /// Patches for Water Filteration system.
    /// Configuration is in the SaveMyEyesFromFlares static class
    /// </summary>
    [HarmonyPatch(typeof(BaseFiltrationMachineGeometry))]
    internal static class BaseFiltrationMachineGeometryPatches
    {
        [HarmonyPatch(nameof(BaseFiltrationMachineGeometry.Awake))]
        [HarmonyPrefix]
        public static void AwakePrefix(BaseFiltrationMachineGeometry __instance)
        {
            SaveMyEyesFromWaterFiltrationBeams.Register(__instance);
        }

        [HarmonyPatch("UpdateVisuals")]
        [HarmonyPostfix]
        public static void UpdateVisualsPostfix(BaseFiltrationMachineGeometry __instance, bool ___cachedScanning)
        {
            SaveMyEyesFromWaterFiltrationBeams.ConfigureFromGameState(__instance, ___cachedScanning);
        }
        
        [HarmonyPatch(nameof(BaseFiltrationMachineGeometry.OnDestroy))]
        [HarmonyPrefix]
        public static void OnDestroyPrefix(BaseFiltrationMachineGeometry __instance)
        {
            SaveMyEyesFromWaterFiltrationBeams.Unregister(__instance);
        }


    }
}
