using HarmonyLib;
using static DaftAppleGames.CuddlefishRecall_SN.CuddlefishRecallPlugin;

namespace DaftAppleGames.CuddlefishRecall_SN.Patches
{
    /// <summary>
    /// Patch CuteFishHandTarget - used to intercept feeding animation
    /// to then replenish Cuddlefish health
    /// </summary>
    internal class CuteFishHandTargetPatches
    {
        private const string SnackAnimationPrefabName = "CutefishSnack";
        private const string HealthReplenished = "Cuddlefish health replenished!";

        [HarmonyPatch(typeof(CuteFishHandTarget))]
        internal class CuteFishHandTargetPatch
        {
            [HarmonyPatch("PrepareCinematicMode")]
            [HarmonyPostfix]
            public static void PrepareCinematicMode_Postfix(CuteFishHandTarget __instance, Player setPlayer, global::CuteFishHandTarget.CuteFishCinematic cinematic)
            {
                ModDebugLog.LogDebug($"Cuddlefish clicked. Playing: {cinematic.itemPrefab.name}");
                if (cinematic.itemPrefab.name == SnackAnimationPrefabName)
                {
                    ModDebugLog.LogDebug($"Replenish Cuddlefish health...");
                    __instance.cuteFish.GetComponent<LiveMixin>().ResetHealth();
                    ErrorMessage.AddMessage(HealthReplenished);
                    ModDebugLog.LogDebug($"Cuddlefish health replenished.");
                }
            }
        }
    }
}

