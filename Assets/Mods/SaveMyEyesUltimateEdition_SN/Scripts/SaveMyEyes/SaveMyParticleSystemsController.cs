using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Applies configured reductions to the supported particle, trail and line effects.
    /// </summary>
    internal class SaveMyParticleSystemsController : MonoBehaviour
    {
        private EffectUseCase useCase;
        private bool includeChildren = true;

        private Dictionary<ParticleSystem, ParticleEffectState> particleSystems;
        private Dictionary<TrailRenderer, TrailEffectState> trails;
        private Dictionary<LineRenderer, LineEffectState> lines;
        private bool effectActive;
        private bool lastFlashesEnabled = MiscSettings.flashes;
        private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[0];

        internal bool EffectActive => effectActive;
        internal EffectUseCase UseCase => useCase;

        private sealed class ParticleEffectState
        {
            internal bool GameEmissionEnabled;
            internal readonly ParticleSystem.Burst[] InitialBursts;
            internal readonly ParticleSystem.Burst[] ScaledBursts;
            internal float GameRateOverTimeMultiplier;
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

                GameEmissionEnabled = emission.enabled;
                InitialBursts = new ParticleSystem.Burst[emission.burstCount];
                emission.GetBursts(InitialBursts);
                ScaledBursts = new ParticleSystem.Burst[InitialBursts.Length];
                GameRateOverTimeMultiplier = emission.rateOverTimeMultiplier;
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
            if (particleSystems is null)
            {
                FindParticleSystems();
                FindTrails();
                FindLines();
            }
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            if (useCase != EffectUseCase.None)
            {
                ApplyCurrentSettings();
            }
        }

        private void OnDisable()
        {
            ConfigFile.SettingsChanged -= OnSettingsChanged;
        }

        internal void Initialize(EffectUseCase useCase, bool includeChildren = true)
        {
            if (particleSystems is null)
            {
                FindParticleSystems();
                FindTrails();
                FindLines();
            }
            // Some fire particles share a hierarchy with separately managed sparks.
            if (this.includeChildren != includeChildren)
            {
                this.includeChildren = includeChildren;
                FindParticleSystems();
                FindTrails();
                FindLines();
            }
            this.useCase = useCase;
            ApplyCurrentSettings();
        }

        private void OnSettingsChanged(EffectUseCase useCase)
        {
            if (this.useCase == useCase)
            {
                ApplyCurrentSettings();
            }
        }

        private void ApplyCurrentSettings()
        {
            ParticleSettings settings = ConfigFile.GetParticleSettings(useCase);
            ModDebugLog.LogDebug($"ParticleController.Apply: Applying {useCase} settings to {gameObject.name}");
            ApplyChanges(settings.Density, settings.Size, settings.Speed, settings.Brightness);
        }

        private void FindParticleSystems()
        {
            particleSystems = new Dictionary<ParticleSystem, ParticleEffectState>();
            effectActive = false;
            ParticleSystem[] foundParticleSystems = includeChildren
                ? GetComponentsInChildren<ParticleSystem>(true)
                : GetComponents<ParticleSystem>();
            foreach (ParticleSystem particleSystem in foundParticleSystems)
            {
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                if (emission.enabled)
                {
                    effectActive = true;
                }
                particleSystems.Add(particleSystem, new ParticleEffectState(particleSystem, emission.enabled));
            }
        }

        private void FindTrails()
        {
            trails = new Dictionary<TrailRenderer, TrailEffectState>();
            TrailRenderer[] foundTrails = includeChildren
                ? GetComponentsInChildren<TrailRenderer>(true)
                : GetComponents<TrailRenderer>();
            foreach (TrailRenderer trail in foundTrails)
            {
                trails.Add(trail, new TrailEffectState(trail));
            }
        }

        private void FindLines()
        {
            lines = new Dictionary<LineRenderer, LineEffectState>();
            LineRenderer[] foundLines = includeChildren
                ? GetComponentsInChildren<LineRenderer>(true)
                : GetComponents<LineRenderer>();
            foreach (LineRenderer line in foundLines)
            {
                lines.Add(line, new LineEffectState(line));
            }
        }

        internal void SetEffectActive(bool effectActive)
        {
            this.effectActive = effectActive;
            float density = ConfigFile.GetParticleSettings(useCase).Density;
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in particleSystems)
            {
                particleSystemState.Value.EffectActive = effectActive;
                ConfigureEmissionState(particleSystemState.Key, particleSystemState.Value, density);
            }
        }

        internal void ApplyEmissionState()
        {
            float density = ConfigFile.GetParticleSettings(useCase).Density;
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in particleSystems)
            {
                ConfigureEmissionState(particleSystemState.Key, particleSystemState.Value, density);
            }
        }

        internal void CaptureAndApplyEmissionRate(ParticleSystem particleSystem)
        {
            ParticleEffectState state = particleSystems[particleSystem];
            state.GameRateOverTimeMultiplier = particleSystem.emission.rateOverTimeMultiplier;
            float density = ConfigFile.GetParticleSettings(useCase).Density;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = state.GameRateOverTimeMultiplier * density;
            ConfigureEmissionState(particleSystem, state, density);
        }

        private void ApplyChanges(float densityModifier, float sizeModifier, float speedModifier,
            float brightnessModifier)
        {
            foreach (KeyValuePair<ParticleSystem, ParticleEffectState> particleSystemState in particleSystems)
            {
                AdjustExistingParticles(particleSystemState.Key, particleSystemState.Value, sizeModifier, speedModifier, brightnessModifier);
                ConfigureParticleIntensity(particleSystemState.Key, particleSystemState.Value, densityModifier);
                ConfigureParticleSize(particleSystemState.Key, particleSystemState.Value, sizeModifier);
                ConfigureParticleSpeed(particleSystemState.Key, particleSystemState.Value, speedModifier);
                ConfigureParticleBrightness(particleSystemState.Key, particleSystemState.Value, brightnessModifier);
            }

            foreach (KeyValuePair<TrailRenderer, TrailEffectState> trailState in trails)
            {
                ConfigureTrail(trailState.Key, trailState.Value, densityModifier);
            }

            foreach (KeyValuePair<LineRenderer, LineEffectState> lineState in lines)
            {
                ConfigureLine(lineState.Key, lineState.Value, densityModifier);
            }
        }

        private void ConfigureParticleIntensity(ParticleSystem particleSystem, ParticleEffectState state, float intensityModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = state.GameRateOverTimeMultiplier * intensityModifier;
            emission.rateOverDistanceMultiplier = state.InitialRateOverDistanceMultiplier * intensityModifier;

            for (int index = 0; index < state.InitialBursts.Length; index++)
            {
                ParticleSystem.Burst burst = state.InitialBursts[index];
                burst.count = ScaleBurstCount(burst.count, intensityModifier);
                state.ScaledBursts[index] = burst;
            }
            emission.SetBursts(state.ScaledBursts);
            ConfigureEmissionState(particleSystem, state, intensityModifier);
        }

        private void ConfigureEmissionState(ParticleSystem particleSystem, ParticleEffectState state, float intensityModifier)
        {
            if (!particleSystem)
            {
                return;
            }

            bool enableEmission = state.EffectActive && state.GameEmissionEnabled && intensityModifier > 0.0f && ToolFlashesAllowed;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            if (emission.enabled != enableEmission)
            {
                emission.enabled = enableEmission;
            }

            if (!enableEmission && (intensityModifier <= 0.0f || !ToolFlashesAllowed) && particleSystem.particleCount > 0)
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

        internal void BeginPlayback(bool enablesAllEmissions)
        {
            if (enablesAllEmissions)
            {
                // VFXLateTimeParticles.Play explicitly enables every child emission.
                foreach (ParticleEffectState state in particleSystems.Values)
                {
                    state.GameEmissionEnabled = true;
                }
            }
            SetEffectActive(true);
        }

        private bool ToolFlashesAllowed =>
            (useCase != EffectUseCase.LaserCutter && useCase != EffectUseCase.RepairTool) || MiscSettings.flashes;

        internal void ApplyFlashesSettingIfChanged()
        {
            if (lastFlashesEnabled == MiscSettings.flashes)
            {
                return;
            }
            lastFlashesEnabled = MiscSettings.flashes;
            ApplyEmissionState();
        }

        internal static ParticleSystem.MinMaxCurve ScaleBurstCount(ParticleSystem.MinMaxCurve count, float density)
        {
            if (density == 1.0f)
            {
                return count;
            }
            // Round constant counts down so small positive settings cannot retain a full burst.
            switch (count.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    count.constant = Mathf.Floor(count.constant * density);
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    count.constantMin = Mathf.Floor(count.constantMin * density);
                    count.constantMax = Mathf.Floor(count.constantMax * density);
                    break;
                default:
                    count.curveMultiplier *= density;
                    break;
            }
            return count;
        }

        private void AdjustExistingParticles(ParticleSystem particleSystem, ParticleEffectState state,
            float size, float speed, float brightness)
        {
            if (!particleSystem || particleSystem.particleCount == 0)
            {
                return;
            }
            bool sizeChanged = !Mathf.Approximately(size, state.CurrentSizeModifier);
            bool speedChanged = !Mathf.Approximately(speed, state.CurrentSpeedModifier);
            bool brightnessChanged = !Mathf.Approximately(brightness, state.CurrentBrightnessModifier);
            if (!sizeChanged && !speedChanged && !brightnessChanged)
            {
                return;
            }
            if (size > state.CurrentSizeModifier || speed > state.CurrentSpeedModifier || brightness > state.CurrentBrightnessModifier)
            {
                // Lost size/velocity and byte color precision cannot be recovered reliably.
                // Clearing does not play or restart a stopped effect.
                particleSystem.Clear(false);
                return;
            }
            int capacity = particleSystem.particleCount;
            if (particleBuffer.Length < capacity)
            {
                particleBuffer = new ParticleSystem.Particle[capacity];
            }
            int count = particleSystem.GetParticles(particleBuffer);
            float sizeScale = state.CurrentSizeModifier > 0.0f ? size / state.CurrentSizeModifier : 0.0f;
            float speedScale = state.CurrentSpeedModifier > 0.0f ? speed / state.CurrentSpeedModifier : 0.0f;
            float brightnessScale = state.CurrentBrightnessModifier > 0.0f ? brightness / state.CurrentBrightnessModifier : 0.0f;
            for (int index = 0; index < count; index++)
            {
                ParticleSystem.Particle particle = particleBuffer[index];
                if (sizeChanged)
                {
                    if (state.Uses3DStartSize)
                    {
                        particle.startSize3D *= sizeScale;
                    }
                    else
                    {
                        particle.startSize *= sizeScale;
                    }
                }
                if (speedChanged)
                {
                    particle.velocity *= speedScale;
                }
                if (brightnessChanged)
                {
                    Color32 color = particle.startColor;
                    color.r = ScaleColorChannel(color.r, brightnessScale);
                    color.g = ScaleColorChannel(color.g, brightnessScale);
                    color.b = ScaleColorChannel(color.b, brightnessScale);
                    color.a = ScaleColorChannel(color.a, brightnessScale);
                    particle.startColor = color;
                }
                particleBuffer[index] = particle;
            }
            particleSystem.SetParticles(particleBuffer, count);
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
