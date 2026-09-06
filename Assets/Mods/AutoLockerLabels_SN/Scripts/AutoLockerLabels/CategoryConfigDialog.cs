using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Bootstrap;
using DaftAppleGames.ModTools.Extensions;
using Nautilus.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Edits automatic locker label categories using an unapplied working copy.
    /// </summary>
    public sealed class CategoryConfigDialog : MonoBehaviour
    {
        private const string MainMenuOptionsPanelPath = "Panel/Options";
        private const string InGameMenuOptionsPanelPath = "Options";

        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private Transform categoryContent;
        [SerializeField] private Transform techTypeContent;
        [SerializeField] private CategoryListEntry categoryEntryPrefab;
        [SerializeField] private TechTypeListEntry techTypeEntryPrefab;
        [SerializeField] private TMP_InputField categoryNameInput;
        [SerializeField] private TMP_Text categoryStatusText;
        [SerializeField] private Button addButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button doneButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button restoreDefaultsButton;
        [SerializeField] private Button addTechTypeButton;
        [SerializeField] private GameObject pickerPanel;
        [SerializeField] private TMP_InputField pickerSearchInput;
        [SerializeField] private Toggle pickerCustomOnlyToggle;
        [SerializeField] private Transform pickerContent;
        [SerializeField] private TechTypePickerEntry pickerEntryPrefab;
        [SerializeField] private TMP_Text pickerEmptyText;
        [SerializeField] private Button pickerCloseButton;

        private readonly List<CategoryListEntry> categoryEntries = new List<CategoryListEntry>();
        private readonly List<TechTypeListEntry> techTypeEntries = new List<TechTypeListEntry>();
        private readonly List<TechTypePickerEntry> pickerEntries = new List<TechTypePickerEntry>();
        private readonly List<TechType> availableTechTypes = new List<TechType>();
        private readonly List<TechType> candidateTechTypes = new List<TechType>();
        private readonly InventoryTechTypeCatalog inventoryTechTypeCatalog =
            new InventoryTechTypeCatalog();
        private List<EditableCategory> draft;
        private CanvasGroup parentCanvasGroup;
        private int selectedIndex = -1;
        private bool isShowing;
        private Coroutine catalogCoroutine;

        private void Awake()
        {
            versionText.text = $"v{VersionString}";
            addButton.onClick.AddListener(AddCategory);
            removeButton.onClick.AddListener(RemoveCategory);
            moveUpButton.onClick.AddListener(MoveUp);
            moveDownButton.onClick.AddListener(MoveDown);
            applyButton.onClick.AddListener(Apply);
            doneButton.onClick.AddListener(Done);
            cancelButton.onClick.AddListener(Cancel);
            restoreDefaultsButton.onClick.AddListener(RestoreDefaults);
            addTechTypeButton.onClick.AddListener(OpenTechTypePicker);
            pickerCloseButton.onClick.AddListener(CloseTechTypePicker);
            pickerSearchInput.onValueChanged.AddListener(FilterTechTypePicker);
            pickerCustomOnlyToggle.onValueChanged.AddListener(FilterTechTypePickerBySource);
            categoryNameInput.onEndEdit.AddListener(RenameSelectedCategory);
            pickerPanel.SetActive(false);
            Hide();
        }

        internal void Reparent()
        {
            GameObject optionsPanel = GetOptionsPanel();
            if (!optionsPanel)
            {
                ModDebugLog.LogError("Could not find the Options panel for the category dialog.");
                return;
            }

            mainPanel.transform.SetParent(optionsPanel.transform);
            mainPanel.transform.LocalZero();
            parentCanvasGroup = optionsPanel.GetComponentInParent<CanvasGroup>();
        }

        internal void Show()
        {
            if (isShowing)
            {
                return;
            }

            draft = CategoryService.CreateDraft();
            selectedIndex = draft.Count == 0 ? -1 : 0;
            mainPanel.SetActive(true);
            SetParentInteraction(false);
            isShowing = true;
            RefreshAll();
        }

        private void AddCategory()
        {
            EditableCategory source = GetSelectedCategory();
            EditableCategory category = new EditableCategory
            {
                Id = "custom-" + Guid.NewGuid().ToString("N"),
                DisplayName = "New Category",
                IsBuiltIn = false,
                IsModified = true
            };
            if (source != null)
            {
                category.TechTypes.AddRange(source.TechTypes);
            }

            draft.Add(category);
            selectedIndex = draft.Count - 1;
            RefreshAll();
            categoryNameInput.Select();
        }

        private void RemoveCategory()
        {
            if (selectedIndex < 0 || selectedIndex >= draft.Count)
            {
                return;
            }

            draft.RemoveAt(selectedIndex);
            if (selectedIndex >= draft.Count)
            {
                selectedIndex = draft.Count - 1;
            }

            RefreshAll();
        }

        private void MoveUp()
        {
            MoveSelected(-1);
        }

        private void MoveDown()
        {
            MoveSelected(1);
        }

        private void MoveSelected(int offset)
        {
            int destination = selectedIndex + offset;
            if (selectedIndex < 0 || destination < 0 || destination >= draft.Count)
            {
                return;
            }

            EditableCategory category = draft[selectedIndex];
            draft[selectedIndex] = draft[destination];
            draft[destination] = category;
            selectedIndex = destination;
            RefreshAll();
        }

        private void RenameSelectedCategory(string categoryName)
        {
            EditableCategory category = GetSelectedCategory();
            if (category == null || string.IsNullOrWhiteSpace(categoryName))
            {
                RefreshDetails();
                return;
            }

            string normalizedName = categoryName.Trim();
            if (string.Equals(category.DisplayName, normalizedName, StringComparison.Ordinal))
            {
                RefreshDetails();
                return;
            }

            category.DisplayName = normalizedName;
            category.IsModified = true;
            RefreshCategories();
            RefreshDetails();
        }

        private void RemoveTechType(TechType techType)
        {
            EditableCategory category = GetSelectedCategory();
            if (category == null)
            {
                return;
            }

            if (category.TechTypes.Count <= 1)
            {
                return;
            }

            category.TechTypes.Remove(techType);
            category.IsModified = true;
            RefreshTechTypes();
            RefreshCategories();
        }

        private void OpenTechTypePicker()
        {
            if (GetSelectedCategory() == null)
            {
                return;
            }

            BuildCandidateTechTypes();
            availableTechTypes.Clear();
            availableTechTypes.AddRange(candidateTechTypes);
            availableTechTypes.Sort(CompareTechTypeNames);
            pickerSearchInput.SetTextWithoutNotify(string.Empty);
            pickerCustomOnlyToggle.SetIsOnWithoutNotify(false);
            pickerSearchInput.interactable = true;
            pickerPanel.SetActive(true);
            ClearEntries(pickerEntries);

            // Prefab inspection is retained for diagnostics, but is too expensive for normal picker use.
            // catalogCoroutine = StartCoroutine(inventoryTechTypeCatalog.Filter(
            //     candidateTechTypes,
            //     AcceptInventoryTechType,
            //     LogRejectedInventoryTechType,
            //     UpdateCatalogProgress,
            //     CompleteInventoryCatalog));

            RefreshTechTypePicker(string.Empty);
            pickerSearchInput.Select();
        }

        private void CloseTechTypePicker()
        {
            if (catalogCoroutine != null)
            {
                StopCoroutine(catalogCoroutine);
                catalogCoroutine = null;
            }

            pickerPanel.SetActive(false);
            ClearEntries(pickerEntries);
        }

        private void FilterTechTypePicker(string searchText)
        {
            RefreshTechTypePicker(searchText);
        }

        private void FilterTechTypePickerBySource(bool customOnly)
        {
            RefreshTechTypePicker(pickerSearchInput.text);
        }

        private void SelectTechType(TechType techType)
        {
            EditableCategory category = GetSelectedCategory();
            if (category == null || category.TechTypes.Contains(techType))
            {
                return;
            }

            category.TechTypes.Add(techType);
            category.IsModified = true;
            CloseTechTypePicker();
            RefreshTechTypes();
            RefreshCategories();
        }

        private void AcceptInventoryTechType(TechType techType)
        {
            availableTechTypes.Add(techType);
        }

        private static void LogRejectedInventoryTechType(TechType techType)
        {
            ModDebugLog.LogInfo("VANILLA_NON_PICKUPABLE: " + techType);
        }

        private void UpdateCatalogProgress(int processed, int total)
        {
            pickerEmptyText.text = "Checking inventory-compatible items... " + processed + "/" + total;
        }

        private void CompleteInventoryCatalog()
        {
            catalogCoroutine = null;
            availableTechTypes.Sort(CompareTechTypeNames);
            pickerSearchInput.interactable = true;
            ModDebugLog.LogInfo(
                "TechType pickupable catalogue: candidates=" + candidateTechTypes.Count +
                ", pickupable=" + availableTechTypes.Count + ".");
            if (pickerPanel.activeSelf)
            {
                RefreshTechTypePicker(pickerSearchInput.text);
            }
        }

        private void BuildCandidateTechTypes()
        {
            candidateTechTypes.Clear();
            EditableCategory category = GetSelectedCategory();
            HashSet<TechType> uniqueTechTypes = new HashSet<TechType>();
            Array values = Enum.GetValues(typeof(TechType));
            int noneCount = 0;
            int duplicateCount = 0;
            int assignedCount = 0;
            int missingClassIdCount = 0;
            int missingNameCount = 0;
            int missingRegisteredIconCount = 0;
            int vanillaExclusionCount = 0;
            int exceptionCount = 0;
            StringBuilder exclusionSamples = new StringBuilder();
            foreach (object value in values)
            {
                TechType techType = (TechType)value;
                if (techType == TechType.None)
                {
                    noneCount++;
                    continue;
                }

                if (!uniqueTechTypes.Add(techType))
                {
                    duplicateCount++;
                    continue;
                }

                if (category.TechTypes.Contains(techType))
                {
                    assignedCount++;
                    continue;
                }

                string exclusionReason;
                if (!IsSelectableTechType(techType, out exclusionReason))
                {
                    if (exclusionReason == "class ID")
                    {
                        missingClassIdCount++;
                    }
                    else if (exclusionReason == "localized name")
                    {
                        missingNameCount++;
                    }
                    else if (exclusionReason == "registered icon")
                    {
                        missingRegisteredIconCount++;
                    }
                    else
                    {
                        exceptionCount++;
                    }

                    if (exclusionSamples.Length < 1800)
                    {
                        exclusionSamples.Append(techType);
                        exclusionSamples.Append(" [");
                        exclusionSamples.Append(exclusionReason);
                        exclusionSamples.Append("], ");
                    }

                    continue;
                }

                GetTechTypeSource(techType, out bool isModded);
                if (!isModded && VanillaTechTypeExclusions.Contains(techType))
                {
                    vanillaExclusionCount++;
                    continue;
                }

                candidateTechTypes.Add(techType);
            }

            ModDebugLog.LogInfo(
                "TechType picker catalogue: enumerated=" + values.Length +
                ", metadata candidates=" + candidateTechTypes.Count +
                ", none=" + noneCount +
                ", duplicates=" + duplicateCount +
                ", already assigned=" + assignedCount +
                ", missing class ID=" + missingClassIdCount +
                ", missing localized name=" + missingNameCount +
                ", missing registered icon=" + missingRegisteredIconCount +
                ", known vanilla non-pickupable=" + vanillaExclusionCount +
                ", exceptions=" + exceptionCount + ".");
            ModDebugLog.LogDebug("TechType picker exclusion samples: " + exclusionSamples);
            LogTechTypeProbe(TechType.Titanium);
            LogTechTypeProbe(TechType.Copper);
            LogTechTypeProbe(TechType.Battery);
        }

        private void RefreshTechTypePicker(string searchText)
        {
            ClearEntries(pickerEntries);
            string normalizedSearch = string.IsNullOrWhiteSpace(searchText)
                ? string.Empty
                : searchText.Trim();
            foreach (TechType techType in availableTechTypes)
            {
                string displayName = GetTechTypeDisplayName(techType);
                string sourceName = GetTechTypeSource(techType, out bool isModded);
                if (pickerCustomOnlyToggle.isOn && !isModded)
                {
                    continue;
                }

                if (normalizedSearch.Length > 0 &&
                    displayName.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) < 0 &&
                    sourceName.IndexOf(normalizedSearch, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                TechTypePickerEntry entry = Instantiate(pickerEntryPrefab, pickerContent);
                entry.gameObject.SetActive(true);
                entry.Bind(techType, displayName, sourceName, isModded, SelectTechType);
                pickerEntries.Add(entry);
            }

            pickerEmptyText.gameObject.SetActive(pickerEntries.Count == 0);
        }

        private static bool IsSelectableTechType(TechType techType, out string exclusionReason)
        {
            try
            {
                string classId = CraftData.GetClassIdForTechType(techType);
                string localizedName = Language.main == null ? string.Empty : Language.main.Get(techType);
                Sprite icon = SpriteManager.Get(techType);
                if (string.IsNullOrWhiteSpace(classId))
                {
                    exclusionReason = "class ID";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(localizedName))
                {
                    exclusionReason = "localized name";
                    return false;
                }

                if (icon == null || icon == SpriteManager.defaultSprite)
                {
                    exclusionReason = "registered icon";
                    return false;
                }

                exclusionReason = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                exclusionReason = "exception: " + exception.GetType().Name;
                return false;
            }
        }

        private static void LogTechTypeProbe(TechType techType)
        {
            try
            {
                string techTypeName = techType.ToString();
                string classId = CraftData.GetClassIdForTechType(techType);
                string localizedName = Language.main == null ? string.Empty : Language.main.Get(techType);
                Sprite normalIcon = SpriteManager.Get(techType);
                Sprite registeredIcon = SpriteManager.Get(SpriteManager.Group.None, techTypeName, null);
                ModDebugLog.LogInfo(
                    "TechType picker probe " + techTypeName +
                    ": classId='" + classId +
                    "', localizedName='" + localizedName +
                    "', normalIcon='" + (normalIcon == null ? "null" : normalIcon.name) +
                    "', isDefaultIcon=" + (normalIcon == SpriteManager.defaultSprite) +
                    ", registeredIcon='" + (registeredIcon == null ? "null" : registeredIcon.name) + "'.");
            }
            catch (Exception exception)
            {
                ModDebugLog.LogError("TechType picker probe failed for " + techType + ": " + exception);
            }
        }

        private static int CompareTechTypeNames(TechType first, TechType second)
        {
            return string.Compare(
                GetTechTypeDisplayName(first),
                GetTechTypeDisplayName(second),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GetTechTypeDisplayName(TechType techType)
        {
            string techTypeName = techType.ToString();
            string localizedName = Language.main == null ? string.Empty : Language.main.Get(techType);
            return string.IsNullOrWhiteSpace(localizedName) || localizedName == techTypeName
                ? techTypeName
                : localizedName + "  (" + techTypeName + ")";
        }

        private static string GetTechTypeSource(TechType techType, out bool isModded)
        {
            Assembly ownerAssembly;
            if (!EnumHandler.TryGetOwnerAssembly(techType, out ownerAssembly))
            {
                isModded = false;
                return "Subnautica";
            }

            isModded = true;
            foreach (KeyValuePair<string, BepInEx.PluginInfo> plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Instance != null &&
                    plugin.Value.Instance.GetType().Assembly == ownerAssembly)
                {
                    return plugin.Value.Metadata.Name;
                }
            }

            return ownerAssembly.GetName().Name;
        }

        private void SelectCategory(int index)
        {
            selectedIndex = index;
            RefreshDetails();
            RefreshTechTypes();
            UpdateButtons();
        }

        private void Apply()
        {
            RenameSelectedCategory(categoryNameInput.text);

            CategoryService.ApplyDraft(draft);
            draft = CategoryService.CreateDraft();
            selectedIndex = Mathf.Clamp(selectedIndex, 0, draft.Count - 1);
            RefreshAll();
        }

        private void Done()
        {
            Apply();
            Hide();
        }

        private void Cancel()
        {
            Hide();
        }

        private void RestoreDefaults()
        {
            draft = CategoryService.CreateDefaultDraft();
            selectedIndex = draft.Count == 0 ? -1 : 0;
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshCategories();
            RefreshDetails();
            RefreshTechTypes();
            UpdateButtons();
        }

        private void RefreshCategories()
        {
            ClearEntries(categoryEntries);
            for (int index = 0; index < draft.Count; index++)
            {
                EditableCategory category = draft[index];
                CategoryListEntry entry = Instantiate(categoryEntryPrefab, categoryContent);
                entry.gameObject.SetActive(true);
                entry.Bind(index, category.DisplayName, GetStatus(category), SelectCategory);
                categoryEntries.Add(entry);
            }
        }

        private void RefreshDetails()
        {
            EditableCategory category = GetSelectedCategory();
            categoryNameInput.interactable = category != null;
            categoryNameInput.SetTextWithoutNotify(category == null ? string.Empty : category.DisplayName);
            categoryStatusText.text = category == null ? "No category selected" : GetStatus(category);
        }

        private void RefreshTechTypes()
        {
            ClearEntries(techTypeEntries);
            EditableCategory category = GetSelectedCategory();
            if (category == null)
            {
                return;
            }

            foreach (TechType techType in category.TechTypes)
            {
                string sourceName = GetTechTypeSource(techType, out bool isModded);
                TechTypeListEntry entry = Instantiate(techTypeEntryPrefab, techTypeContent);
                entry.gameObject.SetActive(true);
                entry.Bind(techType, sourceName, isModded, RemoveTechType);
                techTypeEntries.Add(entry);
            }
        }

        private void UpdateButtons()
        {
            bool hasSelection = GetSelectedCategory() != null;
            removeButton.interactable = hasSelection;
            moveUpButton.interactable = hasSelection && selectedIndex > 0;
            moveDownButton.interactable = hasSelection && selectedIndex < draft.Count - 1;
            addTechTypeButton.interactable = hasSelection;
        }

        private EditableCategory GetSelectedCategory()
        {
            return selectedIndex >= 0 && selectedIndex < draft.Count ? draft[selectedIndex] : null;
        }

        private static string GetStatus(EditableCategory category)
        {
            if (!category.IsBuiltIn)
            {
                return "CUSTOM";
            }

            return category.IsModified ? "MODIFIED" : "DEFAULT";
        }

        private static void ClearEntries<T>(List<T> entries) where T : Component
        {
            foreach (T entry in entries)
            {
                Destroy(entry.gameObject);
            }

            entries.Clear();
        }

        private void Hide()
        {
            CloseTechTypePicker();
            mainPanel.SetActive(false);
            SetParentInteraction(true);
            isShowing = false;
        }

        private void SetParentInteraction(bool state)
        {
            if (parentCanvasGroup != null)
            {
                parentCanvasGroup.interactable = state;
                parentCanvasGroup.blocksRaycasts = state;
            }
        }

        private static GameObject GetOptionsPanel()
        {
            IngameMenu inGameMenu = FindObjectOfType<IngameMenu>();
            if (inGameMenu != null)
            {
                Transform panel = inGameMenu.transform.Find(InGameMenuOptionsPanelPath);
                if (panel != null)
                {
                    return panel.gameObject;
                }
            }

            uGUI_MainMenu mainMenu = FindObjectOfType<uGUI_MainMenu>();
            if (mainMenu != null)
            {
                Transform panel = mainMenu.transform.Find(MainMenuOptionsPanelPath);
                if (panel != null)
                {
                    return panel.gameObject;
                }
            }

            return null;
        }
    }
}
