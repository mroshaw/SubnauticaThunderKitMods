using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.MyFirstThunderKitMod
{
    /// <summary>
    /// Class to implement Harmony patches on the Player class
    /// </summary>
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {

        // Postfix patch on the Awake method
        // Spawn a prefab instance when the player is spawned
        [HarmonyPatch(nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(Player __instance)
        {
            MyFirstThunderKitModPlugin.ModLogger.LogInfo($"Spawning Prefab instance...");

            // Get prefab from Asset Bundle then spawn an instance in front of the player
            GameObject myPrefab = MyFirstThunderKitModPlugin.MyAssetBundle.LoadAsset<GameObject>("MyFirstPrefab.prefab");
            MyFirstThunderKitModPlugin.ModLogger.LogInfo($"Found prefab in Asset Bundle. Creating instance...");
            GameObject newPrefabInstance = Object.Instantiate(myPrefab, __instance.transform, true);
            // Move the transform to 2 units in front of the player
            newPrefabInstance.transform.localPosition = Vector3.zero + __instance.transform.forward * 2;
            // Scale the instance - you could also set the scale in your prefab in Unity
            newPrefabInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            newPrefabInstance.name = "MyNewPrefabInstance";
            
            MyFirstThunderKitModPlugin.ModLogger.LogInfo($"Prefab instance spawned!");
        }
    }
}