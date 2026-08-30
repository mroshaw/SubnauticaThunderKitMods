using LockerLabel.Components;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Handles setting automatic labels on the "LockerLabel" modded freestanding lockers
    /// </summary>
    public class FreeStandingController : AutoLabelController
    {
        // Reference to the LocalLabelController component from the LockerLabel mod
        private LockerLabelController lockerLabelController;
        
        internal override void Start()
        {
            // get the LockerLabelController from the locker
            lockerLabelController = GetComponent<LockerLabelController>();
            base.Start();
        }

        protected override string GetLockerId()
        {
            return lockerLabelController.ComputeSaveId();
        }

        protected override void SetLabel(string newLabel)
        {
            lockerLabelController.SetCustomLabel(newLabel);
        }

        protected override bool IsValidLocker()
        {
            return base.IsValidLocker() && lockerLabelController != null;
        }
    }
}

