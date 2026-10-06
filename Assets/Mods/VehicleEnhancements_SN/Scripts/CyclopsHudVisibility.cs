using UnityEngine;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal class CyclopsHudVisibility : MonoBehaviour
    {
        private GameObject controllerHost;
        private SubRoot cyclops;

        internal void Configure(GameObject enhancedControllerHost, SubRoot cyclopsSubRoot)
        {
            controllerHost = enhancedControllerHost;
            cyclops = cyclopsSubRoot;
        }

        private void Update()
        {
            if (!controllerHost)
            {
                return;
            }

            Player player = Player.main;
            bool shouldRunControllers = player && player.currentSub == cyclops;
            if (controllerHost.activeSelf != shouldRunControllers)
            {
                controllerHost.SetActive(shouldRunControllers);
            }
        }
    }
}
