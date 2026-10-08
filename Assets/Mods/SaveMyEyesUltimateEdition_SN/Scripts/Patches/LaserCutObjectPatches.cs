using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch(typeof(LaserCutObject))]
    internal static class LaserCutObjectPatches
    {
        [HarmonyPatch(nameof(LaserCutObject.OnEnable))]
        [HarmonyPostfix]
        public static void OnEnablePostfix(LaserCutObject __instance)
        {
            SaveMyParticleSystemsController particleController =
                __instance.laserCutFX.GetComponent<SaveMyParticleSystemsController>();
            if (!particleController)
            {
                particleController = __instance.laserCutFX.AddComponent<SaveMyParticleSystemsController>();
                particleController.Initialize(EffectUseCase.LaserCutter);
            }
            particleController.SetEffectActive(false);

            if (!__instance.laserCutStreak.GetComponent<SaveMyParticleSystemsController>())
            {
                __instance.laserCutStreak.AddComponent<SaveMyParticleSystemsController>().Initialize(EffectUseCase.LaserCutter);
            }

            if (!__instance.cutObject.GetComponent<SaveMyEyesMaterialController>())
            {
                __instance.cutObject.AddComponent<SaveMyEyesMaterialController>().Initialize(EffectUseCase.LaserCutter);
            }
        }

        [HarmonyPatch(nameof(LaserCutObject.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(LaserCutObject __instance)
        {
            bool effectActive = __instance.cutting && !__instance.isCutOpen && MiscSettings.flashes;
            SaveMyParticleSystemsController particleController =
                __instance.laserCutFX.GetComponent<SaveMyParticleSystemsController>();
            particleController.SetEffectActive(effectActive);
        }
    }
}
