using static DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabelsPlugin;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    /// <summary>
    /// Handles automatic labels on the standard "Wall Locker"
    /// </summary>
    public class WallController : AutoLabelController
    {
        private ColoredLabel coloredLabel;
        
        internal override void Start()
        {
            coloredLabel = GetComponentInChildren<ColoredLabel>(true);
            base.Start();
        }

        protected override string GetLockerId()
        {
            PrefabIdentifier prefabIdentifier = StorageContainer.GetComponent<PrefabIdentifier>();
            return prefabIdentifier.Id;
        }

        protected override void SetLabel(string newLabel)
        {
            coloredLabel.signInput.text = newLabel;
        }
    }
}

