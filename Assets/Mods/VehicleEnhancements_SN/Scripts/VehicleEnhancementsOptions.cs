using System;
using System.Collections.Generic;
using Nautilus.Options;
using TMPro;
using UnityEngine;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal class VehicleEnhancementsOptions : ModOptions
    {
        private const string MenuName = "Vehicle Enhancements SN";
        private static readonly string[] ReversingVoices =
        {
            "This vehicle is reversing", "This Prawn Suit is reversing", "None",
            "This Seamoth is reversing", "This Cyclops is reversing"
        };

        private readonly ModConfigFile config;
        private readonly SortedDictionary<int, OptionItem> orderedOptions = new SortedDictionary<int, OptionItem>();

        internal VehicleEnhancementsOptions(ModConfigFile config) : base(MenuName)
        {
            this.config = config;
            AddToggle(100, nameof(config.EnableSeamothSpeedometer), "Speedometer", config.EnableSeamothSpeedometer,
                value => config.EnableSeamothSpeedometer = value);
            AddToggle(101, nameof(config.EnableSeamothHsi), "HSI", config.EnableSeamothHsi,
                value => config.EnableSeamothHsi = value);
            AddToggle(102, nameof(config.EnableSeamothTime), "Time and Weather", config.EnableSeamothTime,
                value => config.EnableSeamothTime = value);
            AddVoiceChoice(103, nameof(config.SeamothReversingVoice), config.SeamothReversingVoice,
                value => config.SeamothReversingVoice = value);
            AddToggle(104, nameof(config.EnableSeamothReversingBeeps), "Reversing Beeps", config.EnableSeamothReversingBeeps,
                value => config.EnableSeamothReversingBeeps = value);

            AddPositionSlider(105, nameof(config.SeamothSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.SeamothSpeedometerHorizontalAdjust,
                false, value => config.SeamothSpeedometerHorizontalAdjust = value);
            AddPositionSlider(106, nameof(config.SeamothSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.SeamothSpeedometerVerticalAdjust,
                true, value => config.SeamothSpeedometerVerticalAdjust = value);
            AddScaleSlider(107, nameof(config.SeamothSpeedometerUiScale), "Speedometer: UI Scale", config.SeamothSpeedometerUiScale,
                value => config.SeamothSpeedometerUiScale = value);
            AddPositionSlider(108, nameof(config.SeamothHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.SeamothHsiHorizontalAdjust,
                false, value => config.SeamothHsiHorizontalAdjust = value);
            AddPositionSlider(109, nameof(config.SeamothHsiVerticalAdjust), "HSI: Vertical Adjust", config.SeamothHsiVerticalAdjust,
                true, value => config.SeamothHsiVerticalAdjust = value);
            AddScaleSlider(110, nameof(config.SeamothHsiUiScale), "HSI: UI Scale", config.SeamothHsiUiScale,
                value => config.SeamothHsiUiScale = value);
            AddPositionSlider(111, nameof(config.SeamothTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.SeamothTimeHorizontalAdjust,
                false, value => config.SeamothTimeHorizontalAdjust = value);
            AddPositionSlider(112, nameof(config.SeamothTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.SeamothTimeVerticalAdjust,
                true, value => config.SeamothTimeVerticalAdjust = value);
            AddScaleSlider(113, nameof(config.SeamothTimeUiScale), "Time/Weather: UI Scale", config.SeamothTimeUiScale,
                value => config.SeamothTimeUiScale = value);

            AddToggle(200, nameof(config.EnablePrawnSpeedometer), "Speedometer", config.EnablePrawnSpeedometer,
                value => config.EnablePrawnSpeedometer = value);
            AddToggle(201, nameof(config.EnablePrawnHsi), "HSI", config.EnablePrawnHsi,
                value => config.EnablePrawnHsi = value);
            AddToggle(202, nameof(config.EnablePrawnTime), "Time and Weather", config.EnablePrawnTime,
                value => config.EnablePrawnTime = value);
            AddVoiceChoice(203, nameof(config.PrawnReversingVoice), config.PrawnReversingVoice,
                value => config.PrawnReversingVoice = value);
            AddToggle(204, nameof(config.EnablePrawnReversingBeeps), "Reversing Beeps", config.EnablePrawnReversingBeeps,
                value => config.EnablePrawnReversingBeeps = value);

            AddPositionSlider(205, nameof(config.PrawnSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.PrawnSpeedometerHorizontalAdjust,
                false, value => config.PrawnSpeedometerHorizontalAdjust = value);
            AddPositionSlider(206, nameof(config.PrawnSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.PrawnSpeedometerVerticalAdjust,
                true, value => config.PrawnSpeedometerVerticalAdjust = value);
            AddScaleSlider(207, nameof(config.PrawnSpeedometerUiScale), "Speedometer: UI Scale", config.PrawnSpeedometerUiScale,
                value => config.PrawnSpeedometerUiScale = value);
            AddPositionSlider(208, nameof(config.PrawnHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.PrawnHsiHorizontalAdjust,
                false, value => config.PrawnHsiHorizontalAdjust = value);
            AddPositionSlider(209, nameof(config.PrawnHsiVerticalAdjust), "HSI: Vertical Adjust", config.PrawnHsiVerticalAdjust,
                true, value => config.PrawnHsiVerticalAdjust = value);
            AddScaleSlider(210, nameof(config.PrawnHsiUiScale), "HSI: UI Scale", config.PrawnHsiUiScale,
                value => config.PrawnHsiUiScale = value);
            AddPositionSlider(211, nameof(config.PrawnTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.PrawnTimeHorizontalAdjust,
                false, value => config.PrawnTimeHorizontalAdjust = value);
            AddPositionSlider(212, nameof(config.PrawnTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.PrawnTimeVerticalAdjust,
                true, value => config.PrawnTimeVerticalAdjust = value);
            AddScaleSlider(213, nameof(config.PrawnTimeUiScale), "Time/Weather: UI Scale", config.PrawnTimeUiScale,
                value => config.PrawnTimeUiScale = value);

            AddToggle(300, nameof(config.EnableCyclopsSpeedometer), "Speedometer", config.EnableCyclopsSpeedometer,
                value => config.EnableCyclopsSpeedometer = value);
            AddToggle(301, nameof(config.EnableCyclopsHsi), "HSI", config.EnableCyclopsHsi,
                value => config.EnableCyclopsHsi = value);
            AddToggle(302, nameof(config.EnableCyclopsTime), "Time and Weather", config.EnableCyclopsTime,
                value => config.EnableCyclopsTime = value);
            AddVoiceChoice(303, nameof(config.CyclopsReversingVoice), config.CyclopsReversingVoice,
                value => config.CyclopsReversingVoice = value);
            AddToggle(304, nameof(config.EnableCyclopsReversingBeeps), "Reversing Beeps", config.EnableCyclopsReversingBeeps,
                value => config.EnableCyclopsReversingBeeps = value);

            AddPositionSlider(305, nameof(config.CyclopsSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.CyclopsSpeedometerHorizontalAdjust,
                false, value => config.CyclopsSpeedometerHorizontalAdjust = value);
            AddPositionSlider(306, nameof(config.CyclopsSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.CyclopsSpeedometerVerticalAdjust,
                true, value => config.CyclopsSpeedometerVerticalAdjust = value);
            AddScaleSlider(307, nameof(config.CyclopsSpeedometerUiScale), "Speedometer: UI Scale", config.CyclopsSpeedometerUiScale,
                value => config.CyclopsSpeedometerUiScale = value);
            AddPositionSlider(308, nameof(config.CyclopsHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.CyclopsHsiHorizontalAdjust,
                false, value => config.CyclopsHsiHorizontalAdjust = value);
            AddPositionSlider(309, nameof(config.CyclopsHsiVerticalAdjust), "HSI: Vertical Adjust", config.CyclopsHsiVerticalAdjust,
                true, value => config.CyclopsHsiVerticalAdjust = value);
            AddScaleSlider(310, nameof(config.CyclopsHsiUiScale), "HSI: UI Scale", config.CyclopsHsiUiScale,
                value => config.CyclopsHsiUiScale = value);
            AddPositionSlider(311, nameof(config.CyclopsTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.CyclopsTimeHorizontalAdjust,
                false, value => config.CyclopsTimeHorizontalAdjust = value);
            AddPositionSlider(312, nameof(config.CyclopsTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.CyclopsTimeVerticalAdjust,
                true, value => config.CyclopsTimeVerticalAdjust = value);
            AddScaleSlider(313, nameof(config.CyclopsTimeUiScale), "Time/Weather: UI Scale", config.CyclopsTimeUiScale,
                value => config.CyclopsTimeUiScale = value);

            AddOption(500, ModSliderOption.Create(nameof(config.ReversingAudioVolume), "Reversing Audio Volume",
                0.0f, 1.0f, config.ReversingAudioVolume, 0.1f, "{0:F2}", 0.05f),
                value => config.ReversingAudioVolume = value);
            AddToggle(501, nameof(config.DetailedLogging), "Detailed Logging", config.DetailedLogging, value =>
            {
                config.DetailedLogging = value;
                ModDebugLog.SetDetailedLoggingState(value);
            });
        }

        /// <summary>
        /// Builds vehicle settings beneath standard subheadings in one mod section.
        /// </summary>
        public override void BuildModOptions(uGUI_TabbedControlsPanel panel, int modsTabIndex, IReadOnlyCollection<OptionItem> options)
        {
            panel.AddHeading(modsTabIndex, MenuName);
            int previousSection = -1;
            foreach (KeyValuePair<int, OptionItem> entry in orderedOptions)
            {
                int section = entry.Key / 100;
                if (section != previousSection)
                {
                    // Stock game headings stay inside the single collapsible Nautilus mod section.
                    GameObject heading = panel.controls.AddHeading(panel.GetParent(modsTabIndex), GetSectionName(section));
                    heading.GetComponentInChildren<TextMeshProUGUI>().fontStyle |= FontStyles.Bold;
                    previousSection = section;
                }

                entry.Value.AddToPanel(panel, modsTabIndex);
            }
        }

        private static string GetSectionName(int section)
        {
            switch (section)
            {
                case 1: return "Seamoth";
                case 2: return "Prawn Suit";
                case 3: return "Cyclops";
                default: return "General";
            }
        }

        private void AddToggle(int order, string id, string label, bool value, Action<bool> setValue)
        {
            AddOption(order, ModToggleOption.Create(id, label, value), setValue);
        }

        private void AddVoiceChoice(int order, string id, ReversingVoice voice, Action<ReversingVoice> setValue)
        {
            ModChoiceOption<string> option = ModChoiceOption<string>.Create(id, "Reversing Voice", ReversingVoices,
                Mathf.Clamp((int)voice, 0, ReversingVoices.Length - 1));
            option.OnChanged += (sender, args) =>
            {
                setValue((ReversingVoice)args.Index);
                config.Save();
            };
            AddItem(option);
            orderedOptions.Add(order, option);
        }

        private void AddPositionSlider(int order, string id, string label, float value, bool vertical, Action<float> setValue)
        {
            string tooltip = vertical
                ? "Offset from the default position in UI units. Positive moves up."
                : "Offset from the default position in UI units. Positive moves right.";
            AddOption(order, ModSliderOption.Create(id, label, ModConfigFile.MinimumHudAdjust, ModConfigFile.MaximumHudAdjust,
                value, 0.0f, "{0:F0}", 1.0f, tooltip), setValue);
        }

        private void AddScaleSlider(int order, string id, string label, float value, Action<float> setValue)
        {
            AddOption(order, ModSliderOption.Create(id, label, ModConfigFile.MinimumHudScale, ModConfigFile.MaximumHudScale,
                value, 1.0f, "{0:F2}x", 0.05f, "Multiplier on the default size. Combines with any scaling already applied by the game."), setValue);
        }

        private void AddOption<T, TEventArgs>(int order, ModOption<T, TEventArgs> option, Action<T> setValue)
            where TEventArgs : ConfigOptionEventArgs<T>
        {
            option.OnChanged += (sender, args) =>
            {
                setValue(args.Value);
                config.Save();
            };
            AddItem(option);
            orderedOptions.Add(order, option);
        }
    }
}
