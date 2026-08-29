using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    [HarmonyPatch(typeof(CuteFish))]
    internal static class CuteFishPatches
    {
        private const string CameraPrefabName = "CuddleCamCamera.prefab";

        /// <summary>
        /// Adds a CuddleCam source to each active Cuddlefish.
        /// </summary>
        [HarmonyPatch(nameof(CuteFish.Start))]
        [HarmonyPostfix]
        private static void Start_Postfix(CuteFish __instance)
        {
            ModDebugLog.LogDebug(
                $"CuteFish.Start postfix invoked for '{__instance.name}'. " +
                $"ActiveSelf={__instance.gameObject.activeSelf}, ActiveInHierarchy={__instance.gameObject.activeInHierarchy}.");

            if (__instance.GetComponentInChildren<CuddleCamSource>(true))
            {
                ModDebugLog.LogDebug($"'{__instance.name}' already has a CuddleCam source; skipping creation.");
                return;
            }

            GameObject cameraGameObject =
                ModAssetUtils.GetPrefabInstanceFromAssetBundle(CameraPrefabName, false);

            if (!cameraGameObject)
            {
                ModDebugLog.LogError(
                    $"Could not create CuddleCam source prefab '{CameraPrefabName}'.");
                return;
            }

            CuddleCamSource source = cameraGameObject.GetComponent<CuddleCamSource>();
            if (!source)
            {
                ModDebugLog.LogError(
                    $"CuddleCam source prefab '{CameraPrefabName}' has no CuddleCamSource component.");
                Object.Destroy(cameraGameObject);
                return;
            }

            ModDebugLog.LogDebug(
                $"Created camera source '{cameraGameObject.name}' for '{__instance.name}'. " +
                $"ActiveSelf={cameraGameObject.activeSelf}, ActiveInHierarchy={cameraGameObject.activeInHierarchy}, " +
                $"SourceEnabled={source.enabled}.");

            source.AttachTo(__instance);
            ModDebugLog.LogDebug(
                $"Attached camera source '{cameraGameObject.name}' to '{__instance.name}'. " +
                $"Parent={cameraGameObject.transform.parent}, ActiveSelf={cameraGameObject.activeSelf}, " +
                $"ActiveInHierarchy={cameraGameObject.activeInHierarchy}.");

            cameraGameObject.SetActive(true);
            ModDebugLog.LogDebug(
                $"Activated camera source '{cameraGameObject.name}'. ActiveSelf={cameraGameObject.activeSelf}, " +
                $"ActiveInHierarchy={cameraGameObject.activeInHierarchy}, SourceActiveAndEnabled={source.isActiveAndEnabled}.");
        }
    }
}
