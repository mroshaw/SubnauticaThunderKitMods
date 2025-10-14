using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;

namespace DaftAppleGames.BetterAquariums_SN
{
    /// <summary>
    /// Nautilus mod config class
    /// </summary>
    [Menu("Better Aquariums")]
    internal class ModConfigFile : ConfigFile
    {
        [Toggle("Detailed Logging", Tooltip="Only check this if you have a problem and need to see the debug output of the mod in the Player.log file"), OnChange(nameof(DetailedLoggingChangedHandler))]
        public bool DetailedLogging = false;

        /// <summary>
        /// Set the Detailed Logging on the Mod Logger
        /// </summary>
        private void DetailedLoggingChangedHandler(ToggleChangedEventArgs newArgs)
        {
            BetterAquariumsPlugin.ModDebugLog.SetDetailedLoggingState(newArgs.Value);
        }
    }
}