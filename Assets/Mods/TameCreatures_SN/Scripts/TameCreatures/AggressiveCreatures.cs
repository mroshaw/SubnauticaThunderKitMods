using System.Collections.Generic;
using static DaftAppleGames.TameCreatures_SN.TameCreaturesPlugin;

namespace DaftAppleGames.TameCreatures_SN.TameCreatures
{
    internal static class AggressiveCreatures
    {
        private static readonly Dictionary<TechType, HashSet<Creature>> CreaturesByTechType =
            new Dictionary<TechType, HashSet<Creature>>();

        /// <summary>
        /// Registers a supported aggressive creature and applies its configured friendliness
        /// </summary>
        internal static void Register(Creature creature)
        {
            TechType techType = CraftData.GetTechType(creature.gameObject);
            if (!TryGetConfiguredFriendliness(techType, out var isFriendlyToPlayer))
            {
                return;
            }

            if (!CreaturesByTechType.TryGetValue(techType, out var creatures))
            {
                creatures = new HashSet<Creature>();
                CreaturesByTechType.Add(techType, creatures);
            }

            ModDebugLog.LogDebug($"Adding {techType} '{creature.gameObject.name}' to Aggressive Creatures list, friendliness: {isFriendlyToPlayer}.");
            SetPlayerFriendlinessOnCreature(techType, creature, isFriendlyToPlayer);
            creatures.Add(creature);
        }

        /// <summary>
        /// Unregisters a supported aggressive creature
        /// </summary>
        internal static void Unregister(Creature creature)
        {
            TechType techType = CraftData.GetTechType(creature.gameObject);
            if (!CreaturesByTechType.TryGetValue(techType, out var creatures))
            {
                return;
            }

            ModDebugLog.LogDebug($"Removing {techType} '{creature.gameObject.name}' from Aggressive Creatures list.");
            creatures.Remove(creature);

            if (creatures.Count == 0)
            {
                CreaturesByTechType.Remove(techType);
            }
        }

        /// <summary>
        /// Sets player friendliness on all registered creatures of the given type
        /// </summary>
        internal static void SetPlayerFriendlinessOnAll(TechType creatureTechType, bool isFriendlyToPlayer)
        {
            HashSet<Creature> creatures;
            if (!CreaturesByTechType.TryGetValue(creatureTechType, out creatures))
            {
                return;
            }

            foreach (Creature creature in creatures)
            {
                if (creature)
                {
                    SetPlayerFriendlinessOnCreature(creatureTechType, creature, isFriendlyToPlayer);
                }
            }
        }

        /// <summary>
        /// Gets the configured friendliness when the creature type is supported
        /// </summary>
        private static bool TryGetConfiguredFriendliness(TechType creatureTechType, out bool isFriendlyToPlayer)
        {
            switch (creatureTechType)
            {
                case TechType.Biter:
                    isFriendlyToPlayer = ConfigFile.TameBiters;
                    return true;
                case TechType.Blighter:
                    isFriendlyToPlayer = ConfigFile.TameBlighters;
                    return true;
                case TechType.BoneShark:
                    isFriendlyToPlayer = ConfigFile.TameBoneSharks;
                    return true;
                case TechType.CaveCrawler:
                    isFriendlyToPlayer = ConfigFile.TameCaveCrawlers;
                    return true;
                case TechType.Stalker:
                    isFriendlyToPlayer = ConfigFile.TameStalkers;
                    return true;
                case TechType.CrabSquid:
                    isFriendlyToPlayer = ConfigFile.TameCrabSquids;
                    return true;
                case TechType.Crabsnake:
                    isFriendlyToPlayer = ConfigFile.TameCrabsnakes;
                    return true;
                case TechType.Warper:
                    isFriendlyToPlayer = ConfigFile.TameWarpers;
                    return true;
                case TechType.ReaperLeviathan:
                    isFriendlyToPlayer = ConfigFile.TameReaperLeviathans;
                    return true;
                case TechType.SeaDragon:
                    isFriendlyToPlayer = ConfigFile.TameSeaDragonLeviathans;
                    return true;
                case TechType.GhostLeviathan:
                    isFriendlyToPlayer = ConfigFile.TameGhostLeviathans;
                    return true;
                case TechType.GhostLeviathanJuvenile:
                    isFriendlyToPlayer = ConfigFile.TameGhostLeviathanJuveniles;
                    return true;
                case TechType.LavaLizard:
                    isFriendlyToPlayer = ConfigFile.TameLavaLizards;
                    return true;
                case TechType.Mesmer:
                    isFriendlyToPlayer = ConfigFile.TameMesmers;
                    return true;
                case TechType.Sandshark:
                    isFriendlyToPlayer = ConfigFile.TameSandsharks;
                    return true;
                case TechType.SpineEel:
                    isFriendlyToPlayer = ConfigFile.TameRiverProwlers;
                    return true;
                default:
                    isFriendlyToPlayer = false;
                    return false;
            }
        }

        /// <summary>
        /// Sets player friendliness on one creature
        /// </summary>
        private static void SetPlayerFriendlinessOnCreature(TechType creatureTechType, Creature creature, bool isFriendlyToPlayer)
        {
            ModDebugLog.LogDebug($"Setting friendliness to '{isFriendlyToPlayer}' on {creatureTechType} {creature.gameObject.name}.");
            creature.friendlyToPlayer = isFriendlyToPlayer;
        }
    }
}
