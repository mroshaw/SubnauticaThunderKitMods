using System;
using System.Collections.Generic;
using Nautilus.Options;
using DaftAppleGames.VehicleEnhancements_SN.Integration;
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
        private readonly Dictionary<int, string> vehicleHeadings = new Dictionary<int, string>();
        private readonly List<int> generatedOrders = new List<int>();
        private bool applyingCustomVehicleLayout;

        internal VehicleEnhancementsOptions(ModConfigFile config) : base(MenuName)
        {
            this.config = config;
            ModConfigFile defaults = new ModConfigFile();
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

            AddLocationChoice(105, nameof(config.SeamothSpeedometerLocation), "Speedometer: Location", config.SeamothSpeedometerLocation,
                value => config.SeamothSpeedometerLocation = value);
            AddPositionSlider(106, nameof(config.SeamothSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.SeamothSpeedometerHorizontalAdjust,
                false, value => config.SeamothSpeedometerHorizontalAdjust = value, defaults.SeamothSpeedometerHorizontalAdjust);
            AddPositionSlider(107, nameof(config.SeamothSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.SeamothSpeedometerVerticalAdjust,
                true, value => config.SeamothSpeedometerVerticalAdjust = value, defaults.SeamothSpeedometerVerticalAdjust);
            AddScaleSlider(108, nameof(config.SeamothSpeedometerUiScale), "Speedometer: UI Scale", config.SeamothSpeedometerUiScale,
                value => config.SeamothSpeedometerUiScale = value, defaults.SeamothSpeedometerUiScale);
            AddLocationChoice(109, nameof(config.SeamothHsiLocation), "HSI: Location", config.SeamothHsiLocation,
                value => config.SeamothHsiLocation = value);
            AddPositionSlider(110, nameof(config.SeamothHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.SeamothHsiHorizontalAdjust,
                false, value => config.SeamothHsiHorizontalAdjust = value, defaults.SeamothHsiHorizontalAdjust);
            AddPositionSlider(111, nameof(config.SeamothHsiVerticalAdjust), "HSI: Vertical Adjust", config.SeamothHsiVerticalAdjust,
                true, value => config.SeamothHsiVerticalAdjust = value, defaults.SeamothHsiVerticalAdjust);
            AddScaleSlider(112, nameof(config.SeamothHsiUiScale), "HSI: UI Scale", config.SeamothHsiUiScale,
                value => config.SeamothHsiUiScale = value, defaults.SeamothHsiUiScale);
            AddLocationChoice(113, nameof(config.SeamothTimeLocation), "Time/Weather: Location", config.SeamothTimeLocation,
                value => config.SeamothTimeLocation = value);
            AddPositionSlider(114, nameof(config.SeamothTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.SeamothTimeHorizontalAdjust,
                false, value => config.SeamothTimeHorizontalAdjust = value, defaults.SeamothTimeHorizontalAdjust);
            AddPositionSlider(115, nameof(config.SeamothTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.SeamothTimeVerticalAdjust,
                true, value => config.SeamothTimeVerticalAdjust = value, defaults.SeamothTimeVerticalAdjust);
            AddScaleSlider(116, nameof(config.SeamothTimeUiScale), "Time/Weather: UI Scale", config.SeamothTimeUiScale,
                value => config.SeamothTimeUiScale = value, defaults.SeamothTimeUiScale);

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

            AddLocationChoice(205, nameof(config.PrawnSpeedometerLocation), "Speedometer: Location", config.PrawnSpeedometerLocation,
                value => config.PrawnSpeedometerLocation = value);
            AddPositionSlider(206, nameof(config.PrawnSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.PrawnSpeedometerHorizontalAdjust,
                false, value => config.PrawnSpeedometerHorizontalAdjust = value, defaults.PrawnSpeedometerHorizontalAdjust);
            AddPositionSlider(207, nameof(config.PrawnSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.PrawnSpeedometerVerticalAdjust,
                true, value => config.PrawnSpeedometerVerticalAdjust = value, defaults.PrawnSpeedometerVerticalAdjust);
            AddScaleSlider(208, nameof(config.PrawnSpeedometerUiScale), "Speedometer: UI Scale", config.PrawnSpeedometerUiScale,
                value => config.PrawnSpeedometerUiScale = value, defaults.PrawnSpeedometerUiScale);
            AddLocationChoice(209, nameof(config.PrawnHsiLocation), "HSI: Location", config.PrawnHsiLocation,
                value => config.PrawnHsiLocation = value);
            AddPositionSlider(210, nameof(config.PrawnHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.PrawnHsiHorizontalAdjust,
                false, value => config.PrawnHsiHorizontalAdjust = value, defaults.PrawnHsiHorizontalAdjust);
            AddPositionSlider(211, nameof(config.PrawnHsiVerticalAdjust), "HSI: Vertical Adjust", config.PrawnHsiVerticalAdjust,
                true, value => config.PrawnHsiVerticalAdjust = value, defaults.PrawnHsiVerticalAdjust);
            AddScaleSlider(212, nameof(config.PrawnHsiUiScale), "HSI: UI Scale", config.PrawnHsiUiScale,
                value => config.PrawnHsiUiScale = value, defaults.PrawnHsiUiScale);
            AddLocationChoice(213, nameof(config.PrawnTimeLocation), "Time/Weather: Location", config.PrawnTimeLocation,
                value => config.PrawnTimeLocation = value);
            AddPositionSlider(214, nameof(config.PrawnTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.PrawnTimeHorizontalAdjust,
                false, value => config.PrawnTimeHorizontalAdjust = value, defaults.PrawnTimeHorizontalAdjust);
            AddPositionSlider(215, nameof(config.PrawnTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.PrawnTimeVerticalAdjust,
                true, value => config.PrawnTimeVerticalAdjust = value, defaults.PrawnTimeVerticalAdjust);
            AddScaleSlider(216, nameof(config.PrawnTimeUiScale), "Time/Weather: UI Scale", config.PrawnTimeUiScale,
                value => config.PrawnTimeUiScale = value, defaults.PrawnTimeUiScale);

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

            AddLocationChoice(305, nameof(config.CyclopsSpeedometerLocation), "Speedometer: Location", config.CyclopsSpeedometerLocation,
                value => config.CyclopsSpeedometerLocation = value);
            AddPositionSlider(306, nameof(config.CyclopsSpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust", config.CyclopsSpeedometerHorizontalAdjust,
                false, value => config.CyclopsSpeedometerHorizontalAdjust = value, defaults.CyclopsSpeedometerHorizontalAdjust);
            AddPositionSlider(307, nameof(config.CyclopsSpeedometerVerticalAdjust), "Speedometer: Vertical Adjust", config.CyclopsSpeedometerVerticalAdjust,
                true, value => config.CyclopsSpeedometerVerticalAdjust = value, defaults.CyclopsSpeedometerVerticalAdjust);
            AddScaleSlider(308, nameof(config.CyclopsSpeedometerUiScale), "Speedometer: UI Scale", config.CyclopsSpeedometerUiScale,
                value => config.CyclopsSpeedometerUiScale = value, defaults.CyclopsSpeedometerUiScale);
            AddLocationChoice(309, nameof(config.CyclopsHsiLocation), "HSI: Location", config.CyclopsHsiLocation,
                value => config.CyclopsHsiLocation = value);
            AddPositionSlider(310, nameof(config.CyclopsHsiHorizontalAdjust), "HSI: Horizontal Adjust", config.CyclopsHsiHorizontalAdjust,
                false, value => config.CyclopsHsiHorizontalAdjust = value, defaults.CyclopsHsiHorizontalAdjust);
            AddPositionSlider(311, nameof(config.CyclopsHsiVerticalAdjust), "HSI: Vertical Adjust", config.CyclopsHsiVerticalAdjust,
                true, value => config.CyclopsHsiVerticalAdjust = value, defaults.CyclopsHsiVerticalAdjust);
            AddScaleSlider(312, nameof(config.CyclopsHsiUiScale), "HSI: UI Scale", config.CyclopsHsiUiScale,
                value => config.CyclopsHsiUiScale = value, defaults.CyclopsHsiUiScale);
            AddLocationChoice(313, nameof(config.CyclopsTimeLocation), "Time/Weather: Location", config.CyclopsTimeLocation,
                value => config.CyclopsTimeLocation = value);
            AddPositionSlider(314, nameof(config.CyclopsTimeHorizontalAdjust), "Time/Weather: Horizontal Adjust", config.CyclopsTimeHorizontalAdjust,
                false, value => config.CyclopsTimeHorizontalAdjust = value, defaults.CyclopsTimeHorizontalAdjust);
            AddPositionSlider(315, nameof(config.CyclopsTimeVerticalAdjust), "Time/Weather: Vertical Adjust", config.CyclopsTimeVerticalAdjust,
                true, value => config.CyclopsTimeVerticalAdjust = value, defaults.CyclopsTimeVerticalAdjust);
            AddScaleSlider(316, nameof(config.CyclopsTimeUiScale), "Time/Weather: UI Scale", config.CyclopsTimeUiScale,
                value => config.CyclopsTimeUiScale = value, defaults.CyclopsTimeUiScale);

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
            RebuildVehicleOptions();
            panel.AddHeading(modsTabIndex, MenuName);
            List<OptionItem> generalOptions = new List<OptionItem>();
            int previousSection = -1;
            foreach (KeyValuePair<int, OptionItem> entry in orderedOptions)
            {
                int section = entry.Key / 100;
                if (section == 5)
                {
                    generalOptions.Add(entry.Value);
                    continue;
                }

                if (section != previousSection)
                {
                    string name = vehicleHeadings.TryGetValue(section, out string vehicleName) ? vehicleName : GetSectionName(section);
                    AddSectionHeading(panel, modsTabIndex, name);
                    previousSection = section;
                }

                entry.Value.AddToPanel(panel, modsTabIndex);
            }

            AddSectionHeading(panel, modsTabIndex, "General");
            foreach (OptionItem option in generalOptions)
            {
                option.AddToPanel(panel, modsTabIndex);
            }
        }

        private static void AddSectionHeading(uGUI_TabbedControlsPanel panel, int modsTabIndex, string name)
        {
            // Stock game headings stay inside the single collapsible Nautilus mod section.
            GameObject heading = panel.controls.AddHeading(panel.GetParent(modsTabIndex), name);
            heading.GetComponentInChildren<TextMeshProUGUI>().fontStyle |= FontStyles.Bold;
        }

        private void RebuildVehicleOptions()
        {
            foreach (int order in generatedOrders)
            {
                RemoveItem(orderedOptions[order].Id);
                orderedOptions.Remove(order);
            }

            generatedOrders.Clear();
            vehicleHeadings.Clear();
            if (!VehicleFrameworkIntegration.IsEnabled)
            {
                return;
            }

            int section = 6;
            foreach (KeyValuePair<string, string> vehicle in VehicleFrameworkIntegration.GetRegisteredVehicles())
            {
                vehicleHeadings.Add(section, vehicle.Value);
                AddVehicleProfile(section * 100, vehicle.Key, config.GetOrCreateCustomVehicleSettings(vehicle.Key));
                section++;
            }
        }

        private void AddVehicleProfile(int order, string identifier, CustomVehicleSettings settings)
        {
            CustomVehicleSettings defaults = new CustomVehicleSettings();
            string prefix = "CustomVehicleProfile." + identifier + ".";
            AddToggle(order, prefix + nameof(settings.EnableSpeedometer), "Speedometer", settings.EnableSpeedometer,
                value => settings.EnableSpeedometer = value);
            AddToggle(order + 1, prefix + nameof(settings.EnableHsi), "HSI", settings.EnableHsi,
                value => settings.EnableHsi = value);
            AddToggle(order + 2, prefix + nameof(settings.EnableTime), "Time and Weather", settings.EnableTime,
                value => settings.EnableTime = value);
            AddVoiceChoice(order + 3, prefix + nameof(settings.ReversingVoice), settings.ReversingVoice,
                value => settings.ReversingVoice = value);
            AddToggle(order + 4, prefix + nameof(settings.EnableReversingBeeps), "Reversing Beeps", settings.EnableReversingBeeps,
                value => settings.EnableReversingBeeps = value);
            AddLocationChoice(order + 5, prefix + nameof(settings.SpeedometerLocation), "Speedometer: Location", settings.SpeedometerLocation,
                value => settings.SpeedometerLocation = value);
            AddPositionSlider(order + 6, prefix + nameof(settings.SpeedometerHorizontalAdjust), "Speedometer: Horizontal Adjust",
                settings.SpeedometerHorizontalAdjust, false, value => settings.SpeedometerHorizontalAdjust = value, defaults.SpeedometerHorizontalAdjust);
            AddPositionSlider(order + 7, prefix + nameof(settings.SpeedometerVerticalAdjust), "Speedometer: Vertical Adjust",
                settings.SpeedometerVerticalAdjust, true, value => settings.SpeedometerVerticalAdjust = value, defaults.SpeedometerVerticalAdjust);
            AddScaleSlider(order + 8, prefix + nameof(settings.SpeedometerUiScale), "Speedometer: UI Scale",
                settings.SpeedometerUiScale, value => settings.SpeedometerUiScale = value, defaults.SpeedometerUiScale);
            AddLocationChoice(order + 9, prefix + nameof(settings.HsiLocation), "HSI: Location", settings.HsiLocation,
                value => settings.HsiLocation = value);
            AddPositionSlider(order + 10, prefix + nameof(settings.HsiHorizontalAdjust), "HSI: Horizontal Adjust",
                settings.HsiHorizontalAdjust, false, value => settings.HsiHorizontalAdjust = value, defaults.HsiHorizontalAdjust);
            AddPositionSlider(order + 11, prefix + nameof(settings.HsiVerticalAdjust), "HSI: Vertical Adjust",
                settings.HsiVerticalAdjust, true, value => settings.HsiVerticalAdjust = value, defaults.HsiVerticalAdjust);
            AddScaleSlider(order + 12, prefix + nameof(settings.HsiUiScale), "HSI: UI Scale",
                settings.HsiUiScale, value => settings.HsiUiScale = value, defaults.HsiUiScale);
            AddLocationChoice(order + 13, prefix + nameof(settings.TimeLocation), "Time/Weather: Location", settings.TimeLocation,
                value => settings.TimeLocation = value);
            AddPositionSlider(order + 14, prefix + nameof(settings.TimeHorizontalAdjust), "Time/Weather: Horizontal Adjust",
                settings.TimeHorizontalAdjust, false, value => settings.TimeHorizontalAdjust = value, defaults.TimeHorizontalAdjust);
            AddPositionSlider(order + 15, prefix + nameof(settings.TimeVerticalAdjust), "Time/Weather: Vertical Adjust",
                settings.TimeVerticalAdjust, true, value => settings.TimeVerticalAdjust = value, defaults.TimeVerticalAdjust);
            AddScaleSlider(order + 16, prefix + nameof(settings.TimeUiScale), "Time/Weather: UI Scale",
                settings.TimeUiScale, value => settings.TimeUiScale = value, defaults.TimeUiScale);

            ModButtonOption applyLayout = ModButtonOption.Create(prefix + "ApplyLayoutToAll",
                "Apply layout to all custom", args => ApplyLayoutToAllCustomVehicles(settings),
                "Copy this vehicle's instrument locations, position offsets and UI scales to all other custom vehicles.");
            AddItem(applyLayout);
            orderedOptions.Add(order + 17, applyLayout);

            for (int offset = 0; offset < 18; offset++)
            {
                generatedOrders.Add(order + offset);
            }
        }

        private void ApplyLayoutToAllCustomVehicles(CustomVehicleSettings source)
        {
            foreach (CustomVehicleSettings settings in config.CustomVehicleProfiles.Values)
            {
                settings.CopyLayoutFrom(source);
            }

            // Refresh open controls without saving the config for every slider and choice callback.
            applyingCustomVehicleLayout = true;
            try
            {
                foreach (int section in vehicleHeadings.Keys)
                {
                    int order = section * 100;
                    RefreshLocationChoice(order + 5, source.SpeedometerLocation);
                    RefreshSlider(order + 6, source.SpeedometerHorizontalAdjust);
                    RefreshSlider(order + 7, source.SpeedometerVerticalAdjust);
                    RefreshSlider(order + 8, source.SpeedometerUiScale);
                    RefreshLocationChoice(order + 9, source.HsiLocation);
                    RefreshSlider(order + 10, source.HsiHorizontalAdjust);
                    RefreshSlider(order + 11, source.HsiVerticalAdjust);
                    RefreshSlider(order + 12, source.HsiUiScale);
                    RefreshLocationChoice(order + 13, source.TimeLocation);
                    RefreshSlider(order + 14, source.TimeHorizontalAdjust);
                    RefreshSlider(order + 15, source.TimeVerticalAdjust);
                    RefreshSlider(order + 16, source.TimeUiScale);
                }
            }
            finally
            {
                applyingCustomVehicleLayout = false;
                config.Save();
            }
        }

        private void RefreshLocationChoice(int order, HudLocation location)
        {
            ModChoiceOption<string> option = (ModChoiceOption<string>)orderedOptions[order];
            if (option.Choice)
            {
                option.Choice.value = (int)location;
            }
        }

        private void RefreshSlider(int order, float value)
        {
            ModSliderOption option = (ModSliderOption)orderedOptions[order];
            option.Value = value;
            if (option.OptionGameObject)
            {
                uGUI_SnappingSlider slider = option.OptionGameObject.GetComponentInChildren<uGUI_SnappingSlider>(true);
                if (slider)
                {
                    slider.unsnappedValue = value;
                }
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

        private void AddLocationChoice(int order, string id, string label, HudLocation location, Action<HudLocation> setValue)
        {
            ModChoiceOption<string> option = ModChoiceOption<string>.Create(id, label,
                new[] { "Left", "Center", "Right" }, Mathf.Clamp((int)location, 0, 2));
            option.OnChanged += (sender, args) =>
            {
                setValue((HudLocation)args.Index);
                if (!applyingCustomVehicleLayout)
                {
                    config.Save();
                }
            };
            AddItem(option);
            orderedOptions.Add(order, option);
        }

        private void AddPositionSlider(int order, string id, string label, float value, bool vertical, Action<float> setValue, float defaultValue)
        {
            string tooltip = vertical
                ? "Offset from the selected location in UI units. Positive moves up."
                : "Offset from the selected location in UI units. Positive moves right.";
            AddOption(order, ModSliderOption.Create(id, label, ModConfigFile.MinimumHudAdjust, ModConfigFile.MaximumHudAdjust,
                value, defaultValue, "{0:F1}", 1.0f, tooltip), setValue);
        }

        private void AddScaleSlider(int order, string id, string label, float value, Action<float> setValue, float defaultValue)
        {
            AddOption(order, ModSliderOption.Create(id, label, ModConfigFile.MinimumHudScale, ModConfigFile.MaximumHudScale,
                value, defaultValue, "{0:F2}x", 0.05f, "Scale within the selected location. Inherits any scaling applied by the game."), setValue);
        }

        private void AddOption<T, TEventArgs>(int order, ModOption<T, TEventArgs> option, Action<T> setValue)
            where TEventArgs : ConfigOptionEventArgs<T>
        {
            option.OnChanged += (sender, args) =>
            {
                setValue(args.Value);
                if (!applyingCustomVehicleLayout)
                {
                    config.Save();
                }
            };
            AddItem(option);
            orderedOptions.Add(order, option);
        }
    }
}
