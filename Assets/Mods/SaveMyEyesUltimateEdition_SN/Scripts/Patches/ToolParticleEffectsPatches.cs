using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch]
    internal static class ToolParticleEffectsPatches
    {
        [HarmonyPatch(typeof(VFXController), nameof(VFXController.SpawnFX))]
        [HarmonyPostfix]
        private static void SpawnFXPostfix(VFXController __instance, int i)
        {
            if (__instance.GetComponentInParent<Welder>())
            {
                __instance.emitters[i].instanceGO.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.RepairTool);
            }
        }

        [HarmonyPatch(typeof(Welder), nameof(Welder.Update))]
        [HarmonyPostfix]
        private static void WelderUpdatePostfix(Welder __instance)
        {
            ApplyFlashesSettingIfChanged(__instance.fxControl);
        }

        internal static void ApplyFlashesSettingIfChanged(VFXController vfxController)
        {
            if (!vfxController)
            {
                return;
            }
            foreach (VFXController.VFXEmitter emitter in vfxController.emitters)
            {
                if (!(emitter is null) && emitter.instanceGO)
                {
                    SaveMyParticleSystemsController controller = emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>();
                    if (controller)
                    {
                        // A live game-option change needs enforcement; idle frames write no modules.
                        controller.ApplyFlashesSettingIfChanged();
                    }
                }
            }
        }

        // Attached controllers limit these general game hooks to the supported tool emitters.
        [HarmonyPatch(typeof(VFXController), nameof(VFXController.Play), typeof(int))]
        [HarmonyPostfix]
        private static void PlayPostfix(VFXController __instance, int i)
        {
            ConfigurePlayback(__instance, i, true);
        }

        [HarmonyPatch(typeof(VFXController), nameof(VFXController.Stop), typeof(int))]
        [HarmonyPostfix]
        private static void StopPostfix(VFXController __instance, int i)
        {
            ConfigurePlayback(__instance, i, false);
        }

        [HarmonyPatch(typeof(VFXLateTimeParticles), nameof(VFXLateTimeParticles.Play))]
        [HarmonyPostfix]
        private static void LateTimePlayPostfix(VFXLateTimeParticles __instance)
        {
            SaveMyParticleSystemsController controller = __instance.GetComponent<SaveMyParticleSystemsController>();
            if (controller)
            {
                // Play's simulation has already used the configured rates and bursts.
                // Reapply after the game unconditionally enables child emissions.
                controller.BeginPlayback(true);
            }
        }

        [HarmonyPatch(typeof(VFXLateTimeParticles), nameof(VFXLateTimeParticles.Stop))]
        [HarmonyPostfix]
        private static void LateTimeStopPostfix(VFXLateTimeParticles __instance)
        {
            SaveMyParticleSystemsController controller = __instance.GetComponent<SaveMyParticleSystemsController>();
            if (controller)
            {
                controller.SetEffectActive(false);
            }
        }

        private static void ConfigurePlayback(VFXController vfxController, int index, bool playing)
        {
            VFXController.VFXEmitter emitter = vfxController.emitters[index];
            if (emitter is null || !emitter.instanceGO)
            {
                return;
            }
            SaveMyParticleSystemsController controller = emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>();
            if (!controller)
            {
                return;
            }
            if (playing)
            {
                controller.BeginPlayback(emitter.lateTime);
            }
            else
            {
                controller.SetEffectActive(false);
            }
        }
    }
}
