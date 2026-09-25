using DaftAppleGames.TameCreatures_SN.TameCreatures;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using static DaftAppleGames.TameCreatures_SN.TameCreaturesPlugin;

namespace DaftAppleGames.TameCreatures_SN
{
    /// <summary>
    /// Nautilus mod config class
    /// </summary>
    [Menu("Tame Creatures")]
    public class ModConfigFile : ConfigFile
    {
        [Toggle("Tame Biters", Tooltip = "Toggle this ON to tame Biters."), OnChange(nameof(OnTameBitersChanged))]
        public bool TameBiters = true;

        [Toggle("Tame Blighters", Tooltip = "Toggle this ON to tame Blighters."), OnChange(nameof(OnTameBlightersChanged))]
        public bool TameBlighters = true;

        [Toggle("Tame Bone Sharks", Tooltip = "Toggle this ON to tame Bone Sharks."), OnChange(nameof(OnTameBoneSharksChanged))]
        public bool TameBoneSharks = true;

        [Toggle("Tame Cave Crawlers", Tooltip = "Toggle this ON to tame Cave Crawlers."), OnChange(nameof(OnTameCaveCrawlersChanged))]
        public bool TameCaveCrawlers = true;

        [Toggle("Tame Stalkers", Tooltip = "Toggle this ON to tame Stalkers."), OnChange(nameof(OnTameStalkersChanged))]
        public bool TameStalkers = true;
        
        [Toggle("Tame Crab Squids", Tooltip = "Toggle this ON to tame Crab Squids."), OnChange(nameof(OnTameCrabSquidsChanged))]
        public bool TameCrabSquids = true;

        [Toggle("Tame Crabsnakes", Tooltip = "Toggle this ON to tame Crabsnakes."), OnChange(nameof(OnTameCrabsnakesChanged))]
        public bool TameCrabsnakes = true;
        
        [Toggle("Tame Warpers", Tooltip = "Toggle this ON to tame Warpers."), OnChange(nameof(OnTameWarpersChanged))]
        public bool TameWarpers = true;
        
        [Toggle("Tame Reaper Leviathans", Tooltip = "Toggle this ON to tame Reaper Leviathans."), OnChange(nameof(OnTameReaperLeviathansChanged))]
        public bool TameReaperLeviathans = true;
        
        [Toggle("Tame Sea Dragon Leviathans", Tooltip = "Toggle this ON to tame Sea Dragon Leviathans."), OnChange(nameof(OnTameSeaDragonLeviathansChanged))]
        public bool TameSeaDragonLeviathans = true;
        
        [Toggle("Tame Ghost Leviathans", Tooltip = "Toggle this ON to tame Ghost Leviathans."), OnChange(nameof(OnTameGhostLeviathansChanged))]
        public bool TameGhostLeviathans = true;

        [Toggle("Tame Juvenile Ghost Leviathans", Tooltip = "Toggle this ON to tame Juvenile Ghost Leviathans."), OnChange(nameof(OnTameGhostLeviathanJuvenilesChanged))]
        public bool TameGhostLeviathanJuveniles = true;

        [Toggle("Tame Lava Lizards", Tooltip = "Toggle this ON to tame Lava Lizards."), OnChange(nameof(OnTameLavaLizardsChanged))]
        public bool TameLavaLizards = true;

        [Toggle("Tame Mesmers", Tooltip = "Toggle this ON to tame Mesmers."), OnChange(nameof(OnTameMesmersChanged))]
        public bool TameMesmers = true;

        [Toggle("Tame Sand Sharks", Tooltip = "Toggle this ON to tame Sand Sharks."), OnChange(nameof(OnTameSandsharksChanged))]
        public bool TameSandsharks = true;

        [Toggle("Tame River Prowlers", Tooltip = "Toggle this ON to tame River Prowlers."), OnChange(nameof(OnTameRiverProwlersChanged))]
        public bool TameRiverProwlers = true;
        
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

        private void OnTameStalkersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Stalker, eventArgs.Value);
        }

        private void OnTameBitersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Biter, eventArgs.Value);
        }

        private void OnTameBlightersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Blighter, eventArgs.Value);
        }

        private void OnTameBoneSharksChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.BoneShark, eventArgs.Value);
        }

        private void OnTameCaveCrawlersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.CaveCrawler, eventArgs.Value);
        }

        private void OnTameCrabSquidsChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.CrabSquid, eventArgs.Value);
        }

        private void OnTameCrabsnakesChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Crabsnake, eventArgs.Value);
        }

        private void OnTameWarpersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Warper, eventArgs.Value);
        }

        private void OnTameReaperLeviathansChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.ReaperLeviathan, eventArgs.Value);
        }

        private void OnTameSeaDragonLeviathansChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.SeaDragon, eventArgs.Value);
        }

        private void OnTameGhostLeviathansChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.GhostLeviathan, eventArgs.Value);
        }

        private void OnTameGhostLeviathanJuvenilesChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.GhostLeviathanJuvenile, eventArgs.Value);
        }

        private void OnTameLavaLizardsChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.LavaLizard, eventArgs.Value);
        }

        private void OnTameMesmersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Mesmer, eventArgs.Value);
        }

        private void OnTameSandsharksChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.Sandshark, eventArgs.Value);
        }

        private void OnTameRiverProwlersChanged(ToggleChangedEventArgs eventArgs)
        {
            AggressiveCreatures.SetPlayerFriendlinessOnAll(TechType.SpineEel, eventArgs.Value);
        }
    }
}
