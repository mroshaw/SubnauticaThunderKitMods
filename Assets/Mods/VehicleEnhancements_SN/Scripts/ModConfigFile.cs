using DaftAppleGames.VehicleEnhancements_SN.Enhancements;
using Nautilus.Json;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal enum ReversingVoice
    {
        ThisVehicleIsReversing,
        ThisPrawnSuitIsReversing,
        None,
        // Append new voices to preserve values already stored in config files.
        ThisSeaMothIsReversing,
        ThisCyclopsIsReversing
    }

    /// <summary>
    /// Options for enhanced vehicle displays and reversing sounds.
    /// </summary>
    internal class ModConfigFile : ConfigFile
    {
        internal const float MinimumHudAdjust = -200.0f;
        internal const float MaximumHudAdjust = 200.0f;
        internal const float MinimumHudScale = 0.1f;
        internal const float MaximumHudScale = 3.0f;

        public bool EnableSeamothSpeedometer = true;
        public bool EnableSeamothHsi = true;
        public bool EnableSeamothTime = true;
        public ReversingVoice SeamothReversingVoice = ReversingVoice.ThisSeaMothIsReversing;
        public bool EnableSeamothReversingBeeps = true;

        public float SeamothSpeedometerHorizontalAdjust { get; set; } = 0.0f;
        public float SeamothSpeedometerVerticalAdjust { get; set; } = 0.0f;
        public float SeamothSpeedometerUiScale { get; set; } = 1.0f;

        public float SeamothHsiHorizontalAdjust { get; set; } = 0.0f;
        public float SeamothHsiVerticalAdjust { get; set; } = 0.0f;
        public float SeamothHsiUiScale { get; set; } = 1.0f;

        public float SeamothTimeHorizontalAdjust { get; set; } = 0.0f;
        public float SeamothTimeVerticalAdjust { get; set; } = 0.0f;
        public float SeamothTimeUiScale { get; set; } = 1.0f;

        public bool EnablePrawnSpeedometer = true;
        public bool EnablePrawnHsi = true;
        public bool EnablePrawnTime = true;
        public ReversingVoice PrawnReversingVoice = ReversingVoice.ThisPrawnSuitIsReversing;
        public bool EnablePrawnReversingBeeps = true;

        public float PrawnSpeedometerHorizontalAdjust { get; set; } = 0.0f;
        public float PrawnSpeedometerVerticalAdjust { get; set; } = 0.0f;
        public float PrawnSpeedometerUiScale { get; set; } = 1.0f;

        public float PrawnHsiHorizontalAdjust { get; set; } = 0.0f;
        public float PrawnHsiVerticalAdjust { get; set; } = 0.0f;
        public float PrawnHsiUiScale { get; set; } = 1.0f;

        public float PrawnTimeHorizontalAdjust { get; set; } = 0.0f;
        public float PrawnTimeVerticalAdjust { get; set; } = 0.0f;
        public float PrawnTimeUiScale { get; set; } = 1.0f;

        public bool EnableCyclopsSpeedometer = true;
        public bool EnableCyclopsHsi = true;
        public bool EnableCyclopsTime = true;
        public ReversingVoice CyclopsReversingVoice = ReversingVoice.ThisCyclopsIsReversing;
        public bool EnableCyclopsReversingBeeps = true;

        public float CyclopsSpeedometerHorizontalAdjust { get; set; } = 0.0f;
        public float CyclopsSpeedometerVerticalAdjust { get; set; } = 0.0f;
        public float CyclopsSpeedometerUiScale { get; set; } = 1.0f;

        public float CyclopsHsiHorizontalAdjust { get; set; } = 0.0f;
        public float CyclopsHsiVerticalAdjust { get; set; } = 0.0f;
        public float CyclopsHsiUiScale { get; set; } = 1.0f;

        public float CyclopsTimeHorizontalAdjust { get; set; } = 0.0f;
        public float CyclopsTimeVerticalAdjust { get; set; } = 0.0f;
        public float CyclopsTimeUiScale { get; set; } = 1.0f;

        public float ReversingAudioVolume = 0.1f;

        internal ReversingVoice GetReversingVoice(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return PrawnReversingVoice;
                case EnhancedVehicle.Cyclops: return CyclopsReversingVoice;
                default: return SeamothReversingVoice;
            }
        }

        internal bool AreReversingBeepsEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnReversingBeeps;
                case EnhancedVehicle.Cyclops: return EnableCyclopsReversingBeeps;
                default: return EnableSeamothReversingBeeps;
            }
        }

        internal bool IsSpeedometerEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnSpeedometer;
                case EnhancedVehicle.Cyclops: return EnableCyclopsSpeedometer;
                default: return EnableSeamothSpeedometer;
            }
        }

        internal bool IsHsiEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnHsi;
                case EnhancedVehicle.Cyclops: return EnableCyclopsHsi;
                default: return EnableSeamothHsi;
            }
        }

        internal bool IsTimeAndWeatherEnabled(EnhancedVehicle vehicle)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit: return EnablePrawnTime;
                case EnhancedVehicle.Cyclops: return EnableCyclopsTime;
                default: return EnableSeamothTime;
            }
        }

        public bool DetailedLogging = false;
    }
}
