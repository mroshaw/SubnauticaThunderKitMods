using HarmonyLib;
using UnityEngine;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums.Patches
{
    /// <summary>
    /// Patch in the Player methods
    /// </summary>
    [HarmonyPatch(typeof(Base))] public class BasePatches
    {
        [HarmonyPatch(nameof(Base.SpawnPiece), typeof(Base.Piece), typeof(Int3), typeof(Vector3), typeof(Quaternion), typeof(Base.Direction), typeof(BaseDeconstructable))]
        [HarmonyPostfix]
        public static void SpawnPiece_Postfix(Base __instance, Base.Piece piece,  Int3 cell,  Vector3 position, Quaternion rotation, Base.Direction? faceDirection, BaseDeconstructable sourceBaseDeconstructable, ref Transform __result)
        {
            // Keep debugging log down
            if (piece != Base.Piece.Observatory)
            {
                return;
            }
            
            bool isResultCustomAquarium = __result.gameObject.TryGetComponent<CustomAquarium>(out CustomAquarium resultCustomAquarium);
            bool isBaseCustomAquarium = __instance.gameObject.TryGetComponent<CustomAquarium>(out CustomAquarium baseCustomAquarium);
            
            ModDebugLog.LogDebug($"SpawnPiece_Postfix called. IsGhost: {__instance.isGhost}, Piece: {piece}, Result.isCustomAquarium: {isResultCustomAquarium}, Base.isCustomAquarium: {isBaseCustomAquarium}");
            
            // Patch the Ghost prefab - here we want to replace the model only
            if (__instance.isGhost && isBaseCustomAquarium)
            {
                ModDebugLog.LogDebug($"SpawnPiece_Prefix: Checking patch state of Ghost prefab...");
                
                // Check if this is a piece that should be patched, and patch it with the appropriate AquariumType
                bool ghostPatched = BaseGhostPatches.HasGhostBeenPatched(piece);
                if (ghostPatched)
                {
                    ModDebugLog.LogDebug($"SpawnPiece_Prefix: Ghost prefab for Piece: {piece} has already been patched");
                    return;
                }
                AquariumType newAquariumType = BaseGhostPatches.GetAquariumType(piece);

                // Check if there's anything to patch
                if (newAquariumType == AquariumType.None)
                {
                    ModDebugLog.LogDebug($"SpawnPiece_Prefix: No ghost prefab patching request found  for Piece: {piece}.");
                    return;
                }
                
                ModDebugLog.LogDebug($"SpawnPiece_Prefix: Patching Ghost prefab: {newAquariumType} to: {__result.gameObject.name}");
                CustomAquarium newCustomAquarium = __result.gameObject.EnsureComponent<CustomAquarium>();
                newCustomAquarium.SetAquariumType(newAquariumType);
                newCustomAquarium.SetPrefabName(resultCustomAquarium.PrefabName);
                newCustomAquarium.SetIsGhost(true);
                BaseGhostPatches.SetGhostPatched(newAquariumType);
            }

            // Patch the Base prefab - here we want to add a new component that will reconfigure the base piece of Awake
            if (!__instance.isGhost)
            {
                ModDebugLog.LogDebug($"SpawnPiece_Prefix: Checking patch state of Base prefab...");
                
                bool basePatched = BaseGhostPatches.HasBaseBeenPatched(piece);
                if (basePatched)
                {
                    ModDebugLog.LogDebug($"SpawnPiece_Prefix: Base prefab for Piece: {piece} has already been patched");
                    return;
                }
                AquariumType newAquariumType = BaseGhostPatches.GetAquariumType(piece);

                // Check if there's anything to patch
                if (newAquariumType == AquariumType.None)
                {
                    ModDebugLog.LogDebug($"SpawnPiece_Prefix: No base prefab patching request found  for Piece: {piece}.");
                    return;
                }
                
                ModDebugLog.LogDebug($"SpawnPiece_Prefix: Patching Base prefab: {newAquariumType} to: {__result.gameObject.name}");
                CustomAquarium newCustomAquarium = __result.gameObject.EnsureComponent<CustomAquarium>();
                newCustomAquarium.SetAquariumType(newAquariumType);
                newCustomAquarium.SetPrefabName(resultCustomAquarium.PrefabName);
                newCustomAquarium.SetIsGhost(false);
                BaseGhostPatches.SetBasePatched(newAquariumType);
            }
        }
    }
}