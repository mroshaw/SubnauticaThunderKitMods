using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    internal static class LaserCutterPatches
    {
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(PlayerTool __instance)
        {
            LaserCutter laserCutter = __instance as LaserCutter;
            if (laserCutter && laserCutter.fxLight)
            {
                laserCutter.fxLight.gameObject.AddComponent<SaveMyEyesLightController>();
            }
        }

        [HarmonyPatch(typeof(LaserCutter), "StartLaserCuttingFX")]
        [HarmonyPostfix]
        public static void StartLaserCuttingFXPostfix(LaserCutter __instance)
        {
            SetParticleEffectsActive(__instance, __instance.fxIsPlaying && MiscSettings.flashes);
            ConfigureActiveLight(__instance);
        }

        [HarmonyPatch(typeof(VFXController), "SpawnFX")]
        [HarmonyPostfix]
        public static void SpawnFXPostfix(VFXController __instance, int i)
        {
            if (__instance.GetComponent<LaserCutter>())
            {
                __instance.emitters[i].instanceGO.AddComponent<SaveMyParticleSystemsController>();
            }
        }

        [HarmonyPatch(typeof(LaserCutter), "StopLaserCuttingFX")]
        [HarmonyPostfix]
        public static void StopLaserCuttingFXPostfix(LaserCutter __instance)
        {
            SetParticleEffectsActive(__instance, false);
        }

        [HarmonyPatch(typeof(LaserCutter), "RandomizeIntensity")]
        [HarmonyPostfix]
        public static void RandomizeIntensityPostfix(LaserCutter __instance)
        {
            __instance.lightIntensity *= ConfigFile.ToolLightIntensity;
        }

        [HarmonyPatch(typeof(LaserCutter), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(LaserCutter __instance)
        {
            ConfigureActiveLight(__instance);
        }

        private static void SetParticleEffectsActive(LaserCutter laserCutter, bool effectActive)
        {
            foreach (VFXController.VFXEmitter emitter in laserCutter.fxControl.emitters)
            {
                if (emitter.instanceGO)
                {
                    SaveMyParticleSystemsController controller =
                        emitter.instanceGO.GetComponent<SaveMyParticleSystemsController>();
                    if (controller)
                    {
                        controller.SetEffectActive(effectActive);
                    }
                }
            }
        }

        private static void ConfigureActiveLight(LaserCutter laserCutter)
        {
            if (!laserCutter.fxIsPlaying)
            {
                return;
            }

            bool enableLight = MiscSettings.flashes && ConfigFile.ToolLightIntensity > 0.0f;
            laserCutter.fxLight.enabled = enableLight;
            if (!enableLight)
            {
                laserCutter.fxLight.intensity = 0.0f;
            }
        }
    }
}
