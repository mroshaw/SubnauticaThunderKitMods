using LockerLabel.Components;
using UnityEngine;
using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    public class AutoLockerLabelController : MonoBehaviour
    {
        // Reference to the LocalLabelController component from the LockerLabel mod
        private LockerLabelController lockerLabelController;
        
        private StorageContainer storageContainer;
        private bool isAutomatic;
        private bool recalculationPending;
        private string lastGeneratedLabel;
        private string lockerId;
        
        internal bool IsAutomatic => isAutomatic;
        
        private void Start()
        {
            storageContainer = GetComponent<StorageContainer>();
            
            // get the LockerLabelController from the locker
            lockerLabelController = GetComponent<LockerLabelController>();
            enabled = lockerLabelController != null;
            
            if (storageContainer == null ||
                storageContainer.container == null ||
                lockerLabelController == null)
            {
                enabled = false;
                return;
            }
            
            // Use the LockerLabel generated Id for consistency
            lockerId = lockerLabelController.ComputeSaveId();
            
            // Check to see if this is a locker we've saved
            isAutomatic = SaveData.IsAutomatic(lockerId);

            storageContainer.container.onAddItem += OnItemAdded;
            storageContainer.container.onRemoveItem += OnItemRemoved;
            
            if (isAutomatic)
            {
                ApplyAutomaticLabel();
            }
        }
        
        private void OnDestroy()
        {
            if (storageContainer == null ||
                storageContainer.container == null)
            {
                return;
            }

            // Cleanup listeners
            storageContainer.container.onAddItem -= OnItemAdded;
            storageContainer.container.onRemoveItem -= OnItemRemoved;
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

            SaveData.EnableAutomatic(lockerId);

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

            SaveData.DisableAutomatic(lockerId);

            isAutomatic = false;
        }
        
        /// <summary>
        /// Applies the generated label to the locker, using the
        /// Locker Label 3rd party mod
        /// </summary>
        private void ApplyAutomaticLabel()
        {
            string newSeneratedLabel = "TEST";
            
            if (lastGeneratedLabel == newSeneratedLabel)
            {
                // Label has not changed
                return;
            }
            lockerLabelController.SetCustomLabel(newSeneratedLabel);
        }
    }
}

