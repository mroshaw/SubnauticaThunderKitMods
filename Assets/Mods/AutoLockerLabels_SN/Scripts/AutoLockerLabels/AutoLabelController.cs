using UnityEngine;
using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    public abstract class AutoLabelController : MonoBehaviour
    {
        protected StorageContainer StorageContainer;
        private Constructable constructable;
        
        private bool isAutomatic;
        private string lastGeneratedLabel;
        private string lockerId;
        
        internal bool IsAutomatic => isAutomatic;
        
        internal virtual void Start()
        {
            StorageContainer = GetComponent<StorageContainer>();
            constructable = GetComponent<Constructable>();
            
            if (StorageContainer == null ||
                StorageContainer.container == null)
            {
                enabled = false;
                return;
            }
            
            // Get a unique Id for the locker for our save file
            lockerId = GetLockerId();
            
            // Check to see if this is a locker we've saved
            isAutomatic = AutoLockerLabelsPlugin.SaveData.IsAutomatic(lockerId);

            StorageContainer.container.onAddItem += OnItemAdded;
            StorageContainer.container.onRemoveItem += OnItemRemoved;
            
            if (isAutomatic)
            {
                ApplyAutomaticLabel();
            }
        }
        
        private void OnDestroy()
        {
            // If the locker is deconstructed, remove it from the save file
            if (constructable != null &&
                constructable.constructedAmount <= 0f &&
                !string.IsNullOrWhiteSpace(lockerId))
            {
                AutoLockerLabelsPlugin.SaveData.DisableAutomatic(lockerId);
            }
            
            if (StorageContainer == null|| StorageContainer.container == null)
            {
                return;
            }

            // Clean-up listeners
            StorageContainer.container.onAddItem -= OnItemAdded;
            StorageContainer.container.onRemoveItem -= OnItemRemoved;
        }

        protected abstract string GetLockerId();
        protected abstract void SetLabel(string newLabel);

        protected virtual bool IsValidLocker()
        {
            return StorageContainer != null && StorageContainer.container != null;
        }
        
        /// <summary>
        /// Hook into the storage container `AddItem`
        /// </summary>
        private void OnItemAdded(InventoryItem item)
        {
            if (isAutomatic)
            {
                ApplyAutomaticLabel();
            }
        }

        /// <summary>
        /// Hook into the storage container `RemoveItem`
        /// </summary>
        private void OnItemRemoved(InventoryItem item)
        {
            if (isAutomatic)
            {
                ApplyAutomaticLabel();
            }
        }
        
        /// <summary>
        /// Helper to update the save data
        /// </summary>
        internal void ToggleAutomatic()
        {
            if (isAutomatic)
            {
                DisableAutomatic();
                return;
            }

            EnableAutomatic();
        }
        
        /// <summary>
        /// Helper to update the save data
        /// </summary>
        internal void EnableAutomatic()
        {
            if (string.IsNullOrWhiteSpace(lockerId))
            {
                return;
            }

            AutoLockerLabelsPlugin.SaveData.EnableAutomatic(lockerId);

            isAutomatic = true;
            ApplyAutomaticLabel();
        }
        
        /// <summary>
        /// Helper to update the save data
        /// </summary>
        internal void DisableAutomatic()
        {
            if (string.IsNullOrWhiteSpace(lockerId))
            {
                return;
            }

            AutoLockerLabelsPlugin.SaveData.DisableAutomatic(lockerId);

            isAutomatic = false;
        }
        
        /// <summary>
        /// Applies the generated label to the locker, using the
        /// Locker Label 3rd party mod
        /// </summary>
        private void ApplyAutomaticLabel()
        {
            string newGeneratedLabel = LabelGenerator.Generate(StorageContainer.container).ToUpper();
            
            if (lastGeneratedLabel == newGeneratedLabel)
            {
                // Label has not changed
                return;
            }
            SetLabel(newGeneratedLabel);
            lastGeneratedLabel = newGeneratedLabel;
        }
        
    }
}

