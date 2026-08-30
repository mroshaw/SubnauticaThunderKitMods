using UnityEngine;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Mimic LockerLabel, but adds an additional modifier.
    /// So shift + alt + click on locker
    /// </summary>
    internal static class AutoLabelInput
    {
        internal static bool IsToggleModifierPressed()
        {
            bool controlPressed =
                Input.GetKey(KeyCode.LeftAlt) ||
                Input.GetKey(KeyCode.RightAlt);

            bool shiftPressed =
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift);

            return controlPressed && shiftPressed;
        }
    }
}