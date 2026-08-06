using DaftAppleGames.ModTools.Extensions;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// 
    /// </summary>
    [HarmonyPatch(typeof(StorageContainer))]
    internal class StorageContainerPatches
    {
        [HarmonyPatch(nameof(StorageContainer.CreateContainer))]
        [HarmonyPrefix]
        public static void CreateContainer_Prefix(StorageContainer __instance)
        {
            // ModDebugLog.LogDebug($"StorageContainer.CreateContainer called on GameObject {__instance.gameObject.name}");
            // ModDebugLog.LogDebug($"1 Container: {__instance.container}, StorageRoot: {__instance.storageRoot}, StorageLabel: {__instance.storageLabel}");

            if (!__instance.storageRoot)
            {
                __instance.storageRoot = __instance.GetComponentInChildren<ChildObjectIdentifier>(true);
            }
            // ModDebugLog.LogDebug($"2 Container: {__instance.container}, StorageRoot: {__instance.storageRoot}, StorageLabel: {__instance.storageLabel}");

            if (!__instance.storageRoot)
            {
                GameObject newStorageRoot = new GameObject("StorageRoot");
                newStorageRoot.transform.parent = __instance.transform;
                newStorageRoot.transform.LocalZero();
                ChildObjectIdentifier storageRootIdentifier = newStorageRoot.EnsureComponent<ChildObjectIdentifier>();
                __instance.storageRoot = storageRootIdentifier;
            }
            // ModDebugLog.LogDebug($"3 Container: {__instance.container}, StorageRoot: {__instance.storageRoot}, StorageLabel: {__instance.storageLabel}");
        }
    }
}
