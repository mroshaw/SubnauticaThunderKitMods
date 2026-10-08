using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch]
    internal static class ExosuitDrillArmPatches
    {
        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.OnHit))]
        [HarmonyPrefix]
        private static void OnHitPrefix(ExosuitDrillArm __instance,
            out SaveMyParticleSystemsController __state)
        {
            __state = null;
            if (SaveMyEyesUltimateEditionPlugin.ConfigFile.PrawnDrillParticleDensity > 0.0f)
            {
                return;
            }

            VFXController.VFXEmitter emitter = __instance.fxControl.emitters[0];
            if (emitter.instanceGO)
            {
                SaveMyParticleSystemsController particleController =
                    emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>();
                if (particleController && particleController.EffectActive)
                {
                    ParticleSystem.EmissionModule emission = emitter.fxPS.emission;
                    emission.enabled = true;
                    __state = particleController;
                }
            }
        }

        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.OnHit))]
        [HarmonyPostfix]
        private static void OnHitPostfix(SaveMyParticleSystemsController __state)
        {
            if (__state)
            {
                __state.ApplyEmissionState();
            }
        }

        [HarmonyPatch(typeof(VFXSurfaceTypeManager), nameof(VFXSurfaceTypeManager.Play),
            typeof(VFXSurfaceTypes), typeof(VFXEventTypes), typeof(Vector3), typeof(Quaternion), typeof(Transform))]
        [HarmonyPostfix]
        private static void PlayPostfix(VFXEventTypes eventType, ParticleSystem __result)
        {
            if (eventType != VFXEventTypes.exoDrill || !__result)
            {
                return;
            }

            __result.gameObject.AddComponent<SaveMyEyesLightController>().Initialize(EffectUseCase.PrawnDrill);
            __result.gameObject.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.PrawnDrill);
        }

        [HarmonyPatch(typeof(VFXController), nameof(VFXController.SpawnFX))]
        [HarmonyPostfix]
        private static void SpawnFXPostfix(VFXController __instance, int i)
        {
            if (!__instance.GetComponent<ExosuitDrillArm>())
            {
                return;
            }

            GameObject effectRoot = __instance.emitters[i].instanceGO;
            effectRoot.AddComponent<SaveMyEyesLightController>().Initialize(EffectUseCase.PrawnDrill);
            SaveMyParticleSystemsController particleController =
                effectRoot.AddComponent<SaveMyParticleSystemsController>();
            particleController.Initialize(EffectUseCase.PrawnDrill);
            particleController.SetEffectActive(false);
        }

        [HarmonyPatch(typeof(VFXController), nameof(VFXController.Play), typeof(int))]
        [HarmonyPostfix]
        private static void PlayPostfix(VFXController __instance, int i)
        {
            ConfigureEmitter(__instance, i, true);
        }

        [HarmonyPatch(typeof(VFXController), nameof(VFXController.Stop), typeof(int))]
        [HarmonyPostfix]
        private static void StopPostfix(VFXController __instance, int i)
        {
            ConfigureEmitter(__instance, i, false);
        }

        [HarmonyPatch(typeof(VFXLateTimeParticles), nameof(VFXLateTimeParticles.Stop))]
        [HarmonyPostfix]
        private static void LateTimeParticlesStopPostfix(VFXLateTimeParticles __instance)
        {
            SaveMyParticleSystemsController particleController =
                __instance.GetComponent<SaveMyParticleSystemsController>();
            if (particleController)
            {
                particleController.SetEffectActive(false);
            }
        }

        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.StopEffects))]
        [HarmonyPostfix]
        private static void StopEffectsPostfix(ExosuitDrillArm __instance)
        {
            if (__instance.fxControl)
            {
                ConfigureEmitter(__instance.fxControl, 0, false);
            }
        }

        private static void ConfigureEmitter(VFXController controller, int emitterIndex, bool effectActive)
        {
            if (!controller.GetComponent<ExosuitDrillArm>())
            {
                return;
            }

            VFXController.VFXEmitter emitter = controller.emitters[emitterIndex];
            if (emitter.instanceGO)
            {
                SaveMyParticleSystemsController particleController =
                    emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>();
                if (particleController)
                {
                    particleController.SetEffectActive(effectActive);
                }
            }
        }
    }
}
