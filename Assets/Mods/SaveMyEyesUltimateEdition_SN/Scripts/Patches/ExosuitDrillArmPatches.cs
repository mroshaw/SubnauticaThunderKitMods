using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    internal static class ExosuitDrillArmPatches
    {
        [HarmonyPatch(typeof(VFXSurfaceTypeManager), nameof(VFXSurfaceTypeManager.Play),
            typeof(VFXSurfaceTypes), typeof(VFXEventTypes), typeof(Vector3), typeof(Quaternion), typeof(Transform))]
        [HarmonyPostfix]
        private static void PlayPostfix(VFXEventTypes eventType, ParticleSystem __result)
        {
            if (eventType != VFXEventTypes.exoDrill || !__result)
            {
                return;
            }

            ParticleEffectContribution contribution =
                __result.gameObject.AddComponent<ParticleEffectContribution>();
            contribution.Initialize(true, false, true);
        }

        [HarmonyPatch(typeof(VFXController), "Start")]
        [HarmonyPostfix]
        private static void StartPostfix(VFXController __instance)
        {
            if (!__instance.GetComponent<ExosuitDrillArm>())
            {
                return;
            }

            foreach (VFXController.VFXEmitter emitter in __instance.emitters)
            {
                if (emitter != null && emitter.instanceGO)
                {
                    ParticleEffectContribution contribution =
                        emitter.instanceGO.AddComponent<ParticleEffectContribution>();
                    contribution.Initialize(false, false, true);
                }
            }
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
            ParticleEffectContribution contribution =
                __instance.GetComponent<ParticleEffectContribution>();
            if (contribution)
            {
                contribution.SetEffectActive(false);
            }
        }

        [HarmonyPatch(typeof(ExosuitDrillArm), "StopEffects")]
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
            if (!controller || !controller.GetComponent<ExosuitDrillArm>() || controller.emitters is null ||
                emitterIndex < 0 || emitterIndex >= controller.emitters.Length)
            {
                return;
            }

            VFXController.VFXEmitter emitter = controller.emitters[emitterIndex];
            if (emitter != null && emitter.instanceGO)
            {
                ParticleEffectContribution contribution =
                    emitter.instanceGO.GetComponent<ParticleEffectContribution>();
                if (contribution)
                {
                    contribution.SetEffectActive(effectActive);
                }
            }
        }
    }
}
