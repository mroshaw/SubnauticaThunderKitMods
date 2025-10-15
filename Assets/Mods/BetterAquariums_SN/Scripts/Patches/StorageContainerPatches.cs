using HarmonyLib;
using static DaftAppleGames.BetterAquariums_SN.BetterAquariumsPlugin;

namespace DaftAppleGames.BetterAquariums_SN
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
                
                BetterAquarium betterAquarium = __instance.GetComponent<BetterAquarium>();
                if (!betterAquarium)
                {
                    return true;
                }
               
                ModDebugLog.LogDebug("StorageContainer.Awake: in new 'BetterAquarium'!");
                ModDebugLog.LogDebug($"Setting storage container height and width to {betterAquarium.StorageHeight}x{betterAquarium.StorageWidth}");
                __instance.height = betterAquarium.StorageHeight;
                __instance.width = betterAquarium.StorageWidth;
                ModDebugLog.LogDebug($"Done configuring storage container!");
                return true;
            }
        }
    }
}
