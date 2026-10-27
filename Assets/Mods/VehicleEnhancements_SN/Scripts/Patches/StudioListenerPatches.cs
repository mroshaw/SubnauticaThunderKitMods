using FMODUnity;
using HarmonyLib;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Patches
{
    [HarmonyPatch(typeof(StudioListener))]
    internal static class StudioListenerPatches
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyPostfix]
        private static void OnEnablePostfix()
        {
            // The native method has registered the listener before custom sounds are created.
            RegisterReversingSounds();
        }
    }
}
