using HarmonyLib;

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
                // Add CreatureRecallListener - this will listen for recall requests and set the
                // Cuddlefish in motion
                __instance.gameObject.AddComponent<CreatureRecallListener>();
                CuddlefishRecallPlugin.Log.LogDebug("Added CreatureRecallListener component.");

                // Add the Health Regen component, topping up health over time
                __instance.gameObject.AddComponent<HealthRegen>();
                CuddlefishRecallPlugin.Log.LogDebug("Added EnhancedCuddlefish component.");
            }
        }
    }
}
