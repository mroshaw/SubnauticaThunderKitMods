using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Maintains configurable properties for contributed particle, trail and line effects
    /// </summary>
    public static class SaveMyEyesFromParticleFx
    {
        private static readonly Dictionary<ParticleSystem, ParticleEffectState> ParticleSystems = new Dictionary<ParticleSystem, ParticleEffectState>();
        private static readonly Dictionary<TrailRenderer, TrailEffectState> Trails = new Dictionary<TrailRenderer, TrailEffectState>();
        private static readonly Dictionary<LineRenderer, LineEffectState> Lines = new Dictionary<LineRenderer, LineEffectState>();

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

        /// <summary>
        /// Stores a contributed particle system and its initial visual properties
        /// </summary>
        internal static void Register(ParticleSystem particleSystem, bool effectActive)
        {
            if (!particleSystem)
            {
                return;
            }

            ParticleEffectState state;
            if (!ParticleSystems.TryGetValue(particleSystem, out state))
            {
                state = new ParticleEffectState(particleSystem, effectActive);
                ParticleSystems.Add(particleSystem, state);
            }
            else
            {
                state.EffectActive = effectActive;
            }

            ConfigureParticleIntensity(particleSystem, state, ConfigFile.ParticleFXIntensity);
            ConfigureParticleSize(particleSystem, state, ConfigFile.ParticleFXSize);
            ConfigureParticleSpeed(particleSystem, state, ConfigFile.ParticleFXSpeed);
            ConfigureParticleBrightness(particleSystem, state, ConfigFile.ParticleFXBrightness);
            ConfigureEmissionState(particleSystem, state, ConfigFile.ParticleFXIntensity);
        }

        /// <summary>
        /// Records whether the game currently wants a contributed particle effect active
        /// </summary>
        internal static void ConfigureFromGameState(ParticleSystem particleSystem, bool effectActive)
        {
            ParticleEffectState state;
            if (!particleSystem || !ParticleSystems.TryGetValue(particleSystem, out state))
            {
                return;
            }

            state.EffectActive = effectActive;
            ConfigureEmissionState(particleSystem, state, ConfigFile.ParticleFXIntensity);
        }

        /// <summary>
        /// Removes a particle system from the registry
        /// </summary>
        internal static void Unregister(ParticleSystem particleSystem)
        {
            if (!ReferenceEquals(particleSystem, null))
            {
                ParticleSystems.Remove(particleSystem);
            }
        }

        /// <summary>
        /// Stores a contributed trail and its initial visual properties
        /// </summary>
        internal static void Register(TrailRenderer trail)
        {
            if (!trail)
            {
                return;
            }

            TrailEffectState state;
            if (!Trails.TryGetValue(trail, out state))
            {
                state = new TrailEffectState(trail);
                Trails.Add(trail, state);
            }

            ConfigureTrail(trail, state, ConfigFile.ParticleFXIntensity);
        }

        /// <summary>
        /// Removes a trail from the registry
        /// </summary>
        internal static void Unregister(TrailRenderer trail)
        {
            if (!ReferenceEquals(trail, null))
            {
                Trails.Remove(trail);
            }
        }

        /// <summary>
        /// Stores a contributed line and its initial visual properties
        /// </summary>
        internal static void Register(LineRenderer line)
        {
            if (!line)
            {
                return;
            }

            LineEffectState state;
            if (!Lines.TryGetValue(line, out state))
            {
                state = new LineEffectState(line);
                Lines.Add(line, state);
            }

            ConfigureLine(line, state, ConfigFile.ParticleFXIntensity);
        }

        /// <summary>
        /// Removes a line from the registry
        /// </summary>
        internal static void Unregister(LineRenderer line)
        {
            if (!ReferenceEquals(line, null))
            {
                Lines.Remove(line);
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

        /// <summary>
        /// Applies the configured intensity to all contributed particle systems
        /// </summary>
        public static void ConfigureAllParticleSystems(float intensityModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in ParticleSystems)
            {
                ConfigureParticleIntensity(particleSystemState.Key, particleSystemState.Value, intensityModifier);
            }
        }

        /// <summary>
        /// Applies the configured size to all contributed particle systems
        /// </summary>
        public static void ConfigureAllParticleSizes(float sizeModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in ParticleSystems)
            {
                ConfigureParticleSize(particleSystemState.Key, particleSystemState.Value, sizeModifier);
            }
        }

        /// <summary>
        /// Applies the configured speed to all contributed particle systems
        /// </summary>
        public static void ConfigureAllParticleSpeeds(float speedModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in ParticleSystems)
            {
                ConfigureParticleSpeed(particleSystemState.Key, particleSystemState.Value, speedModifier);
            }
        }

        /// <summary>
        /// Applies the configured brightness to all contributed particle systems
        /// </summary>
        public static void ConfigureAllParticleBrightness(float brightnessModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in ParticleSystems)
            {
                ConfigureParticleBrightness(particleSystemState.Key, particleSystemState.Value, brightnessModifier);
            }
        }

        /// <summary>
        /// Applies the configured intensity to all contributed trails
        /// </summary>
        public static void ConfigureAllTrails(float intensityModifier)
        {
            foreach (KeyValuePair<TrailRenderer, TrailEffectState> trailState in Trails)
            {
                ConfigureTrail(trailState.Key, trailState.Value, intensityModifier);
            }
        }

        /// <summary>
        /// Applies the configured intensity to all contributed lines
        /// </summary>
        public static void ConfigureAllLines(float intensityModifier)
        {
            foreach (KeyValuePair<LineRenderer, LineEffectState> lineState in Lines)
            {
                ConfigureLine(lineState.Key, lineState.Value, intensityModifier);
            }
        }
    }
}
