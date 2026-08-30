using DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels;
using HarmonyLib;
using LockerLabel.Patches;
using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.Patches
{
    [HarmonyPatch(typeof(StorageContainer))]
    internal static class StorageContainerPatches
    {
        /// <summary>
        /// Adds an AutoLockerLabelController to storage lockers
        /// </summary>
        [HarmonyPatch(nameof(StorageContainer.Awake))]
        [HarmonyAfter(LockerLabelModGuid)]
        [HarmonyPostfix]
        private static void AwakePostFix(StorageContainer __instance)
        {
            // Reuse the check from the LockerLabel mod
            if (StorageContainerPatch.ShouldHandle(__instance))
            {
                __instance.gameObject.EnsureComponent<FreeStandingController>();
                ModDebugLog.LogDebug($"LockerLabelModLabelController added to storage container on: '{__instance.name}'. ");
                return;
            }

            if (CraftData.GetTechType(__instance.gameObject) == TechType.SmallLocker)
            {
                __instance.gameObject.EnsureComponent<WallController>();
                ModDebugLog.LogDebug($"WallLockerLabelController added to storage container on: '{__instance.name}'. ");
            }
        }
        
        [HarmonyPatch(nameof(StorageContainer.OnHandClick))]
        [HarmonyPriority(Priority.First)]
        [HarmonyBefore(LockerLabelModGuid)]
        [HarmonyPrefix]
        private static bool OnHandClickPrefix(
            StorageContainer __instance)
        {
            if (!AutoLabelInput.IsToggleModifierPressed() || !__instance.TryGetComponent<AutoLabelController>(out AutoLabelController controller))
            {
                return true;
            }
            
            controller.ToggleAutomatic();

            string message = controller.IsAutomatic
                ? "Automatic locker label enabled"
                : "Automatic locker label disabled";

            ErrorMessage.AddMessage(message);
            return false;
        }
        
        /// <summary>
        /// Add our 'toggle automatic' prompt to the hand target UI 
        /// </summary>
        [HarmonyPatch(nameof(StorageContainer.OnHandHover))]
        [HarmonyAfter(LockerLabelModGuid)]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        private static void OnHandHoverPostfix(
            StorageContainer __instance)
        {
            if (!AutoLabelInput.IsToggleModifierPressed() || !__instance.TryGetComponent<AutoLabelController>(
                    out AutoLabelController controller))
            {
                return;
            }

            string prompt = controller.IsAutomatic ? "Disable automatic label" : "Enable automatic label";

            HandReticle.main.SetText(HandReticle.TextType.Hand, prompt, false, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
            HandReticle.main.SetIcon(HandReticle.IconType.Rename, 1f);
        }
    }
}
