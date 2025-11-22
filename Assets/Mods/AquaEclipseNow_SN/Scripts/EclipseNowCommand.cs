using System;
using System.Collections.Generic;
using System.Linq;
using Nautilus.Commands;
using Nautilus.Handlers;
using static DaftAppleGames.AquaEclipseNowPlugin.AquaEclipseNowPlugin;

namespace DaftAppleGames.AquaEclipseNowPlugin
{
    public class EclipseNowCommand
    {
        internal static void Register()
        {
            ModDebugLog.LogDebug("Registering EclipseNowCommand...");
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(EclipseNowCommand));
            ModDebugLog.LogDebug("Done Registering EclipseNowCommand.");
        }
        
        [ConsoleCommand("eclipse")]
        public static string EclipseCommandHandler(string mode)
        {
            switch (mode)
            {
                case "now":
                    return EclipseNow();
                case "next":
                    return EclipseNext();
                case "simulation":
                    return EclipseSimulation();
                default:
                    return $"Unknown mode parameter: {mode}. Use valid modes are 'now' and 'next'";
            }
        }

        /// <summary>
        /// Triggers an Eclipse right now
        /// </summary>
        private static string EclipseNow()
        {
            ModDebugLog.LogDebug("Processing eclipse now command...");
            ModDebugLog.LogDebug($"Current time: {DayNightCycle.main.timePassed}.");
            // Start a new simulation
            double timeOfEclipse = DayNightPlanetSimulation.RunSimulation(ConfigFile.SimulationIterations, ConfigFile.SimulationTimeStep , ConfigFile.SimulationThreshold);
            double timeToEclipse = timeOfEclipse - DayNightCycle.main.timePassedAsDouble;
            double timeToSkip = timeToEclipse - ConfigFile.TimeBeforeEclipse;
            ModDebugLog.LogDebug($"Current Time: {DayNightCycle.main.timePassed}, Time of Next Eclipse: {timeOfEclipse}, Time To Eclipse: {timeToEclipse}, Time To Skip: {timeToSkip}");
            DayNightCycle.main.SkipTime((float)(timeToSkip), ConfigFile.TimeSkipDuration);
            return $"Skipped {FormatFriendlyTime(timeToSkip)}. Enjoy the eclipse!";
        }

        /// <summary>
        /// Calculates and returns the number of seconds until the next eclipse
        /// </summary>
        private static string EclipseNext()
        {
            ModDebugLog.LogDebug("Processing eclipse next command...");
            ModDebugLog.LogDebug($"Current time: {DayNightCycle.main.timePassed}.");
            double timeOfEclipse = DayNightPlanetSimulation.RunSimulation(ConfigFile.SimulationIterations, ConfigFile.SimulationTimeStep , ConfigFile.SimulationThreshold);
            double timeToEclipse = timeOfEclipse - DayNightCycle.main.timePassedAsDouble;
            ModDebugLog.LogDebug($"Current Time: {DayNightCycle.main.timePassed}, Time of Next Eclipse: {timeOfEclipse}, Time To Eclipse: {timeToEclipse}");
            return ($"Next eclipse is in {FormatFriendlyTime(timeToEclipse)}!");
        }
        
        /// <summary>
        /// Runs the simulation to derive the highest dot value for config purposes
        /// </summary>
        private static string EclipseSimulation()
        {
            DayNightPlanetSimulation.RunSimulation(ConfigFile.SimulationIterations, ConfigFile.SimulationTimeStep, 0f);
            return "Simulation complete! Check the logs!";
        }
        
        /// <summary>
        /// Return a friendly string version of number of minutes
        /// </summary>
        private static string FormatFriendlyTime(double totalSeconds)
        {
            // Convert to whole seconds (optional)
            long seconds = (long)Math.Floor(totalSeconds);

            long days = seconds / 86400;
            seconds %= 86400;

            long hours = seconds / 3600;
            seconds %= 3600;

            long minutes = seconds / 60;
            seconds %= 60;

            long secs = seconds;

            // Build parts list
            List<string> parts = new List<string>();

            if (days > 0) parts.Add($"{days} day{(days == 1 ? "" : "s")}");
            if (hours > 0) parts.Add($"{hours} hour{(hours == 1 ? "" : "s")}");
            if (minutes > 0) parts.Add($"{minutes} minute{(minutes == 1 ? "" : "s")}");
            if (secs > 0 || parts.Count == 0) parts.Add($"{secs} second{(secs == 1 ? "" : "s")}");

            // Join with commas and "and"
            if (parts.Count == 1)
                return parts[0];

            if (parts.Count == 2)
                return parts[0] + " and " + parts[1];

            return string.Join(", ", parts.Take(parts.Count - 1)) + " and " + parts.Last();
        }
    }
}