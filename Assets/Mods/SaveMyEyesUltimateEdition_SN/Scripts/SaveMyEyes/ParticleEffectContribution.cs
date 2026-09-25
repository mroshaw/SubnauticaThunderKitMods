using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    internal sealed class ParticleEffectContribution : MonoBehaviour
    {
        private ParticleSystem[] particleSystems;
        private Light[] lights;
        private bool enforceActiveState;

        internal void Initialize(bool effectActive, bool shouldEnforceActiveState, bool includeLights)
        {
            if (particleSystems != null)
            {
                return;
            }

            enforceActiveState = shouldEnforceActiveState;
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                SaveMyEyesFromParticleFx.Register(particleSystem, effectActive);
            }

            lights = includeLights ? GetComponentsInChildren<Light>(true) : new Light[0];
            foreach (Light light in lights)
            {
                SaveMyEyesFromToolLights.Register(light);
            }
        }

        internal void SetEffectActive(bool effectActive)
        {
            if (particleSystems is null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                SaveMyEyesFromParticleFx.ConfigureFromGameState(particleSystem, effectActive);
            }
        }

        private void LateUpdate()
        {
            if (!enforceActiveState || particleSystems is null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                SaveMyEyesFromParticleFx.ConfigureFromGameState(particleSystem, true);
            }
        }

        private void OnDestroy()
        {
            if (particleSystems != null)
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    SaveMyEyesFromParticleFx.Unregister(particleSystem);
                }
            }

            if (lights != null)
            {
                foreach (Light light in lights)
                {
                    SaveMyEyesFromToolLights.Unregister(light);
                }
            }
        }
    }
}
