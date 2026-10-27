using System.Collections.Generic;
using DaftAppleGames.VehicleEnhancements_SN.Enhancements;
using Nautilus.Json;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal enum HudLocation
    {
        Left,
        Center,
        Right
    }

    internal enum ReversingVoice
    {
        ThisVehicleIsReversing,
        ThisPrawnSuitIsReversing,
        None,
        ThisSeaMothIsReversing,
        ThisCyclopsIsReversing
    }

    /// <summary>
    /// Options for enhanced vehicle displays and reversing sounds.
    /// </summary>
    internal class ModConfigFile : ConfigFile
    {
        internal const float MinimumHudAdjust = -700.0f;
        internal const float MaximumHudAdjust = 700.0f;
        internal const float MinimumHudScale = 0.1f;
        internal const float MaximumHudScale = 4.0f;

        public bool EnableSeamothSpeedometer = true;
        public bool EnableSeamothHsi = true;
        public bool EnableSeamothTime = true;
        public ReversingVoice SeamothReversingVoice = ReversingVoice.ThisSeaMothIsReversing;
        public bool EnableSeamothReversingBeeps = true;

        public HudLocation SeamothSpeedometerLocation { get; set; } = HudLocation.Right;
        public float SeamothSpeedometerHorizontalAdjust { get; set; } = 0.0f;
        public float SeamothSpeedometerVerticalAdjust { get; set; } = 110.0f;
        public float SeamothSpeedometerUiScale { get; set; } = 1.0f;

        public HudLocation SeamothHsiLocation { get; set; } = HudLocation.Right;
        public float SeamothHsiHorizontalAdjust { get; set; } = -290.0f;
        public float SeamothHsiVerticalAdjust { get; set; } = 110.0f;
        public float SeamothHsiUiScale { get; set; } = 1.0f;

        public HudLocation SeamothTimeLocation { get; set; } = HudLocation.Left;
        public float SeamothTimeHorizontalAdjust { get; set; } = -250.0f;
        public float SeamothTimeVerticalAdjust { get; set; } = 78.7f;
        public float SeamothTimeUiScale { get; set; } = 1.0f;

        public bool EnablePrawnSpeedometer = true;
        public bool EnablePrawnHsi = true;
        public bool EnablePrawnTime = true;
        public ReversingVoice PrawnReversingVoice = ReversingVoice.ThisPrawnSuitIsReversing;
        public bool EnablePrawnReversingBeeps = true;

        public HudLocation PrawnSpeedometerLocation { get; set; } = HudLocation.Right;
        public float PrawnSpeedometerHorizontalAdjust { get; set; } = 0.0f;
        public float PrawnSpeedometerVerticalAdjust { get; set; } = 135.0f;
        public float PrawnSpeedometerUiScale { get; set; } = 1.0f;

        public HudLocation PrawnHsiLocation { get; set; } = HudLocation.Right;
        public float PrawnHsiHorizontalAdjust { get; set; } = -290.0f;
        public float PrawnHsiVerticalAdjust { get; set; } = 110.0f;
        public float PrawnHsiUiScale { get; set; } = 1.0f;

        public HudLocation PrawnTimeLocation { get; set; } = HudLocation.Left;
        public float PrawnTimeHorizontalAdjust { get; set; } = -250.0f;
        public float PrawnTimeVerticalAdjust { get; set; } = 78.7f;
        public float PrawnTimeUiScale { get; set; } = 1.0f;

        public bool EnableCyclopsSpeedometer = true;
        public bool EnableCyclopsHsi = true;
        public bool EnableCyclopsTime = true;
        public ReversingVoice CyclopsReversingVoice = ReversingVoice.ThisCyclopsIsReversing;
        public bool EnableCyclopsReversingBeeps = true;

        public HudLocation CyclopsSpeedometerLocation { get; set; } = HudLocation.Left;
        public float CyclopsSpeedometerHorizontalAdjust { get; set; } = 390.0f;
        public float CyclopsSpeedometerVerticalAdjust { get; set; } = 0.0f;
        public float CyclopsSpeedometerUiScale { get; set; } = 1.3f;

        public HudLocation CyclopsHsiLocation { get; set; } = HudLocation.Left;
        public float CyclopsHsiHorizontalAdjust { get; set; } = 650.0f;
        public float CyclopsHsiVerticalAdjust { get; set; } = 110.0f;
        public float CyclopsHsiUiScale { get; set; } = 0.5f;

        public HudLocation CyclopsTimeLocation { get; set; } = HudLocation.Left;
        public float CyclopsTimeHorizontalAdjust { get; set; } = -85.0f;
        public float CyclopsTimeVerticalAdjust { get; set; } = -385.0f;
        public float CyclopsTimeUiScale { get; set; } = 1.0f;

        public float ReversingAudioVolume = 0.1f;

        public Dictionary<string, CustomVehicleSettings> CustomVehicleProfiles { get; set; } =
            new Dictionary<string, CustomVehicleSettings>();

        private static readonly CustomVehicleSettings defaultCustomVehicleSettings = new CustomVehicleSettings();
        private CustomVehicleSettings currentCustomVehicleSettings;

        internal CustomVehicleSettings CurrentCustomVehicleSettings => currentCustomVehicleSettings ?? defaultCustomVehicleSettings;

        internal void SetCustomVehicleType(string identifier)
        {
            currentCustomVehicleSettings = string.IsNullOrEmpty(identifier) ? null : GetOrCreateCustomVehicleSettings(identifier);
        }

        internal CustomVehicleSettings GetOrCreateCustomVehicleSettings(string identifier)
        {
            if (!CustomVehicleProfiles.TryGetValue(identifier, out CustomVehicleSettings settings))
            {
                settings = new CustomVehicleSettings();
                CustomVehicleProfiles.Add(identifier, settings);
            }

            return settings;
        }

        internal ReversingVoice GetReversingVoice(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return PrawnReversingVoice;
                case EnhancedVehicle.Cyclops: return CyclopsReversingVoice;
                case EnhancedVehicle.CustomVehicle: return CurrentCustomVehicleSettings.ReversingVoice;
                default: return SeamothReversingVoice;
            }
        }

        internal bool AreReversingBeepsEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnReversingBeeps;
                case EnhancedVehicle.Cyclops: return EnableCyclopsReversingBeeps;
                case EnhancedVehicle.CustomVehicle: return CurrentCustomVehicleSettings.EnableReversingBeeps;
                default: return EnableSeamothReversingBeeps;
            }
        }

        internal bool IsSpeedometerEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnSpeedometer;
                case EnhancedVehicle.Cyclops: return EnableCyclopsSpeedometer;
                case EnhancedVehicle.CustomVehicle: return CurrentCustomVehicleSettings.EnableSpeedometer;
                default: return EnableSeamothSpeedometer;
            }
        }

        internal bool IsHsiEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnHsi;
                case EnhancedVehicle.Cyclops: return EnableCyclopsHsi;
                case EnhancedVehicle.CustomVehicle: return CurrentCustomVehicleSettings.EnableHsi;
                default: return EnableSeamothHsi;
            }
        }

        internal bool IsTimeAndWeatherEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnTime;
                case EnhancedVehicle.Cyclops: return EnableCyclopsTime;
                case EnhancedVehicle.CustomVehicle: return CurrentCustomVehicleSettings.EnableTime;
                default: return EnableSeamothTime;
            }
        }


        public bool DetailedLogging = false;
    }
}
