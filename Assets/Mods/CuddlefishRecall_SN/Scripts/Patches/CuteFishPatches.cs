using HarmonyLib;
using static DaftAppleGames.CuddlefishRecall_SN.CuddlefishRecallPlugin;

namespace DaftAppleGames.CuddlefishRecall_SN.Patches
{
    /// <summary>
    /// Harmony patches for the Cuddlefish
    /// </summary>
    internal class CuteFishPatches
    {
        [HarmonyPatch(typeof(CuteFish))]
        internal class CuteFishPatch
        {
            [HarmonyPatch(nameof(CuteFish.Start))]
            [HarmonyPostfix]
            public static void Start_Postfix(CuteFish __instance)
            {
                // Add the PingInstance before the RecallAction so it can be cached in Awake
                PingInstance recallPing = __instance.gameObject.EnsureComponent<PingInstance>();

                // Add the new RecallAction
                CreatureRecallAction recallAction = __instance.gameObject.EnsureComponent<CreatureRecallAction>();
                ModDebugLog.LogDebug("Added CreatureRecallAction component.");
                
                // Add CreatureRecallListener - this will listen for recall requests and set the
                // Cuddlefish in motion
                __instance.gameObject.EnsureComponent<CreatureRecallListener>();
                ModDebugLog.LogDebug("Added CreatureRecallListener component.");

                // Add the Health Regen component, topping up health over time
                __instance.gameObject.EnsureComponent<HealthRegen>();
                ModDebugLog.LogDebug("Added HealthRegen component.");

                // Add a Ping instance so we can show the Cuddlefish location when in recall
                recallPing.pingType = GetRecallPingType();
                recallPing.origin = __instance.transform;
                recallPing.displayPingInManager = false;
                recallPing.minDist = recallAction.ArrivalTolerance;
                recallPing.range = 10f;
                recallPing.SetColor(0);
                recallPing.SetVisible(false);
                ModDebugLog.LogDebug("Added PingInstance component.");
                
                // Make sure the new RecallAction is registered
                __instance.ScanCreatureActions();
                ModDebugLog.LogDebug("ScanCreatureActions complete.");
            }

            /// <summary>
            /// Keeps Cuddlefish loaded after their following state changes
            /// </summary>
            [HarmonyPatch(nameof(CuteFish.followingPlayer), MethodType.Setter)]
            [HarmonyPostfix]
            public static void FollowingPlayerSetter_Postfix(CuteFish __instance)
            {
                __instance.largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;

                if (LargeWorldStreamer.main && LargeWorldStreamer.main.cellManager != null)
                {
                    LargeWorldStreamer.main.cellManager.RegisterEntity(__instance.largeWorldEntity);
                }
            }

            /// <summary>
            /// Removes any active recall marker when a Cuddlefish dies
            /// </summary>
            [HarmonyPatch(nameof(CuteFish.OnKill))]
            [HarmonyPostfix]
            public static void OnKill_Postfix(CuteFish __instance)
            {
                CreatureRecallAction recallAction = __instance.GetComponent<CreatureRecallAction>();
                if (recallAction)
                {
                    recallAction.CancelRecall();
                }
            }
        }
    }
}
