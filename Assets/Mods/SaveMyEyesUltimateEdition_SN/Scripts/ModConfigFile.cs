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
        internal event Action<EffectUseCase> SettingsChanged;

        [Slider("Laser Cutter: Light Intensity", Tooltip = "Adjusts light intensity for Laser Cutter. 0 removes the light; 1 restores original intensity.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterLightIntensity = 0.5f;

        [Slider("Laser Cutter: Material Emission", Tooltip = "Adjusts laser-cut door material glow for Laser Cutter. 0 removes the glow; 1 restores original glow.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterMaterialEmissionIntensity = 0.5f;

        [Slider("Laser Cutter: Particle Emission Density", Tooltip = "Adjusts particle density and trail/line visibility for Laser Cutter. 0 disables particles and hides trails/beams; 1 restores original density.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterParticleDensity = 0.5f;

        [Slider("Laser Cutter: Particle Size", Tooltip = "Adjusts initial particle size for Laser Cutter. 0 reduces particle size to zero; 1 restores original size.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterParticleSize = 0.5f;

        [Slider("Laser Cutter: Particle Speed", Tooltip = "Adjusts initial particle movement speed for Laser Cutter. 0 removes initial particle movement; 1 restores original speed.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterParticleSpeed = 0.5f;

        [Slider("Laser Cutter: Particle Brightness", Tooltip = "Adjusts particle colour/opacity and particle-light brightness for Laser Cutter. 0 makes particles transparent and removes their lighting; 1 restores original brightness.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnLaserCutterSettingsChanged))]
        public float LaserCutterParticleBrightness = 0.5f;

        [Slider("Repair Tool: Particle Emission Density", Tooltip = "Adjusts particle density and trail/line visibility for Repair Tool. 0 disables particles and hides trails/beams; 1 restores original density.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnRepairToolSettingsChanged))]
        public float RepairToolParticleDensity = 0.5f;

        [Slider("Repair Tool: Particle Size", Tooltip = "Adjusts initial particle size for Repair Tool. 0 reduces particle size to zero; 1 restores original size.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnRepairToolSettingsChanged))]
        public float RepairToolParticleSize = 0.5f;

        [Slider("Repair Tool: Particle Speed", Tooltip = "Adjusts initial particle movement speed for Repair Tool. 0 removes initial particle movement; 1 restores original speed.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnRepairToolSettingsChanged))]
        public float RepairToolParticleSpeed = 0.5f;

        [Slider("Repair Tool: Particle Brightness", Tooltip = "Adjusts particle colour/opacity and particle-light brightness for Repair Tool. 0 makes particles transparent and removes their lighting; 1 restores original brightness.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnRepairToolSettingsChanged))]
        public float RepairToolParticleBrightness = 0.5f;

        [Slider("Prawn Suit Drill: Light Intensity", Tooltip = "Adjusts light intensity for Prawn Suit Drill. 0 removes the light; 1 restores original intensity.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnPrawnDrillSettingsChanged))]
        public float PrawnDrillLightIntensity = 0.5f;

        [Slider("Prawn Suit Drill: Particle Emission Density", Tooltip = "Adjusts particle density and trail/line visibility for Prawn Suit Drill. 0 disables particles and hides trails/beams; 1 restores original density.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnPrawnDrillSettingsChanged))]
        public float PrawnDrillParticleDensity = 0.5f;

        [Slider("Prawn Suit Drill: Particle Size", Tooltip = "Adjusts initial particle size for Prawn Suit Drill. 0 reduces particle size to zero; 1 restores original size.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnPrawnDrillSettingsChanged))]
        public float PrawnDrillParticleSize = 0.5f;

        [Slider("Prawn Suit Drill: Particle Speed", Tooltip = "Adjusts initial particle movement speed for Prawn Suit Drill. 0 removes initial particle movement; 1 restores original speed.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnPrawnDrillSettingsChanged))]
        public float PrawnDrillParticleSpeed = 0.5f;

        [Slider("Prawn Suit Drill: Particle Brightness", Tooltip = "Adjusts particle colour/opacity and particle-light brightness for Prawn Suit Drill. 0 makes particles transparent and removes their lighting; 1 restores original brightness.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnPrawnDrillSettingsChanged))]
        public float PrawnDrillParticleBrightness = 0.5f;

        [Slider("Aurora Sparks: Light Intensity", Tooltip = "Adjusts light intensity for Aurora Sparks. 0 removes the light; 1 restores original intensity.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraSparksSettingsChanged))]
        public float AuroraSparksLightIntensity = 0.5f;

        [Slider("Aurora Sparks: Particle Emission Density", Tooltip = "Adjusts particle density and trail/line visibility for Aurora Sparks. 0 disables particles and hides trails/beams; 1 restores original density.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraSparksSettingsChanged))]
        public float AuroraSparksParticleDensity = 0.5f;

        [Slider("Aurora Sparks: Particle Size", Tooltip = "Adjusts initial particle size for Aurora Sparks. 0 reduces particle size to zero; 1 restores original size.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraSparksSettingsChanged))]
        public float AuroraSparksParticleSize = 0.5f;

        [Slider("Aurora Sparks: Particle Speed", Tooltip = "Adjusts initial particle movement speed for Aurora Sparks. 0 removes initial particle movement; 1 restores original speed.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraSparksSettingsChanged))]
        public float AuroraSparksParticleSpeed = 0.5f;

        [Slider("Aurora Sparks: Particle Brightness", Tooltip = "Adjusts particle colour/opacity and particle-light brightness for Aurora Sparks. 0 makes particles transparent and removes their lighting; 1 restores original brightness.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraSparksSettingsChanged))]
        public float AuroraSparksParticleBrightness = 0.5f;

        [Slider("Aurora Flames: Light Intensity", Tooltip = "Adjusts light intensity for Aurora Flames. 0 removes the light; 1 restores original intensity.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraFlamesSettingsChanged))]
        public float AuroraFlamesLightIntensity = 0.5f;

        [Slider("Aurora Flames: Particle Emission Density", Tooltip = "Adjusts particle density and trail/line visibility for Aurora Flames. 0 disables particles and hides trails/beams; 1 restores original density.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraFlamesSettingsChanged))]
        public float AuroraFlamesParticleDensity = 0.5f;

        [Slider("Aurora Flames: Particle Size", Tooltip = "Adjusts initial particle size for Aurora Flames. 0 reduces particle size to zero; 1 restores original size.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraFlamesSettingsChanged))]
        public float AuroraFlamesParticleSize = 0.5f;

        [Slider("Aurora Flames: Particle Speed", Tooltip = "Adjusts initial particle movement speed for Aurora Flames. 0 removes initial particle movement; 1 restores original speed.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraFlamesSettingsChanged))]
        public float AuroraFlamesParticleSpeed = 0.5f;

        [Slider("Aurora Flames: Particle Brightness", Tooltip = "Adjusts particle colour/opacity and particle-light brightness for Aurora Flames. 0 makes particles transparent and removes their lighting; 1 restores original brightness.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnAuroraFlamesSettingsChanged))]
        public float AuroraFlamesParticleBrightness = 0.5f;

        [Toggle("Water Filtration: Disable Beams", Tooltip = "Disables the water filtration system beams."), OnChange(nameof(OnWaterFiltrationSettingsChanged))]
        public bool DisableWaterFiltrationBeams = true;

        [Slider("Flare: Light Intensity", Tooltip = "Adjusts flare light intensity. 0 removes the light; 1 restores original intensity.",
            Step = 0.1f, Format = "{0:F1}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.5f), OnChange(nameof(OnFlareSettingsChanged))]
        public float FlareIntensity = 0.5f;

        /// <summary>
        /// Enable detailed logging
        /// </summary>
        [Toggle("Detailed logging", Tooltip="Use this to produce a detailed log when reporting bugs. Logs are written to %LOCALAPPDATA%low\\Unknown Worlds\\Subnautica\\Player.log"), OnChange(nameof(OnLoggingChanged))]
        public bool DetailedLogging = false;

        internal float GetLightIntensity(EffectUseCase useCase)
        {
            switch (useCase)
            {
                case EffectUseCase.LaserCutter:
                    return LaserCutterLightIntensity;
                case EffectUseCase.PrawnDrill:
                    return PrawnDrillLightIntensity;
                case EffectUseCase.AuroraSparks:
                    return AuroraSparksLightIntensity;
                case EffectUseCase.AuroraFlames:
                    return AuroraFlamesLightIntensity;
                case EffectUseCase.Flare:
                    return FlareIntensity;
                default:
                    throw new ArgumentOutOfRangeException(nameof(useCase), useCase, "No light settings for this use case.");
            }
        }

        internal float GetMaterialEmissionIntensity(EffectUseCase useCase)
        {
            switch (useCase)
            {
                case EffectUseCase.LaserCutter:
                    return LaserCutterMaterialEmissionIntensity;
                default:
                    throw new ArgumentOutOfRangeException(nameof(useCase), useCase, "No material settings for this use case.");
            }
        }

        internal ParticleSettings GetParticleSettings(EffectUseCase useCase)
        {
            switch (useCase)
            {
                case EffectUseCase.LaserCutter:
                    return new ParticleSettings(LaserCutterParticleDensity, LaserCutterParticleSize,
                        LaserCutterParticleSpeed, LaserCutterParticleBrightness);
                case EffectUseCase.RepairTool:
                    return new ParticleSettings(RepairToolParticleDensity, RepairToolParticleSize,
                        RepairToolParticleSpeed, RepairToolParticleBrightness);
                case EffectUseCase.PrawnDrill:
                    return new ParticleSettings(PrawnDrillParticleDensity, PrawnDrillParticleSize,
                        PrawnDrillParticleSpeed, PrawnDrillParticleBrightness);
                case EffectUseCase.AuroraSparks:
                    return new ParticleSettings(AuroraSparksParticleDensity, AuroraSparksParticleSize,
                        AuroraSparksParticleSpeed, AuroraSparksParticleBrightness);
                case EffectUseCase.AuroraFlames:
                    return new ParticleSettings(AuroraFlamesParticleDensity, AuroraFlamesParticleSize,
                        AuroraFlamesParticleSpeed, AuroraFlamesParticleBrightness);
                default:
                    throw new ArgumentOutOfRangeException(nameof(useCase), useCase, "No particle settings for this use case.");
            }
        }

        private void OnLaserCutterSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.LaserCutter);
        }

        private void OnRepairToolSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.RepairTool);
        }

        private void OnPrawnDrillSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.PrawnDrill);
        }

        private void OnAuroraSparksSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.AuroraSparks);
        }

        private void OnAuroraFlamesSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.AuroraFlames);
        }

        private void OnWaterFiltrationSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.WaterFiltration);
        }

        private void OnFlareSettingsChanged()
        {
            SettingsChanged?.Invoke(EffectUseCase.Flare);
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
