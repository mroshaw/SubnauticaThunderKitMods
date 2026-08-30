using UnityEngine;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Mimick LockerLabel, but adds an additional modifier
    /// </summary>
    internal static class AutoLockerLabelInput
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