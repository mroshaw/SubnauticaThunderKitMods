using DaftAppleGames.MoreAquariums;
using HarmonyLib;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(Player __instance)
        {
            ModDebugLog.LogDebug("Adding BuilderHelper to player...");
            __instance.gameObject.AddComponent<BuilderHelper>();
        }
    }
}
