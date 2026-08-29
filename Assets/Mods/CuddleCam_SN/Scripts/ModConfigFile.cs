using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{    /// <summary>
    /// Nautilus mod config class
    /// </summary>
    [Menu("CuddleCam")]
    public class ModConfigFile : ConfigFile
    {
        /// <summary>
        /// Number of CuddleCam feed renders per second
        /// </summary>
        [Slider(
            "Feed refresh rate",
            Tooltip = "Number of CuddleCam feed updates per second. Higher values are smoother but use more graphics processing time.",
            Step = 1,
            Format = "{0} FPS",
            Min = 1,
            Max = 60,
            DefaultValue = 30)]
        public int FeedRefreshRate = 30;

        /// <summary>
        /// Resolution used for CuddleCam feeds
        /// </summary>
        [Choice(
            "Feed quality",
            "Low",
            "Medium",
            "High",
            Tooltip = "Controls CuddleCam feed resolution. Higher quality uses more graphics processing time and video memory.")]
        public string FeedQuality = "Medium";

        /// <summary>
        /// Pause monitors that are far from the player
        /// </summary>
        [Toggle(
            "Pause distant monitors",
            Tooltip = "Stops rendering feeds on monitors that are farther away than the activation distance.")]
        public bool PauseDistantMonitors = true;

        /// <summary>
        /// Pause monitors outside the player camera view
        /// </summary>
        [Toggle(
            "Pause off-screen monitors",
            Tooltip = "Stops rendering feeds on monitors that remain outside the player camera view.")]
        public bool PauseOffscreenMonitors = true;

        /// <summary>
        /// Maximum distance at which a monitor renders its feed
        /// </summary>
        [Slider(
            "Monitor activation distance",
            Tooltip = "Maximum player distance at which a CuddleCam monitor renders its selected feed.",
            Step = 5,
            Format = "{0} m",
            Min = 5,
            Max = 100,
            DefaultValue = 30)]
        public int MonitorActivationDistance = 30;

        /// <summary>
        /// Enable vanilla underwater waterscape rendering on CuddleCam feeds
        /// </summary>
        [Toggle(
            "Underwater effects",
            Tooltip = "Applies the game's underwater fog, scattering, colour attenuation, and caustics to CuddleCam feeds.")]
        public bool EnableWaterscapeEffects = true;

        
        /// <summary>
        /// Enable detailed logging
        /// </summary>
        [Toggle("Detailed logging", Tooltip="Use this to produce a detailed log when reporting bugs. Logs are written to %LOCALAPPDATA%low\\Unknown Worlds\\Subnautica\\Player.log"), OnChange(nameof(OnLoggingChanged))]
        public bool DetailedLogging = false;
        
        /// <summary>
        /// Handle toggling of detailed logging
        /// </summary>
        private void OnLoggingChanged(ToggleChangedEventArgs eventArgs)
        {
            ModDebugLog.SetDetailedLoggingState(eventArgs.Value);
        }
    }
}
