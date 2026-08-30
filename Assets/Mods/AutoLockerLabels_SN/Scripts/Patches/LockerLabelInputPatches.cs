using DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels;
using HarmonyLib;
using LockerLabel.Components;

namespace DaftAppleGames.AutoLockerLabels_SN.Patches
{
    [HarmonyPatch(typeof(LockerLabelInput))] internal static class LockerLabelInputPatches
    {
        /// <summary>
        /// Prevent LockerLabels from reading modifiers when our toggle modifiers are pressed
        /// </summary>
        [HarmonyPatch(nameof(LockerLabelInput.IsRenameModifierPressed))]
        [HarmonyPrefix]
        private static bool IsRenameModifierPressedPrefix(ref bool __result)
        {
            if (!AutoLockerLabelInput.IsToggleModifierPressed())
            {
                return true;
            }

            __result = false;
            return false;
        }
    }
}