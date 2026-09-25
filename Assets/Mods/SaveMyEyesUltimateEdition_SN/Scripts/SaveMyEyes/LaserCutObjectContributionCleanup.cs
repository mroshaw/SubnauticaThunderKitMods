using DaftAppleGames.SaveMyEyesUltimateEdition_SN.Patches;
using UnityEngine;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    internal sealed class LaserCutObjectContributionCleanup : MonoBehaviour
    {
        private LaserCutObject laserCutObject;

        internal void Initialize(LaserCutObject owner)
        {
            laserCutObject = owner;
        }

        private void OnDestroy()
        {
            LaserCutObjectPatches.UnregisterContribution(laserCutObject);
        }
    }
}
