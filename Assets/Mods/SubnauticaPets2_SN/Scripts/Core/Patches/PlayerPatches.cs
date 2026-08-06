using HarmonyLib;
using System.Collections;
using DaftAppleGames.SubnauticaPets.BaseParts;
using DaftAppleGames.SubnauticaPets.Utils;
using UnityEngine;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        private static float secondsToWaitBeforeCheck = 3.0f;

        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(Player __instance)
        {
            __instance.StartCoroutine(CheckUnlockStateSync(__instance));
            
            // FOR DEV ONLY! REMOVE BEFORE BUILD!!
            /*
            Debug.Log("In Player Start");
            ModDebugLog.LogDebug("Added FragmentSpawner to player!");
            __instance.gameObject.AddComponent<FragmentSpawner>();
            */
        }

        private static IEnumerator CheckUnlockStateSync(Player player)
        {
            ModDebugLog.LogDebug("In CheckUnlockStateSync");
            yield return new WaitForSeconds(secondsToWaitBeforeCheck);
            ModDebugLog.LogDebug($"Player.Awake (After delay): KnownText UnlockState for 'PetFabricatorPrefab' is: {KnownTech.GetTechUnlockState(PetFabricatorPrefab.Info.TechType)}");
            ModDebugLog.LogDebug($"Player.Awake (After delay): KnownText UnlockState for 'PetConsolePrefab' is: {KnownTech.GetTechUnlockState(PetConsolePrefab.Info.TechType)}");
            UnlockUtils.UnlockAllIfCreativeMode();
        }
    }
}
