using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// MonoBehaviour class to manage particle, trail and line effects
    /// </summary>
    internal class SaveMyParticleSystemsController : MonoBehaviour
    {
        private Dictionary<ParticleSystem, ParticleEffectState> _particleSystems;
        private Dictionary<TrailRenderer, TrailEffectState> _trails;
        private Dictionary<LineRenderer, LineEffectState> _lines;
        private bool _effectActive;

        internal bool EffectActive => _effectActive;

        private sealed class ParticleEffectState
        {
            internal readonly bool InitialEmissionEnabled;
            internal readonly float InitialRateOverTimeMultiplier;
            internal readonly float InitialRateOverDistanceMultiplier;
            internal readonly float InitialStartSizeMultiplier;
            internal readonly float InitialStartSizeXMultiplier;
            internal readonly float InitialStartSizeYMultiplier;
            internal readonly float InitialStartSizeZMultiplier;
            internal readonly bool Uses3DStartSize;
            internal readonly float InitialStartSpeedMultiplier;
            internal readonly ParticleSystem.MinMaxGradient InitialStartColor;
            internal readonly bool HasLight;
            internal readonly float InitialLightIntensityMultiplier;
            internal bool EffectActive;
            internal float CurrentSizeModifier = 1.0f;
            internal float CurrentSpeedModifier = 1.0f;
            internal float CurrentBrightnessModifier = 1.0f;

            internal ParticleEffectState(ParticleSystem particleSystem, bool effectActive)
            {
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                ParticleSystem.MainModule main = particleSystem.main;
                ParticleSystem.LightsModule lights = particleSystem.lights;

                InitialEmissionEnabled = emission.enabled;
                InitialRateOverTimeMultiplier = emission.rateOverTimeMultiplier;
                InitialRateOverDistanceMultiplier = emission.rateOverDistanceMultiplier;
                InitialStartSizeMultiplier = main.startSizeMultiplier;
                InitialStartSizeXMultiplier = main.startSizeXMultiplier;
                InitialStartSizeYMultiplier = main.startSizeYMultiplier;
                InitialStartSizeZMultiplier = main.startSizeZMultiplier;
                Uses3DStartSize = main.startSize3D;
                InitialStartSpeedMultiplier = main.startSpeedMultiplier;
                InitialStartColor = main.startColor;
                HasLight = lights.light;
                InitialLightIntensityMultiplier = HasLight ? lights.intensityMultiplier : 0.0f;
                EffectActive = effectActive;
            }
        }

        private sealed class TrailEffectState
        {
            internal readonly Color InitialStartColor;
            internal readonly Color InitialEndColor;
            internal readonly float InitialWidthMultiplier;
            internal readonly float InitialTime;
            internal readonly bool InitialEmitting;

            internal TrailEffectState(TrailRenderer trail)
            {
                InitialStartColor = trail.startColor;
                InitialEndColor = trail.endColor;
                InitialWidthMultiplier = trail.widthMultiplier;
                InitialTime = trail.time;
                InitialEmitting = trail.emitting;
            }
        }

        private sealed class LineEffectState
        {
            internal readonly Color InitialStartColor;
            internal readonly Color InitialEndColor;
            internal readonly float InitialWidthMultiplier;

            internal LineEffectState(LineRenderer line)
            {
                InitialStartColor = line.startColor;
                InitialEndColor = line.endColor;
                InitialWidthMultiplier = line.widthMultiplier;
            }
        }

        private void Awake()
        {
            FindParticleSystems();
            FindTrails();
            FindLines();
        }

        private void OnEnable()
        {
            ConfigFile.ParticleSettingsChanged += ApplyChanges;
            ApplyChanges(ConfigFile.ParticleDensity, ConfigFile.ParticleSize, ConfigFile.ParticleSpeed,
                ConfigFile.ParticleBrightness);
        }

        private void OnDisable()
        {
            ConfigFile.ParticleSettingsChanged -= ApplyChanges;
        }

        private void FindParticleSystems()
        {
            _particleSystems = new Dictionary<ParticleSystem, ParticleEffectState>();
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                if (emission.enabled)
                {
                    _effectActive = true;
                }
                _particleSystems.Add(particleSystem, new ParticleEffectState(particleSystem, emission.enabled));
            }
        }

        private void FindTrails()
        {
            _trails = new Dictionary<TrailRenderer, TrailEffectState>();
            TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>(true);
            foreach (TrailRenderer trail in trails)
            {
                _trails.Add(trail, new TrailEffectState(trail));
            }
        }

        private void FindLines()
        {
            _lines = new Dictionary<LineRenderer, LineEffectState>();
            LineRenderer[] lines = GetComponentsInChildren<LineRenderer>(true);
            foreach (LineRenderer line in lines)
            {
                _lines.Add(line, new LineEffectState(line));
            }
        }

        internal void SetEffectActive(bool effectActive)
        {
            _effectActive = effectActive;
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in _particleSystems)
            {
                particleSystemState.Value.EffectActive = effectActive;
                ConfigureEmissionState(particleSystemState.Key, particleSystemState.Value, ConfigFile.ParticleDensity);
            }
        }

        internal void ApplyEmissionState()
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in _particleSystems)
            {
                ConfigureEmissionState(particleSystemState.Key, particleSystemState.Value, ConfigFile.ParticleDensity);
            }
        }

        private void ApplyChanges(float densityModifier, float sizeModifier, float speedModifier,
            float brightnessModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in _particleSystems)
            {
                ConfigureParticleIntensity(particleSystemState.Key, particleSystemState.Value, densityModifier);
                ConfigureParticleSize(particleSystemState.Key, particleSystemState.Value, sizeModifier);
                ConfigureParticleSpeed(particleSystemState.Key, particleSystemState.Value, speedModifier);
                ConfigureParticleBrightness(particleSystemState.Key, particleSystemState.Value, brightnessModifier);
            }

            foreach (KeyValuePair<TrailRenderer, TrailEffectState> trailState in _trails)
            {
                ConfigureTrail(trailState.Key, trailState.Value, densityModifier);
            }

            foreach (KeyValuePair<LineRenderer, LineEffectState> lineState in _lines)
            {
                ConfigureLine(lineState.Key, lineState.Value, densityModifier);
            }
        }

        private static void ConfigureParticleIntensity(ParticleSystem particleSystem, ParticleEffectState state, float intensityModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = state.InitialRateOverTimeMultiplier * intensityModifier;
            emission.rateOverDistanceMultiplier = state.InitialRateOverDistanceMultiplier * intensityModifier;

            ConfigureEmissionState(particleSystem, state, intensityModifier);
        }

        private static void ConfigureEmissionState(ParticleSystem particleSystem, ParticleEffectState state, float intensityModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            bool enableEmission = state.EffectActive && state.InitialEmissionEnabled && intensityModifier > 0.0f;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = enableEmission;

            if (!enableEmission && intensityModifier <= 0.0f && particleSystem.particleCount > 0)
            {
                particleSystem.Clear(true);
            }
        }

        private static void ConfigureParticleSize(ParticleSystem particleSystem, ParticleEffectState state, float sizeModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            ScaleExistingParticleSizes(particleSystem, state.Uses3DStartSize, state.CurrentSizeModifier, sizeModifier);
            state.CurrentSizeModifier = sizeModifier;

            ParticleSystem.MainModule main = particleSystem.main;
            if (state.Uses3DStartSize)
            {
                main.startSizeXMultiplier = state.InitialStartSizeXMultiplier * sizeModifier;
                main.startSizeYMultiplier = state.InitialStartSizeYMultiplier * sizeModifier;
                main.startSizeZMultiplier = state.InitialStartSizeZMultiplier * sizeModifier;
            }
            else
            {
                main.startSizeMultiplier = state.InitialStartSizeMultiplier * sizeModifier;
            }
        }

        private static void ConfigureParticleSpeed(ParticleSystem particleSystem, ParticleEffectState state, float speedModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            ScaleExistingParticleSpeeds(particleSystem, state.CurrentSpeedModifier, speedModifier);
            state.CurrentSpeedModifier = speedModifier;

            ParticleSystem.MainModule main = particleSystem.main;
            main.startSpeedMultiplier = state.InitialStartSpeedMultiplier * speedModifier;
        }

        private static void ConfigureParticleBrightness(ParticleSystem particleSystem, ParticleEffectState state, float brightnessModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            ScaleExistingParticleBrightness(particleSystem, state.CurrentBrightnessModifier, brightnessModifier);
            state.CurrentBrightnessModifier = brightnessModifier;

            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = ScaleGradient(state.InitialStartColor, brightnessModifier);

            if (state.HasLight)
            {
                ParticleSystem.LightsModule lights = particleSystem.lights;
                lights.intensityMultiplier = state.InitialLightIntensityMultiplier * brightnessModifier;
            }
        }

        private static ParticleSystem.MinMaxGradient ScaleGradient(ParticleSystem.MinMaxGradient initialGradient, float brightnessModifier)
        {
            ParticleSystem.MinMaxGradient scaledGradient = initialGradient;
            switch (initialGradient.mode)
            {
                case ParticleSystemGradientMode.Color:
                    scaledGradient.color = ScaleColor(initialGradient.color, brightnessModifier);
                    break;
                case ParticleSystemGradientMode.TwoColors:
                    scaledGradient.colorMin = ScaleColor(initialGradient.colorMin, brightnessModifier);
                    scaledGradient.colorMax = ScaleColor(initialGradient.colorMax, brightnessModifier);
                    break;
                case ParticleSystemGradientMode.Gradient:
                case ParticleSystemGradientMode.RandomColor:
                    scaledGradient.gradient = ScaleGradient(initialGradient.gradient, brightnessModifier);
                    break;
                case ParticleSystemGradientMode.TwoGradients:
                    scaledGradient.gradientMin = ScaleGradient(initialGradient.gradientMin, brightnessModifier);
                    scaledGradient.gradientMax = ScaleGradient(initialGradient.gradientMax, brightnessModifier);
                    break;
            }

            return scaledGradient;
        }

        private static Gradient ScaleGradient(Gradient initialGradient, float brightnessModifier)
        {
            if (initialGradient is null)
            {
                return null;
            }

            GradientColorKey[] colorKeys = initialGradient.colorKeys;
            for (int index = 0; index < colorKeys.Length; index++)
            {
                GradientColorKey colorKey = colorKeys[index];
                Color color = colorKey.color;
                color.r *= brightnessModifier;
                color.g *= brightnessModifier;
                color.b *= brightnessModifier;
                colorKey.color = color;
                colorKeys[index] = colorKey;
            }

            GradientAlphaKey[] alphaKeys = initialGradient.alphaKeys;
            for (int index = 0; index < alphaKeys.Length; index++)
            {
                GradientAlphaKey alphaKey = alphaKeys[index];
                alphaKey.alpha *= brightnessModifier;
                alphaKeys[index] = alphaKey;
            }

            Gradient scaledGradient = new Gradient();
            scaledGradient.mode = initialGradient.mode;
            scaledGradient.SetKeys(colorKeys, alphaKeys);
            return scaledGradient;
        }

        private static Color ScaleColor(Color initialColor, float brightnessModifier)
        {
            return new Color(
                initialColor.r * brightnessModifier,
                initialColor.g * brightnessModifier,
                initialColor.b * brightnessModifier,
                initialColor.a * brightnessModifier);
        }

        private static void ScaleExistingParticleSizes(ParticleSystem particleSystem, bool uses3DStartSize, float previousModifier, float newModifier)
        {
            if (previousModifier == newModifier)
            {
                return;
            }

            if (particleSystem.particleCount == 0)
            {
                return;
            }

            if (previousModifier <= 0.0f)
            {
                particleSystem.Clear(true);
                return;
            }

            float scale = newModifier / previousModifier;
            ParticleSystem.Particle[] particles = GetParticles(particleSystem, out int particleCount);
            for (int index = 0; index < particleCount; index++)
            {
                ParticleSystem.Particle particle = particles[index];
                if (uses3DStartSize)
                {
                    particle.startSize3D *= scale;
                }
                else
                {
                    particle.startSize *= scale;
                }
                particles[index] = particle;
            }
            particleSystem.SetParticles(particles, particleCount);
        }

        private static void ScaleExistingParticleSpeeds(ParticleSystem particleSystem, float previousModifier, float newModifier)
        {
            if (previousModifier == newModifier)
            {
                return;
            }

            if (particleSystem.particleCount == 0)
            {
                return;
            }

            if (previousModifier <= 0.0f)
            {
                particleSystem.Clear(true);
                return;
            }

            float scale = newModifier / previousModifier;
            ParticleSystem.Particle[] particles = GetParticles(particleSystem, out int particleCount);
            for (int index = 0; index < particleCount; index++)
            {
                ParticleSystem.Particle particle = particles[index];
                particle.velocity *= scale;
                particles[index] = particle;
            }
            particleSystem.SetParticles(particles, particleCount);
        }

        private static void ScaleExistingParticleBrightness(ParticleSystem particleSystem, float previousModifier, float newModifier)
        {
            if (previousModifier == newModifier)
            {
                return;
            }

            if (particleSystem.particleCount == 0)
            {
                return;
            }

            if (previousModifier <= 0.0f)
            {
                particleSystem.Clear(true);
                return;
            }

            float scale = newModifier / previousModifier;
            ParticleSystem.Particle[] particles = GetParticles(particleSystem, out int particleCount);
            for (int index = 0; index < particleCount; index++)
            {
                ParticleSystem.Particle particle = particles[index];
                Color32 color = particle.startColor;
                color.r = ScaleColorChannel(color.r, scale);
                color.g = ScaleColorChannel(color.g, scale);
                color.b = ScaleColorChannel(color.b, scale);
                color.a = ScaleColorChannel(color.a, scale);
                particle.startColor = color;
                particles[index] = particle;
            }
            particleSystem.SetParticles(particles, particleCount);
        }

        private static ParticleSystem.Particle[] GetParticles(ParticleSystem particleSystem, out int particleCount)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            particleCount = particleSystem.GetParticles(particles);
            return particles;
        }

        private static byte ScaleColorChannel(byte channel, float scale)
        {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(channel * scale), 0, byte.MaxValue);
        }

        private static void ConfigureTrail(TrailRenderer trail, TrailEffectState state, float intensityModifier)
        {
            if (!trail)
            {
                return;
            }

            Color startColor = state.InitialStartColor;
            startColor.a *= intensityModifier;
            Color endColor = state.InitialEndColor;
            endColor.a *= intensityModifier;

            trail.startColor = startColor;
            trail.endColor = endColor;
            trail.widthMultiplier = state.InitialWidthMultiplier * intensityModifier;
            trail.time = state.InitialTime * intensityModifier;
            trail.emitting = state.InitialEmitting && intensityModifier > 0.0f;
            if (intensityModifier <= 0.0f)
            {
                trail.Clear();
            }
        }

        private static void ConfigureLine(LineRenderer line, LineEffectState state, float intensityModifier)
        {
            if (!line)
            {
                return;
            }

            Color startColor = state.InitialStartColor;
            startColor.a *= intensityModifier;
            Color endColor = state.InitialEndColor;
            endColor.a *= intensityModifier;

            line.startColor = startColor;
            line.endColor = endColor;
            line.widthMultiplier = state.InitialWidthMultiplier * intensityModifier;
        }

    }
}
