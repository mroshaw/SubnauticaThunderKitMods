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
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleFxIntensityChanged))]
        public float  ParticleFXIntensity = 0.5f;

        [Slider("Particle Size", Tooltip="Adjust the initial size of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleFxSizeChanged))]
        public float ParticleFXSize = 0.5f;

        [Slider("Particle Speed", Tooltip="Adjust the initial movement speed of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleFxSpeedChanged))]
        public float ParticleFXSpeed = 0.5f;

        [Slider("Particle Brightness", Tooltip="Adjust the colour and opacity brightness of particles in supported effects.",
             Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnParticleFxBrightnessChanged))]
        public float ParticleFXBrightness = 0.5f;
        
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
            SaveMyEyesFromFlares.ConfigureAllFlares(intensityModifier: eventArgs.Value);
        }

        private void OnToolLightIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromToolLights.ConfigureAllToolLights(intensityMultiplier: eventArgs.Value);
        }

        private void OnMaterialEmissionIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromEmitterMaterials.ConfigureAllEmitterMaterials(intensityMultiplier: eventArgs.Value);
        }
        
        private void OnParticleFxIntensityChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromParticleFx.ConfigureAllParticleSystems(intensityModifier: eventArgs.Value);
            SaveMyEyesFromParticleFx.ConfigureAllTrails(intensityModifier: eventArgs.Value);
            SaveMyEyesFromParticleFx.ConfigureAllLines(intensityModifier: eventArgs.Value);
        }

        private void OnParticleFxSizeChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromParticleFx.ConfigureAllParticleSizes(sizeModifier: eventArgs.Value);
        }

        private void OnParticleFxSpeedChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromParticleFx.ConfigureAllParticleSpeeds(speedModifier: eventArgs.Value);
        }

        private void OnParticleFxBrightnessChanged(SliderChangedEventArgs eventArgs)
        {
            SaveMyEyesFromParticleFx.ConfigureAllParticleBrightness(brightnessModifier: eventArgs.Value);
        }

        private void OnWaterFiltrationBeamToggleChanged(ToggleChangedEventArgs eventArgs)
        {
            SaveMyEyesFromWaterFiltrationBeams.ConfigureAllFiltrationMachines(disableBeams: eventArgs.Value);
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
