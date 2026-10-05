using System;
using DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN
{
    /// <summary>
    /// Nautilus mod config class
    /// </summary>
    [Menu("Save My Eyes: Ultimate Edition")]
    public class ModConfigFile : ConfigFile
    {
        // Internal change Actions to which our new components can subscribe for "real time" notification of
        // config changes
        internal event Action<float> ToolLightSettingsChanged;
        internal event Action<float> FlareSettingsChanged;
        internal event Action<float> MaterialSettingsChanged;
        internal event Action<float, float, float, float> ParticleSettingsChanged;
        internal event Action<bool> WaterFiltrationBeamSettingsChanged;
        
        [Slider("Light Intensity", Tooltip="Adjusts the intensity of lights for supported effects such as the Laser Cutter. Set to 0 to disable contributed lights.", 
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnToolLightIntensityChanged))]
        public float ToolLightIntensity = 0.5f;

        [Slider("Material Emission", Tooltip="Adjust the light emission from material glow for supported effects such as laser-cut doors. Set to 0 to disable contributed emission.", 
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnMaterialEmissionIntensityChanged))]
        public float MaterialEmissionIntensity = 0.5f;
        
        [Slider("Flare Intensity", Tooltip="Adjust brightness of flare effects. Set to 0 to disable all flares.", 
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnFlareIntensityChanged))]
        public float FlareIntensity = 0.5f;
        
        [Slider("Particle Emission Density", Tooltip="Adjust the number and density of particle, trail and line effects such as Laser Cutter effects and laser-cut door sparks. Set to 0 to disable them.", 
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleDensityChanged))]
        public float  ParticleDensity = 0.5f;

        [Slider("Particle Size", Tooltip="Adjust the initial size of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleSizeChanged))]
        public float ParticleSize = 0.5f;

        [Slider("Particle Speed", Tooltip="Adjust the initial movement speed of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleSpeedChanged))]
        public float ParticleSpeed = 0.5f;

        [Slider("Particle Brightness", Tooltip="Adjust the colour and opacity brightness of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleBrightnessChanged))]
        public float ParticleBrightness = 0.5f;
        
        [Toggle("Disable Water Filtration Beams", Tooltip="Toggle the water filtration system beams on or off."), OnChange(nameof(OnWaterFiltrationBeamToggleChanged))]
        public bool DisableWaterFiltrationBeams = true;
        
        /// <summary>
        /// Enable detailed logging
        /// </summary>
        [Toggle("Detailed logging", Tooltip="Use this to produce a detailed log when reporting bugs. Logs are written to %LOCALAPPDATA%low\\Unknown Worlds\\Subnautica\\Player.log"), OnChange(nameof(OnLoggingChanged))]
        public bool DetailedLogging = false;

        /// <summary>
        /// Call static methods when config values are changed
        /// </summary>
        private void OnFlareIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            FlareSettingsChanged?.Invoke(eventArgs.Value);
        }

        private void OnToolLightIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            ToolLightSettingsChanged?.Invoke(eventArgs.Value);
        }

        private void OnMaterialEmissionIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            MaterialSettingsChanged?.Invoke(eventArgs.Value);
        }
        
        private void OnParticleDensityChanged(SliderChangedEventArgs eventArgs)
        {
            ParticleSettingsChanged?.Invoke(eventArgs.Value, ParticleSize, ParticleSpeed, ParticleBrightness);
        }

        private void OnParticleSizeChanged(SliderChangedEventArgs eventArgs)
        {
            ParticleSettingsChanged?.Invoke(ParticleDensity, eventArgs.Value, ParticleSpeed, ParticleBrightness);
        }

        private void OnParticleSpeedChanged(SliderChangedEventArgs eventArgs)
        {
            ParticleSettingsChanged?.Invoke(ParticleDensity, ParticleSize, eventArgs.Value, ParticleBrightness);
        }

        private void OnParticleBrightnessChanged(SliderChangedEventArgs eventArgs)
        {
            ParticleSettingsChanged?.Invoke(ParticleDensity, ParticleSize, ParticleSpeed, eventArgs.Value);
        }

        private void OnWaterFiltrationBeamToggleChanged(ToggleChangedEventArgs eventArgs)
        {
            WaterFiltrationBeamSettingsChanged?.Invoke(eventArgs.Value);
        }
        
        /// <summary>
        /// Handle toggling of detailed logging
        /// </summary>
        private void OnLoggingChanged(ToggleChangedEventArgs eventArgs)
        {
            ModDebugLog.SetDetailedLoggingState(eventArgs.Value);
        }
    }
}
