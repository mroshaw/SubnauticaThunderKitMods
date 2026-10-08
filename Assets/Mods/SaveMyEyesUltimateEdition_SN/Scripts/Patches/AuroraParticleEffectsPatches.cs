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
                    __instance.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.AuroraSparks);
                    break;

                // Aurora electrical sources also have a separate ElecLight.
                case "00ba4df5-a2bf-433b-8e7b-bd02c69e1c60":
                case "6e4b4259-becc-4d2c-b56a-03ccedbc4672":
                case "3274b205-b153-41b6-9736-f3972e38f0ad":
                    __instance.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.AuroraSparks);
                    __instance.gameObject.AddComponent<SaveMyEyesLightController>().Initialize(EffectUseCase.AuroraSparks);
                    break;

                // Extinguishable Aurora fires spawn their particle visuals later, in Fire.Start.
                case "14bbf7f0-4276-48bf-868b-317b366edd16":
                case "3877d31d-37a5-4c94-8eef-881a500c58bc":
                case "afe53ea1-d2a8-4f76-8ffb-d41ff6046b52":
                    Fire fire = __instance.GetComponentInChildren<Fire>(true);
                    if (fire.fireLight)
                    {
                        fire.fireLight.gameObject.AddComponent<SaveMyEyesLightController>().Initialize(EffectUseCase.AuroraFlames);
                    }
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
                    effectTransform.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.AuroraSparks);
                }
                else if (effectTransform.name == "xFireHuge3" || effectTransform.name == "xFireBall")
                {
                    effectTransform.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.AuroraFlames, false);
                }
                else if (IsFlameLight(effectTransform.name))
                {
                    effectTransform.gameObject.AddComponent<SaveMyEyesLightController>().Initialize(EffectUseCase.AuroraFlames);
                }
            }
        }

        [HarmonyPatch(typeof(VFXExtinguishableFire), nameof(VFXExtinguishableFire.Start))]
        [HarmonyPostfix]
        private static void FireVisualsStartPostfix(VFXExtinguishableFire __instance)
        {
            PrefabIdentifier identifier = __instance.GetComponentInParent<PrefabIdentifier>();
            if (identifier && IsAuroraFirePrefab(identifier.ClassId))
            {
                __instance.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.AuroraFlames);
            }
        }

        [HarmonyPatch(typeof(VFXExtinguishableFire.FireElement), nameof(VFXExtinguishableFire.FireElement.UpdateParticles))]
        [HarmonyPostfix]
        private static void FireParticlesPostfix(VFXExtinguishableFire.FireElement __instance)
        {
            SaveMyParticleSystemsController controller = __instance.gameObject.GetComponentInParent<SaveMyParticleSystemsController>();
            if (controller && controller.UseCase == EffectUseCase.AuroraFlames)
            {
                controller.CaptureAndApplyEmissionRate(__instance.particleSystem);
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.LateUpdate))]
        [HarmonyPrefix]
        private static void FireLateUpdatePrefix(Fire __instance, out SaveMyEyesLightController __state)
        {
            __state = __instance.fireLight ? __instance.fireLight.GetComponent<SaveMyEyesLightController>() : null;
            if (__state)
            {
                __state.PrepareCurrentIntensity(__instance.fireLight);
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.LateUpdate))]
        [HarmonyPostfix]
        private static void FireLateUpdatePostfix(Fire __instance, SaveMyEyesLightController __state)
        {
            if (__state && __instance.fireLight)
            {
                __state.CaptureAndApplyCurrentIntensity(__instance.fireLight);
            }
        }

        private static bool IsAuroraFirePrefab(string classId)
        {
            return classId == "14bbf7f0-4276-48bf-868b-317b366edd16" ||
                   classId == "3877d31d-37a5-4c94-8eef-881a500c58bc" ||
                   classId == "afe53ea1-d2a8-4f76-8ffb-d41ff6046b52";
        }

        private static bool IsFlameLight(string objectName)
        {
            return objectName.StartsWith("Light_Fire", StringComparison.Ordinal) ||
                   objectName.StartsWith("Light_Spot_Fire_Dark", StringComparison.Ordinal) ||
                   objectName.StartsWith("Point_PowerRoomFire", StringComparison.Ordinal);
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
