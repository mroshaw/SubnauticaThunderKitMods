using HarmonyLib;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// Patch in the BaseAddCellGhost to flag when a custom aquarium has been completely built
    /// </summary>
    [HarmonyPatch(typeof(BaseGhost))]
    internal class BaseGhostPatches
    {
        private static readonly AquariumPieceMappingList CustomAquariumPatchList = new AquariumPieceMappingList();
        
        [HarmonyPatch(nameof(BaseGhost.Finish))]
        [HarmonyPrefix]
        public static void Finish_Prefix(BaseGhost __instance)
        {
            ModDebugLog.LogDebug($"BaseGhost.Finish Prefix...");
            bool isCustomAquarium = __instance.TryGetComponent(out CustomAquarium ghostCustomAquarium);
            if (isCustomAquarium)
            {
                ModDebugLog.LogDebug($"Adding Patch Mapping for: {ghostCustomAquarium.AquariumType}...");
                CustomAquarium newCustomerAquarium = __instance.ghostBase.gameObject.EnsureComponent<CustomAquarium>();
                
                // Otherwise, set this up to be patched in BasePatches.SpawnPiece_Postfix
                switch (ghostCustomAquarium.AquariumType)
                {
                    case AquariumType.Observatory:
                        CustomAquariumPatchList.AddAquariumPieceMapping(ghostCustomAquarium.AquariumType, Base.Piece.Observatory);
                        newCustomerAquarium.SetAquariumType(ghostCustomAquarium.AquariumType);
                        break;
                }
            }
        }

        internal static AquariumType GetAquariumType(Base.Piece basePiece)
        {
            return CustomAquariumPatchList.GetAquariumType(basePiece);
        }

        /// <summary>
        /// Set the Ghost prefab as patched
        /// </summary>
        internal static void SetGhostPatched(AquariumType aquariumType)
        {
            CustomAquariumPatchList.SetGhostPatched(aquariumType);
        }

        /// <summary>
        /// Sets the Base prefab as patched
        /// </summary>
        internal static void SetBasePatched(AquariumType aquariumType)
        {
            CustomAquariumPatchList.SetBasePatched(aquariumType);
        }
        
        /// <summary>
        /// Returns true if the Ghost prefab has been patched
        /// </summary>
        internal static bool HasGhostBeenPatched(Base.Piece basePiece)
        {
            return CustomAquariumPatchList.IsGhostPatched(basePiece);
        }

        /// <summary>
        /// Returns true if the Base prefab has been patched
        /// </summary>
        internal static bool HasBaseBeenPatched(Base.Piece basePiece)
        {
            return CustomAquariumPatchList.IsBasePatched(basePiece);
        }
    }
}
