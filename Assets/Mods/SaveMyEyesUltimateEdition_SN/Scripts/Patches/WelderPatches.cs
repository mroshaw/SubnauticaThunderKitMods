using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch]
    internal static class WelderPatches
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
        private static void UpdatePostfix(Welder __instance)
        {
            SetParticleEffectsActive(__instance, __instance.fxIsPlaying && MiscSettings.flashes);
        }

        [HarmonyPatch(typeof(Welder), nameof(Welder.StopWeldingFX))]
        [HarmonyPostfix]
        private static void StopWeldingFXPostfix(Welder __instance)
        {
            SetParticleEffectsActive(__instance, false);
        }

        private static void SetParticleEffectsActive(Welder welder, bool effectActive)
        {
            if (!welder.fxControl)
            {
                return;
            }

            foreach (VFXController.VFXEmitter emitter in welder.fxControl.emitters)
            {
                if (emitter.instanceGO)
                {
                    emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>().SetEffectActive(effectActive);
                }
            }
        }
    }
}
