using DaftAppleGames.SubnauticaPets.Pets;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.Patches
{
    [HarmonyPatch(typeof(BaseFoundationPiece))]
    internal class BaseFoundationPiecePatches
    {
        private const float CollisionFilterMargin = 2.0f;

        /// <summary>
        /// Adds a physical Pet blocker and a surrounding filter that lets non-Pet colliders pass through it.
        /// </summary>
        [HarmonyPatch(nameof(BaseFoundationPiece.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(BaseFoundationPiece __instance)
        {
            if (__instance.gameObject.name != "BaseMoonpool(Clone)" || ConfigFile.DisableMoonpoolCollider)
            {
                return;
            }

            Transform entranceTransform = __instance.transform.Find("entrance");
            if (!entranceTransform)
            {
                ModDebugLog.LogError("Could not find the Moonpool entrance collider transform.");
                return;
            }

            BoxCollider entranceCollider = entranceTransform.GetComponent<BoxCollider>();
            if (!entranceCollider)
            {
                ModDebugLog.LogError("Could not find the Moonpool entrance BoxCollider.");
                return;
            }

            GameObject blockerObject = new GameObject("PetMoonpoolBlocker");
            blockerObject.transform.SetParent(__instance.transform, true);
            blockerObject.transform.position = entranceCollider.transform.position + new Vector3(0.0f, -1.0f, 0.0f);
            blockerObject.transform.rotation = entranceCollider.transform.rotation;
            blockerObject.transform.localScale = entranceCollider.transform.lossyScale;

            BoxCollider blocker = blockerObject.AddComponent<BoxCollider>();
            blocker.size = entranceCollider.size + new Vector3(0.0f, 2.0f, 0.0f);

            GameObject filterObject = new GameObject("PetMoonpoolCollisionFilter");
            filterObject.transform.SetParent(blockerObject.transform, false);
            BoxCollider filterTrigger = filterObject.AddComponent<BoxCollider>();
            filterTrigger.size = blocker.size + Vector3.one * CollisionFilterMargin;
            filterTrigger.isTrigger = true;

            MoonpoolPetCollisionFilter filter = filterObject.AddComponent<MoonpoolPetCollisionFilter>();
            filter.Init(blocker, filterTrigger);
            filter.PrimeExistingOverlaps();
            Physics.SyncTransforms();
        }
    }
}
