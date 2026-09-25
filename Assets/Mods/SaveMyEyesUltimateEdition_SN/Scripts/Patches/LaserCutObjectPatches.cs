using System.Collections.Generic;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches
{
    [HarmonyPatch(typeof(LaserCutObject))]
    internal static class LaserCutObjectPatches
    {
        private static readonly Dictionary<LaserCutObject, LaserCutEffects> Contributions = new Dictionary<LaserCutObject, LaserCutEffects>();

        private sealed class LaserCutEffects
        {
            private readonly ParticleSystem[] particleSystems;
            private readonly TrailRenderer trail;
            private readonly LineRenderer line;
            private readonly Material cutMaterial;

            internal LaserCutEffects(LaserCutObject laserCutObject)
            {
                particleSystems = laserCutObject.laserCutFX
                    ? laserCutObject.laserCutFX.GetComponentsInChildren<ParticleSystem>(true)
                    : new ParticleSystem[0];

                if (laserCutObject.laserCutStreak)
                {
                    trail = laserCutObject.laserCutStreak.GetComponent<TrailRenderer>();
                    line = laserCutObject.laserCutStreak.GetComponent<LineRenderer>();
                }

                if (laserCutObject.cutObject)
                {
                    MeshRenderer cutRenderer = laserCutObject.cutObject.GetComponent<MeshRenderer>();
                    if (cutRenderer)
                    {
                        cutMaterial = cutRenderer.material;
                    }
                }

                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    SaveMyEyesFromParticleFx.Register(particleSystem, false);
                }

                SaveMyEyesFromParticleFx.Register(trail);
                SaveMyEyesFromParticleFx.Register(line);
                SaveMyEyesFromEmitterMaterials.Register(cutMaterial);
            }

            internal void Configure(bool effectActive)
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    SaveMyEyesFromParticleFx.ConfigureFromGameState(particleSystem, effectActive);
                }
            }

            internal void Unregister()
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    SaveMyEyesFromParticleFx.Unregister(particleSystem);
                }

                SaveMyEyesFromParticleFx.Unregister(trail);
                SaveMyEyesFromParticleFx.Unregister(line);
                SaveMyEyesFromEmitterMaterials.Unregister(cutMaterial);
            }
        }

        [HarmonyPatch(nameof(LaserCutObject.OnEnable))]
        [HarmonyPostfix]
        public static void OnEnablePostfix(LaserCutObject __instance)
        {
            LaserCutEffects existingContribution;
            if (Contributions.TryGetValue(__instance, out existingContribution))
            {
                existingContribution.Configure(false);
                return;
            }

            Contributions[__instance] = new LaserCutEffects(__instance);

            LaserCutObjectContributionCleanup cleanup = __instance.GetComponent<LaserCutObjectContributionCleanup>();
            if (!cleanup)
            {
                cleanup = __instance.gameObject.AddComponent<LaserCutObjectContributionCleanup>();
            }
            cleanup.Initialize(__instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(LaserCutObject __instance, bool ___cutting)
        {
            LaserCutEffects contribution;
            if (!Contributions.TryGetValue(__instance, out contribution))
            {
                return;
            }

            bool effectActive = ___cutting && !__instance.isCutOpen && MiscSettings.flashes;
            contribution.Configure(effectActive);
        }

        internal static void UnregisterContribution(LaserCutObject laserCutObject)
        {
            LaserCutEffects contribution;
            if (!ReferenceEquals(laserCutObject, null) && Contributions.TryGetValue(laserCutObject, out contribution))
            {
                contribution.Unregister();
                Contributions.Remove(laserCutObject);
            }
        }
    }
}
