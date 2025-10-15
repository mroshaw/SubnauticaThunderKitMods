using Nautilus.Crafting;

namespace DaftAppleGames.BetterAquariums_SN
{
    public class DoubleAquarium : BetterAquarium
    {
        // Non-static properties of the aquarium component
        public override int StorageHeight => 4;
        public override int StorageWidth => 4;
        
        // Properties of the Aquarium
        private static readonly PrefabData Data = new PrefabData
        {
            ClassId = "DoubleAquarium",
            DisplayName = "Double Aquarium",
            Description = "A double-sized aquarium for use in long rooms.",
            IconAssetName = "DoubleAquariumIcon.png",
            PrefabAssetName = "DoubleAquariumPrefab.prefab",
            AquariumType = BetterAquariumType.Double,
            
            // Recipe for the builder
            Recipe = new RecipeData(
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.CopperWire, 2),
                new Ingredient(TechType.Glass, 5)),
        };
        
        public static void Register() => RegisterInternal(Data);
    }
}