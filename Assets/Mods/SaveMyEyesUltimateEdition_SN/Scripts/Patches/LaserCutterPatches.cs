using System.Collections.Generic;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    internal static class LaserCutterPatches
    {
        private static readonly Dictionary<LaserCutter, ParticleSystem[]> ParticleContributions = new Dictionary<LaserCutter, ParticleSystem[]>();

        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(PlayerTool __instance)
        {
            LaserCutter laserCutter = __instance as LaserCutter;
            if (laserCutter && laserCutter.fxLight)
            {
                SaveMyEyesFromToolLights.Register(laserCutter.fxLight);
            }
        }

        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDestroy))]
        [HarmonyPrefix]
        public static void OnDestroyPrefix(PlayerTool __instance)
        {
            LaserCutter laserCutter = __instance as LaserCutter;
            if (!laserCutter)
            {
                return;
            }

            if (laserCutter.fxLight)
            {
                SaveMyEyesFromToolLights.Unregister(laserCutter.fxLight);
            }

            UnregisterParticleEffects(laserCutter);
        }

        [HarmonyPatch(typeof(LaserCutter), "StartLaserCuttingFX")]
        [HarmonyPrefix]
        public static void StartLaserCuttingFXPrefix(bool ___fxIsPlaying, out bool __state)
        {
            __state = ___fxIsPlaying;
        }

        [HarmonyPatch(typeof(LaserCutter), "StartLaserCuttingFX")]
        [HarmonyPostfix]
        public static void StartLaserCuttingFXPostfix(LaserCutter __instance, bool ___fxIsPlaying, bool __state)
        {
            if (!__state && ___fxIsPlaying && MiscSettings.flashes)
            {
                RegisterParticleEffects(__instance);
            }

            ConfigureActiveLight(__instance, ___fxIsPlaying);
        }

        [HarmonyPatch(typeof(LaserCutter), "StopLaserCuttingFX")]
        [HarmonyPrefix]
        public static void StopLaserCuttingFXPrefix(LaserCutter __instance)
        {
            UnregisterParticleEffects(__instance);
        }

        [HarmonyPatch(typeof(LaserCutter), "RandomizeIntensity")]
        [HarmonyPostfix]
        public static void RandomizeIntensityPostfix(ref float ___lightIntensity)
        {
            ___lightIntensity *= ConfigFile.ToolLightIntensity;
        }

        [HarmonyPatch(typeof(LaserCutter), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(LaserCutter __instance, bool ___fxIsPlaying)
        {
            ConfigureActiveLight(__instance, ___fxIsPlaying);
            ConfigureParticleEffects(__instance, ___fxIsPlaying && MiscSettings.flashes);
        }

        private static void ConfigureActiveLight(LaserCutter laserCutter, bool fxIsPlaying)
        {
            if (!laserCutter.fxLight || !fxIsPlaying)
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

        private static void RegisterParticleEffects(LaserCutter laserCutter)
        {
            UnregisterParticleEffects(laserCutter);
            if (!laserCutter.fxControl || laserCutter.fxControl.emitters is null)
            {
                return;
            }

            List<ParticleSystem> particleSystems = new List<ParticleSystem>();
            foreach (VFXController.VFXEmitter emitter in laserCutter.fxControl.emitters)
            {
                if (emitter != null && emitter.instanceGO)
                {
                    particleSystems.AddRange(emitter.instanceGO.GetComponentsInChildren<ParticleSystem>(true));
                }
            }

            ParticleSystem[] contributedParticleSystems = particleSystems.ToArray();
            ParticleContributions.Add(laserCutter, contributedParticleSystems);
            foreach (ParticleSystem particleSystem in contributedParticleSystems)
            {
                SaveMyEyesFromParticleFx.Register(particleSystem, true);
            }
        }

        private static void ConfigureParticleEffects(LaserCutter laserCutter, bool effectActive)
        {
            ParticleSystem[] particleSystems;
            if (!ParticleContributions.TryGetValue(laserCutter, out particleSystems))
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                SaveMyEyesFromParticleFx.ConfigureFromGameState(particleSystem, effectActive);
            }
        }

        private static void UnregisterParticleEffects(LaserCutter laserCutter)
        {
            ParticleSystem[] particleSystems;
            if (!ParticleContributions.TryGetValue(laserCutter, out particleSystems))
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                SaveMyEyesFromParticleFx.Unregister(particleSystem);
            }

            ParticleContributions.Remove(laserCutter);
        }
    }
}
