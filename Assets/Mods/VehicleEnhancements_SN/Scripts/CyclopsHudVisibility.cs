using UnityEngine;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal class CyclopsHudVisibility : MonoBehaviour
    {
        private GameObject indicators;

        internal void Configure(GameObject enhancedIndicators)
        {
            indicators = enhancedIndicators;
        }

        private void Update()
        {
            if (!indicators)
            {
                return;
            }

            Player player = Player.main;
            SubRoot sub = player ? player.currentSub : null;
            bool visible = sub && sub.isCyclops && player.isPiloting && !player.GetVehicle();
            if (indicators.activeSelf != visible)
            {
                indicators.SetActive(visible);
            }
        }
    }
}
