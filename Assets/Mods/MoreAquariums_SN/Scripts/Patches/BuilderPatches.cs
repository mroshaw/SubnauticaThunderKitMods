using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// Patches to allow aquariums to be constructed in more flexible locations
    /// </summary>
    [HarmonyPatch(typeof(Builder))]
    internal class BuilderPatches
    {
        /// <summary>
        /// Remove checks against BaseCell colliders, as these block perfectly valid placement options for the aquariums 
        /// </summary>
        [HarmonyPatch(nameof(Builder.GetOverlappedColliders), typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(int), typeof(QueryTriggerInteraction), typeof(List<Collider>))]
        [HarmonyPostfix]
        public static void GetOverlappedColliders_Postfix(Vector3 position, Quaternion rotation, Vector3 extents, int layerMask, QueryTriggerInteraction trigger, List<Collider> results)
        {
            // Check if this is one of our new aquariums
            Constructable constructable = Builder.prefab.GetComponent<Constructable>();
            if (!constructable || !constructable.gameObject.GetComponent<AquariumBase>())
            {
                return;
            }
            foreach (Collider collider in results.ToArray())
            {
                if (GetRootObjectName(collider) == "BaseCell(Clone)")
                {
                    // ModDebugLog.LogDebug("Ignoring BaseCell(Clone) collider");
                    // results.Remove(collider);
                }
            }
        }

        /// <summary>
        /// Gets the name of the root GameObject for the given collider
        /// </summary>
        private static string GetRootObjectName(Collider collider)
        {
            GameObject gameObject = collider.gameObject;
            Transform transform = gameObject.transform;
            while (transform != null)
            {
                if (transform.GetComponent<IBaseModuleGeometry>() != null)
                {
                    gameObject = transform.gameObject;
                    break;
                }

                if (transform.GetComponent<PrefabIdentifier>() != null)
                {
                    gameObject = transform.gameObject;
                    break;
                }

                if (transform.GetComponent<SceneObjectIdentifier>() != null)
                {
                    gameObject = transform.gameObject;
                    break;
                }

                transform = transform.parent;
            }

            return gameObject.name;
        }
    }
}
