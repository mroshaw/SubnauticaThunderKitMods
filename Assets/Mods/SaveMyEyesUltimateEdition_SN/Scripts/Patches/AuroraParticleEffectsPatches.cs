
using System;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch(typeof(PrefabSpawnBase), "SpawnObj")]
    internal static class AuroraParticleEffectsPatches
    {
        [HarmonyPrefix]
        private static void SpawnObjPrefix(PrefabSpawnBase __instance, ref Action<GameObject> spawnCallback)
        {
            PrefabSpawn prefabSpawn = __instance as PrefabSpawn;
            if (!prefabSpawn || !prefabSpawn.prefab || !IsAuroraDamageEffectsPrefab(prefabSpawn.prefab.name))
            {
                return;
            }

            Action<GameObject> originalCallback = spawnCallback;
            spawnCallback = spawnedObject =>
            {
                if (spawnedObject)
                {
                    Transform[] transforms = spawnedObject.GetComponentsInChildren<Transform>(true);
                    foreach (Transform effectTransform in transforms)
                    {
                        if (!IsTargetedEffect(effectTransform.name))
                        {
                            continue;
                        }

                        ParticleEffectContribution contribution =
                            effectTransform.gameObject.AddComponent<ParticleEffectContribution>();
                        contribution.Initialize(true, true, false);
                    }
                }

                if (originalCallback != null)
                {
                    originalCallback(spawnedObject);
                }
            };
        }

        private static bool IsTargetedEffect(string effectName)
        {
            return effectName.StartsWith("xSparksOrange_", StringComparison.Ordinal) ||
                   effectName.StartsWith("xSparksBlue_", StringComparison.Ordinal) ||
                   effectName.StartsWith("xSparksElec_", StringComparison.Ordinal);
        }

        private static bool IsAuroraDamageEffectsPrefab(string prefabName)
        {
            return prefabName == "Ship_Interior_FirstLevelFX" ||
                   prefabName == "Ship_Interior_SecondLevelFX" ||
                   prefabName == "Ship_Interior_PowerRoomFX";
        }
    }
}
