using System.Collections.Generic;
using Nautilus.Json;
using Nautilus.Json.Attributes;

namespace DaftAppleGames.AutoLockerLabels_SN
{
    /// <summary>
    /// Stores the lockers that have automatic labelling enabled in the
    /// current save slot.
    /// </summary>
    [FileName("auto_locker_labels")]
    internal sealed class AutoLockerLabelSaveData : SaveDataCache
    {
        private HashSet<string> AutomaticLockerIds { get; set; } =
            new HashSet<string>();
        
        internal bool IsAutomatic(string lockerId)
        {
            return !string.IsNullOrWhiteSpace(lockerId) &&
                   AutomaticLockerIds.Contains(lockerId);
        }

        internal void EnableAutomatic(string lockerId)
        {
            if (string.IsNullOrWhiteSpace(lockerId))
            {
                return;
            }

            AutomaticLockerIds.Add(lockerId);
        }

        internal void DisableAutomatic(string lockerId)
        {
            if (string.IsNullOrWhiteSpace(lockerId))
            {
                return;
            }

            AutomaticLockerIds.Remove(lockerId);
        }
    }
}