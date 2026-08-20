using UnityEngine;
using UnityEngine.InputSystem;
using static DaftAppleGames.CuddlefishRecall_SN.CuddlefishRecallPlugin;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    /// <summary>
    /// Simple helper MonoBehaviour to monitor for Keyboard Input
    /// </summary>
    internal class CreatureRecallerInputManager : MonoBehaviour
    {
        private CreatureRecaller _creatureRecaller;
        
        private void Start()
        {
            ModDebugLog.LogDebug("Getting CreatureRecaller...");
            _creatureRecaller = GetComponent<CreatureRecaller>();
            ModDebugLog.LogDebug(_creatureRecaller ? "CreatureRecaller found." : "CreatureRecaller not found!");
        }

        /// <summary>
        /// Check for keyboard input and act accordingly.
        /// </summary>
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            bool controlPressedThisFrame = keyboard.leftCtrlKey.wasPressedThisFrame ||
                                           keyboard.rightCtrlKey.wasPressedThisFrame;

            bool recallButtonPressed = GameInput.GetButtonDown(_recallButton);
            if (!recallButtonPressed || !keyboard.ctrlKey.isPressed || controlPressedThisFrame)
            {
                return;
            }

            if (keyboard.shiftKey.isPressed)
            {
                ModDebugLog.LogDebug("Cancel recall keypress detected...");
                _creatureRecaller.CancelAllActiveRecalls();
                return;
            }

            if (!Player.main.IsUnderwaterForSwimming())
            {
                ErrorMessage.AddMessage("Cuddlefish recall is only available while underwater.");
                return;
            }

            ModDebugLog.LogDebug("Recall keypress detected...");
            _creatureRecaller.RecallAllCreatures();
            ModDebugLog.LogDebug("All creatures recalled!");
        }
    }
}
