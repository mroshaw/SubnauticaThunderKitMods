using System;
using System.Collections.Generic;
using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Combines built-in categories with player overrides and exposes the applied set.
    /// </summary>
    internal static class CategoryService
    {
        private static readonly List<CategoryDefinition> activeCategories =
            new List<CategoryDefinition>();

        private static CategoryDefinition[] builtInCategories;

        internal static event Action CategoriesChanged;

        internal static IReadOnlyList<CategoryDefinition> ActiveCategories =>
            activeCategories;

        internal static CategoryOverrideFile OverrideFile { get; private set; }

        internal static void Initialize(CategoryDefinition[] defaults)
        {
            builtInCategories = defaults;
            OverrideFile = new CategoryOverrideFile();

            try
            {
                OverrideFile.Load();
            }
            catch (Exception exception)
            {
                ModDebugLog.LogError($"Could not load category overrides. Built-in categories will be used. {exception}");
                OverrideFile = new CategoryOverrideFile();
            }

            ApplyOverrides(false);
        }

        internal static void ApplyOverrides(bool save)
        {
            if (builtInCategories == null)
            {
                return;
            }

            Dictionary<string, CategoryDefinition> effectiveCategories =
                new Dictionary<string, CategoryDefinition>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> removedCategories = CreateRemovedCategorySet();

            int defaultPriority = 10;
            foreach (CategoryDefinition category in builtInCategories)
            {
                if (!removedCategories.Contains(category.Id))
                {
                    effectiveCategories[category.Id] = new CategoryDefinition(
                        category.Id,
                        category.LanguageKey,
                        category.FallbackLabel,
                        defaultPriority,
                        category.ItemTypes);
                }

                defaultPriority += 10;
            }

            List<CategoryOverride> categoryOverrides = OverrideFile.Categories ??
                new List<CategoryOverride>();
            foreach (CategoryOverride categoryOverride in categoryOverrides)
            {
                CategoryDefinition category;
                if (TryCreateCategory(categoryOverride, out category))
                {
                    effectiveCategories[category.Id] = category;
                }
            }

            activeCategories.Clear();
            foreach (CategoryDefinition category in effectiveCategories.Values)
            {
                activeCategories.Add(category);
            }

            activeCategories.Sort(ComparePriority);
            ApplyCategoryOrder();

            if (save)
            {
                OverrideFile.Save();
            }

            Action handler = CategoriesChanged;
            if (handler != null)
            {
                handler();
            }
        }

        internal static void RestoreDefaults()
        {
            OverrideFile.Categories = new List<CategoryOverride>();
            OverrideFile.RemovedCategories = new List<string>();
            OverrideFile.CategoryOrder = new List<string>();
            ApplyOverrides(true);
        }

        internal static List<EditableCategory> CreateDraft()
        {
            List<EditableCategory> draft = new List<EditableCategory>();
            HashSet<string> builtInIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (CategoryDefinition category in builtInCategories)
            {
                builtInIds.Add(category.Id);
            }

            foreach (CategoryDefinition category in activeCategories)
            {
                EditableCategory editableCategory = new EditableCategory
                {
                    Id = category.Id,
                    DisplayName = GetDisplayName(category),
                    IsBuiltIn = builtInIds.Contains(category.Id),
                    IsModified = category.LanguageKey == null
                };
                foreach (TechType techType in category.ItemTypes)
                {
                    editableCategory.TechTypes.Add(techType);
                }

                draft.Add(editableCategory);
            }

            return draft;
        }

        internal static List<EditableCategory> CreateDefaultDraft()
        {
            List<EditableCategory> draft = new List<EditableCategory>();
            foreach (CategoryDefinition category in builtInCategories)
            {
                EditableCategory editableCategory = new EditableCategory
                {
                    Id = category.Id,
                    DisplayName = GetDisplayName(category),
                    IsBuiltIn = true,
                    IsModified = false
                };
                foreach (TechType techType in category.ItemTypes)
                {
                    editableCategory.TechTypes.Add(techType);
                }

                draft.Add(editableCategory);
            }

            return draft;
        }

        internal static void ApplyDraft(List<EditableCategory> draft)
        {
            List<CategoryOverride> overrides = new List<CategoryOverride>();
            HashSet<string> draftIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int priority = 10;
            foreach (EditableCategory category in draft)
            {
                draftIds.Add(category.Id);
                if (!category.IsBuiltIn || category.IsModified)
                {
                    CategoryOverride categoryOverride = new CategoryOverride
                    {
                        Id = category.Id,
                        DisplayName = category.DisplayName,
                        Priority = priority
                    };
                    foreach (TechType techType in category.TechTypes)
                    {
                        categoryOverride.TechTypes.Add(techType.ToString());
                    }

                    overrides.Add(categoryOverride);
                }

                priority += 10;
            }

            List<string> removedCategories = new List<string>();
            foreach (CategoryDefinition category in builtInCategories)
            {
                if (!draftIds.Contains(category.Id))
                {
                    removedCategories.Add(category.Id);
                }
            }

            OverrideFile.Categories = overrides;
            OverrideFile.RemovedCategories = removedCategories;
            OverrideFile.CategoryOrder = new List<string>(draftIds.Count);
            foreach (EditableCategory category in draft)
            {
                OverrideFile.CategoryOrder.Add(category.Id);
            }
            ApplyOverrides(true);
        }

        private static void ApplyCategoryOrder()
        {
            if (OverrideFile.CategoryOrder == null || OverrideFile.CategoryOrder.Count == 0)
            {
                return;
            }

            Dictionary<string, CategoryDefinition> categoriesById =
                new Dictionary<string, CategoryDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (CategoryDefinition category in activeCategories)
            {
                categoriesById[category.Id] = category;
            }

            List<CategoryDefinition> orderedCategories = new List<CategoryDefinition>();
            foreach (string categoryId in OverrideFile.CategoryOrder)
            {
                CategoryDefinition category;
                if (categoriesById.TryGetValue(categoryId, out category))
                {
                    orderedCategories.Add(category);
                    categoriesById.Remove(categoryId);
                }
            }

            foreach (CategoryDefinition category in activeCategories)
            {
                if (categoriesById.ContainsKey(category.Id))
                {
                    orderedCategories.Add(category);
                }
            }

            activeCategories.Clear();
            activeCategories.AddRange(orderedCategories);
        }

        private static string GetDisplayName(CategoryDefinition category)
        {
            if (string.IsNullOrWhiteSpace(category.LanguageKey) || Language.main == null)
            {
                return category.FallbackLabel;
            }

            string localizedName = Language.main.Get(category.LanguageKey);
            return string.IsNullOrWhiteSpace(localizedName) || localizedName == category.LanguageKey
                ? category.FallbackLabel
                : localizedName;
        }

        private static HashSet<string> CreateRemovedCategorySet()
        {
            HashSet<string> removedCategories =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            List<string> removedOverrides = OverrideFile.RemovedCategories ??
                new List<string>();
            foreach (string categoryId in removedOverrides)
            {
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    removedCategories.Add(categoryId);
                }
            }

            return removedCategories;
        }

        private static bool TryCreateCategory(
            CategoryOverride categoryOverride,
            out CategoryDefinition category)
        {
            category = null;
            if (categoryOverride == null ||
                string.IsNullOrWhiteSpace(categoryOverride.Id) ||
                string.IsNullOrWhiteSpace(categoryOverride.DisplayName))
            {
                return false;
            }

            List<TechType> itemTypes = new List<TechType>();
            HashSet<TechType> uniqueItemTypes = new HashSet<TechType>();
            List<string> techTypeNames = categoryOverride.TechTypes ??
                new List<string>();
            foreach (string techTypeName in techTypeNames)
            {
                TechType techType;
                if (!TechTypeExtensions.FromString(techTypeName, out techType, false) ||
                    techType == TechType.None ||
                    !uniqueItemTypes.Add(techType))
                {
                    continue;
                }

                itemTypes.Add(techType);
            }

            if (itemTypes.Count == 0)
            {
                return false;
            }

            category = new CategoryDefinition(
                categoryOverride.Id,
                null,
                categoryOverride.DisplayName,
                categoryOverride.Priority,
                itemTypes);
            return true;
        }

        private static int ComparePriority(
            CategoryDefinition first,
            CategoryDefinition second)
        {
            int priorityComparison = first.Priority.CompareTo(second.Priority);
            return priorityComparison != 0
                ? priorityComparison
                : string.Compare(first.Id, second.Id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
