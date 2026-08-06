using Nautilus.Assets;
using Nautilus.Crafting;
using UnityEngine;

namespace DaftAppleGames.MoreAquariums
{
    /// <summary>
    /// Describes a Double Aquarium prefab
    /// </summary>
    public class ObservatoryAquariumPrefab : ExteriorAquariumPrefab
    {
        public static PrefabInfo PrefabInfo;
        
        /// Properties of the aquarium
        private const string ClassId = "ObservatoryAquarium";
        private const string DisplayName = "Observatory Aquarium";
        private const string Description = "A huge external aquarium that uses the structure of an observatory to house a large number of fish.";
        private const string IconAssetName = "ObservatoryAquariumIcon.png";
        private const string PrefabAssetName = "ObservatoryAquariumPrefab.prefab";
        private const AquariumType AquariumType = MoreAquariums.AquariumType.Observatory;
        
        private const TechType CloneTechType = TechType.BaseObservatory;
        private static readonly TechGroup TechGroup = TechGroup.BasePieces;
        private static readonly TechCategory TechCategory = TechCategory.BasePiece;
        
        // Recipe for the builder
        private static readonly RecipeData RecipeData = new RecipeData(
            new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.CopperWire, 1),
            new Ingredient(TechType.Glass, 4));
        
        // Register the new prefab
        public static void Register() => PrefabInfo = RegisterBase(ClassId, DisplayName, Description, IconAssetName, PrefabAssetName,
            RecipeData, CloneTechType, TechGroup, TechCategory, AquariumType,  CleanupPrefab);

        /// <summary>
        /// Perform clean up actions once the prefab has been configured
        /// </summary>
        private static void CleanupPrefab(GameObject prefabGameObject)
        {
            
        }
    }
}