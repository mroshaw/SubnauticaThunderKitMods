using System.Collections.Generic;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    internal static class LabelGenerator
    {
        private const float DominantItemRatio = 0.6f;
        private const string MixedCategoryKey = "AutoLockerLabels_Category_Mixed";
        private const string MixedCategoryFallback = "Mixed";

        private static readonly AutomaticLabelCategory[] Categories =
        {
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Metals",
                "Metals",
                new[]
                {
                    TechType.Titanium,
                    TechType.Copper,
                    TechType.Silver,
                    TechType.Gold,
                    TechType.Lead,
                    TechType.Lithium,
                    TechType.Nickel,
                    TechType.Magnetite
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Crystals",
                "Crystals",
                new[]
                {
                    TechType.Quartz,
                    TechType.Diamond,
                    TechType.AluminumOxide,
                    TechType.Kyanite
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Electronics",
                "Electronics",
                new[]
                {
                    TechType.Battery,
                    TechType.PrecursorIonBattery,
                    TechType.PowerCell,
                    TechType.PrecursorIonPowerCell,
                    TechType.CopperWire,
                    TechType.WiringKit,
                    TechType.AdvancedWiringKit,
                    TechType.ComputerChip,
                    TechType.ReactorRod
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Food",
                "Food",
                new[]
                {
                    TechType.NutrientBlock,
                    TechType.Snack1,
                    TechType.Snack2,
                    TechType.Snack3,
                    TechType.HoleFish,
                    TechType.Peeper,
                    TechType.Bladderfish,
                    TechType.GarryFish,
                    TechType.Hoverfish,
                    TechType.Reginald,
                    TechType.Spadefish,
                    TechType.Boomerang,
                    TechType.LavaBoomerang,
                    TechType.Eyeye,
                    TechType.LavaEyeye,
                    TechType.Oculus,
                    TechType.Hoopfish,
                    TechType.Spinefish,
                    TechType.CookedHoleFish,
                    TechType.CookedPeeper,
                    TechType.CookedBladderfish,
                    TechType.CookedGarryFish,
                    TechType.CookedHoverfish,
                    TechType.CookedReginald,
                    TechType.CookedSpadefish,
                    TechType.CookedBoomerang,
                    TechType.CookedLavaBoomerang,
                    TechType.CookedEyeye,
                    TechType.CookedLavaEyeye,
                    TechType.CookedOculus,
                    TechType.CookedHoopfish,
                    TechType.CookedSpinefish,
                    TechType.CuredHoleFish,
                    TechType.CuredPeeper,
                    TechType.CuredBladderfish,
                    TechType.CuredGarryFish,
                    TechType.CuredHoverfish,
                    TechType.CuredReginald,
                    TechType.CuredSpadefish,
                    TechType.CuredBoomerang,
                    TechType.CuredLavaBoomerang,
                    TechType.CuredEyeye,
                    TechType.CuredLavaEyeye,
                    TechType.CuredOculus,
                    TechType.CuredHoopfish,
                    TechType.CuredSpinefish,
                    TechType.BulboTreePiece,
                    TechType.PurpleVegetable,
                    TechType.HangingFruit,
                    TechType.Melon,
                    TechType.SmallMelon
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Water",
                "Water",
                new[]
                {
                    TechType.FilteredWater,
                    TechType.DisinfectedWater,
                    TechType.BigFilteredWater
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Tools",
                "Tools",
                new[]
                {
                    TechType.Scanner,
                    TechType.Welder,
                    TechType.Flashlight,
                    TechType.Knife,
                    TechType.DiveReel,
                    TechType.AirBladder,
                    TechType.Flare,
                    TechType.Builder,
                    TechType.LaserCutter,
                    TechType.StasisRifle,
                    TechType.Terraformer,
                    TechType.PropulsionCannon,
                    TechType.LEDLight,
                    TechType.Transfuser
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Equipment",
                "Equipment",
                new[]
                {
                    TechType.Tank,
                    TechType.DoubleTank,
                    TechType.Fins,
                    TechType.RadiationSuit,
                    TechType.ReinforcedDiveSuit,
                    TechType.WaterFiltrationSuit,
                    TechType.FirstAidKit,
                    TechType.FireExtinguisher,
                    TechType.Rebreather,
                    TechType.Compass,
                    TechType.Thermometer,
                    TechType.Pipe,
                    TechType.PipeSurfaceFloater,
                    TechType.PrecursorKey_Purple,
                    TechType.PrecursorKey_Blue,
                    TechType.PrecursorKey_Orange
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Materials",
                "Materials",
                new[]
                {
                    TechType.Titanium,
                    TechType.TitaniumIngot,
                    TechType.FiberMesh,
                    TechType.Silicone,
                    TechType.Glass,
                    TechType.Bleach,
                    TechType.Lubricant,
                    TechType.EnameledGlass,
                    TechType.PlasteelIngot,
                    TechType.HydrochloricAcid,
                    TechType.Benzene,
                    TechType.AramidFibers,
                    TechType.Aerogel,
                    TechType.Polyaniline,
                    TechType.HatchingEnzymes
                }),
            new AutomaticLabelCategory(
                "AutoLockerLabels_Category_Raw Materials",
                "Raw Materials",
                new[]
                {
                    TechType.Titanium,
                    TechType.Copper,
                    TechType.Silver,
                    TechType.Gold,
                    TechType.Lead,
                    TechType.Lithium,
                    TechType.Nickel,
                    TechType.Magnetite,
                    TechType.Quartz,
                    TechType.Diamond,
                    TechType.AluminumOxide,
                    TechType.Kyanite,
                    TechType.Salt,
                    TechType.Sulphur,
                    TechType.UraniniteCrystal,
                    TechType.ScrapMetal,
                    TechType.StalkerTooth,
                    TechType.GasPod,
                    TechType.BloodOil,
                    TechType.AcidMushroom,
                    TechType.WhiteMushroom,
                    TechType.JellyPlant,
                    TechType.CreepvinePiece,
                    TechType.CreepvineSeedCluster
                })
        };

        internal static string Generate(ItemsContainer container)
        {
            if (container == null || container.count == 0)
            {
                return GetLocalizedLabel("AutoLockerLabels_Category_Empty", "Empty");
            }

            List<TechType> itemTypes = container.GetItemTypes();

            if (itemTypes.Count == 1)
            {
                return GetItemLabel(itemTypes[0]);
            }

            int totalCount = 0;
            int highestCount = 0;
            TechType dominantType = TechType.None;

            foreach (TechType techType in itemTypes)
            {
                int count = container.GetCount(techType);
                totalCount += count;

                if (count > highestCount)
                {
                    highestCount = count;
                    dominantType = techType;
                }
            }

            if (IsDominant(highestCount, totalCount))
            {
                return GetItemLabel(dominantType);
            }

            if (TryGetCommonCategoryLabel(
                    itemTypes,
                    out string categoryLabel))
            {
                return categoryLabel;
            }

            return GetLocalizedLabel(
                MixedCategoryKey,
                MixedCategoryFallback);
        }

        private static bool TryGetCommonCategoryLabel(
            List<TechType> itemTypes,
            out string categoryLabel)
        {
            foreach (AutomaticLabelCategory category in Categories)
            {
                if (!category.ContainsAll(itemTypes))
                {
                    continue;
                }

                categoryLabel = GetLocalizedLabel(
                    category.LanguageKey,
                    category.FallbackLabel);
                return true;
            }

            categoryLabel = string.Empty;
            return false;
        }

        private static bool IsDominant(
            int highestCount,
            int totalCount)
        {
            if (totalCount <= 0)
            {
                return false;
            }

            return (float)highestCount / totalCount >=
                   DominantItemRatio;
        }

        private static string GetItemLabel(TechType techType)
        {
            Language language = Language.main;
            string localizedName = language == null
                ? string.Empty
                : language.Get(techType);

            return string.IsNullOrWhiteSpace(localizedName)
                ? techType.ToString()
                : localizedName;
        }

        private static string GetLocalizedLabel(
            string languageKey,
            string fallbackLabel)
        {
            Language language = Language.main;

            if (language == null)
            {
                return fallbackLabel;
            }

            string localizedLabel = language.Get(languageKey);

            if (string.IsNullOrWhiteSpace(localizedLabel) ||
                localizedLabel == languageKey)
            {
                return fallbackLabel;
            }

            return localizedLabel;
        }

        private sealed class AutomaticLabelCategory
        {
            private readonly HashSet<TechType> itemTypes;

            internal string LanguageKey { get; }

            internal string FallbackLabel { get; }

            internal AutomaticLabelCategory(
                string languageKey,
                string fallbackLabel,
                IEnumerable<TechType> itemTypes)
            {
                LanguageKey = languageKey;
                FallbackLabel = fallbackLabel;
                this.itemTypes = new HashSet<TechType>(itemTypes);
            }

            internal bool ContainsAll(List<TechType> contents)
            {
                foreach (TechType techType in contents)
                {
                    if (!itemTypes.Contains(techType))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
