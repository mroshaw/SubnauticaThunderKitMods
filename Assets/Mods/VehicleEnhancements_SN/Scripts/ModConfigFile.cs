using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

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
    [Menu("Vehicle Enhancements SN")]
    internal class ModConfigFile : ConfigFile
    {
        [Toggle("Seamoth: Speedometer", Order = 10)] public bool EnableSeamothSpeedometer = true;
        [Toggle("Seamoth: HSI", Order = 11)] public bool EnableSeamothHsi = true;
        [Toggle("Seamoth: Time", Order = 12)] public bool EnableSeamothTime = true;
        [Choice("Seamoth: Reversing Voice", "This vehicle is reversing", "This Prawn Suit is reversing", "None",
            "This Seamoth is reversing", "This Cyclops is reversing", Order = 13)]
        public ReversingVoice SeamothReversingVoice = ReversingVoice.ThisSeaMothIsReversing;
        [Toggle("Seamoth: Reversing Beeps", Order = 14)] public bool EnableSeamothReversingBeeps = true;

        [Toggle("Prawn Suit: Speedometer", Order = 20)] public bool EnablePrawnSpeedometer = true;
        [Toggle("Prawn Suit: HSI", Order = 21)] public bool EnablePrawnHsi = true;
        [Toggle("Prawn Suit: Time", Order = 22)] public bool EnablePrawnTime = true;
        [Choice("Prawn Suit: Reversing Voice", "This vehicle is reversing", "This Prawn Suit is reversing", "None",
            "This Seamoth is reversing", "This Cyclops is reversing", Order = 23)]
        public ReversingVoice PrawnReversingVoice = ReversingVoice.ThisPrawnSuitIsReversing;
        [Toggle("Prawn Suit: Reversing Beeps", Order = 24)] public bool EnablePrawnReversingBeeps = true;

        [Toggle("Cyclops: Speedometer", Order = 30)] public bool EnableCyclopsSpeedometer = true;
        [Toggle("Cyclops: HSI", Order = 31)] public bool EnableCyclopsHsi = true;
        [Toggle("Cyclops: Time", Order = 32)] public bool EnableCyclopsTime = true;
        [Choice("Cyclops: Reversing Voice", "This vehicle is reversing", "This Prawn Suit is reversing", "None",
            "This Seamoth is reversing", "This Cyclops is reversing", Order = 33)]
        public ReversingVoice CyclopsReversingVoice = ReversingVoice.ThisCyclopsIsReversing;
        [Toggle("Cyclops: Reversing Beeps", Order = 34)] public bool EnableCyclopsReversingBeeps = true;

        [Slider("Reversing Audio Volume", Order = 40, Step = 0.05f, Format = "{0:F2}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.1f)]
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

        [Toggle("Detailed logging", Order = 50), OnChange(nameof(OnLoggingChanged))]
        public bool DetailedLogging = false;

        private void OnLoggingChanged(ToggleChangedEventArgs eventArgs)
        {
            ModDebugLog.SetDetailedLoggingState(eventArgs.Value);
        }
    }
}
