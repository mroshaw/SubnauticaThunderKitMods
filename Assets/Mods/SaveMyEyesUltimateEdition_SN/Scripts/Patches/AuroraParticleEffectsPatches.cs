using System;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch]
    internal static class AuroraParticleEffectsPatches
    {
        [HarmonyPatch(typeof(UniqueIdentifier), nameof(UniqueIdentifier.Awake))]
        [HarmonyPostfix]
        private static void UniqueIdentifierAwakePostfix(UniqueIdentifier __instance)
        {
            if (!(__instance is PrefabIdentifier))
            {
                return;
            }

            switch (__instance.ClassId)
            {
                // Independently streamed blue, electrical and orange sparks (1s and 3s variants).
                case "ccda323d-2b8c-4bd2-b55e-b2def79a0283":
                case "b067ee9e-6613-4631-8c77-86e4ecbc3c98":
                case "361b23ed-58dd-4f45-9c5f-072fa66db88a":
                case "cb05f6ff-d7d6-4e8a-83ab-a1a37346c622":
                case "78afcc32-7963-4939-a894-52a69a8faa9b":
                case "d50be841-735c-4e4b-bf5a-912056f0fb7a":
                    __instance.gameObject.AddComponent<SaveMyParticleSystemsController>();
                    break;

                // Aurora electrical sources also have a separate ElecLight.
                case "00ba4df5-a2bf-433b-8e7b-bd02c69e1c60":
                case "6e4b4259-becc-4d2c-b56a-03ccedbc4672":
                case "3274b205-b153-41b6-9736-f3972e38f0ad":
                    __instance.gameObject.AddComponent<SaveMyParticleSystemsController>();
                    __instance.gameObject.AddComponent<SaveMyEyesLightController>();
                    break;
            }
        }

        [HarmonyPatch(typeof(PrefabSpawnBase), nameof(PrefabSpawnBase.Awake))]
        [HarmonyPostfix]
        private static void PrefabSpawnAwakePostfix(PrefabSpawnBase __instance)
        {
            // PrefabSpawn completes OnAwake instantiation synchronously.
            if (!(__instance is PrefabSpawn prefabSpawn) ||
                prefabSpawn.spawnType != SpawnType.OnAwake ||
                prefabSpawn.spawnedObj == null ||
                !IsAuroraDamageEffectsPrefab(prefabSpawn.spawnedObj.name))
            {
                return;
            }

            Transform[] transforms = prefabSpawn.spawnedObj.GetComponentsInChildren<Transform>(true);
            foreach (Transform effectTransform in transforms)
            {
                if (IsTargetedEffect(effectTransform.name))
                {
                    effectTransform.gameObject.AddComponent<SaveMyParticleSystemsController>();
                }
            }
        }

        private static bool IsTargetedEffect(string effectName)
        {
            return effectName.StartsWith("xSparksOrange_", StringComparison.Ordinal) ||
                   effectName.StartsWith("xSparksBlue_", StringComparison.Ordinal) ||
                   effectName.StartsWith("xSparksElec_", StringComparison.Ordinal) ||
                   effectName == "xSprks";
        }

        private static bool IsAuroraDamageEffectsPrefab(string objectName)
        {
            return objectName.StartsWith("Ship_Interior_FirstLevelFX", StringComparison.Ordinal) ||
                   objectName.StartsWith("Ship_Interior_SecondLevelFX", StringComparison.Ordinal) ||
                   objectName.StartsWith("Ship_Interior_PowerRoomFX", StringComparison.Ordinal) ||
                   objectName.StartsWith("Ship_Interior_InaccessibleFX", StringComparison.Ordinal);
        }
    }
}
