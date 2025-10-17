using HarmonyLib;
using static DaftAppleGames.BiggerAquariums_SN.BiggerAquariumsPlugin;

namespace DaftAppleGames.BiggerAquariums_SN
{
    /// <summary>
    /// Harmony patches for the aquarium storage container
    /// </summary>
    internal class StorageContainerPatches
    {
        [HarmonyPatch(typeof(StorageContainer))]
        internal class CuteFishPatch
        {
            [HarmonyPatch(nameof(StorageContainer.Awake))]
            [HarmonyPrefix]
            public static bool Start_Prefix(StorageContainer __instance)
            {
                ModDebugLog.LogDebug($"StorageContainer.Awake called on {__instance.gameObject.name}");
                
                BiggerAquarium BiggerAquarium = __instance.GetComponent<BiggerAquarium>();
                if (!BiggerAquarium)
                {
                    return true;
                }
               
                ModDebugLog.LogDebug("StorageContainer.Awake: in new 'BiggerAquarium'!");
                ModDebugLog.LogDebug($"Setting storage container height and width to {BiggerAquarium.StorageHeight}x{BiggerAquarium.StorageWidth}");
                __instance.height = BiggerAquarium.StorageHeight;
                __instance.width = BiggerAquarium.StorageWidth;
                ModDebugLog.LogDebug($"Done configuring storage container!");
                return true;
            }
        }
    }
}
