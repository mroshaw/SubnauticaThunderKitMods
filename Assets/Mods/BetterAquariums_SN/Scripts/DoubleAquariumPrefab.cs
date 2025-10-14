using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using static DaftAppleGames.BetterAquariums_SN.BetterAquariumsPlugin;

namespace DaftAppleGames.BetterAquariums_SN
{
    /// <summary>
    /// Static class for creating the new Pet Console
    /// </summary>
    internal static class DoubleAquariumPrefab
    {
        internal static PrefabInfo Info;
        private const string ClassId = "DoubleAquarium";
        private const TechType CloneTechType= TechType.Aquarium;
        private const string IconAssetName = "DoubleAquariumIcon.png";
        private const string ModelPrefabAssetName = "DoubleAquariumPrefab.prefab";
        
        /// <summary>
        /// Register the new prefab
        /// </summary>
        internal static void Register()
        {
            Info = PrefabInfo
                .WithTechType(ClassId, "Double Aquarium", "A double sized aquarium for use in long rooms", unlockAtStart: true)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(IconAssetName) as Sprite);
            CustomPrefab aquariumPrefab = new CustomPrefab(Info);

            // Clone the existing Aquarium
            PrefabTemplate aquariumTemplate = new CloneTemplate(aquariumPrefab.Info, CloneTechType)
            {
                // Reconfigure the prefab, once it's been created
                ModifyPrefab = ConfigurePrefab
            };
            
            // Define the recipe for the new aquarium
            var recipe = new RecipeData(
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.CopperWire, 2),
                    new Ingredient(TechType.Glass, 5));

            // Set the recipe, unlock and register
            aquariumPrefab.SetGameObject(aquariumTemplate);
            aquariumPrefab.SetRecipe(recipe);
            aquariumPrefab.SetUnlock(TechType.Aquarium)
                .WithPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            aquariumPrefab.Register();
            ModDebugLog.LogDebug("Double Aquarium registered successfully!");
        }

        /// <summary>
        /// Configure the new prefab, using the new model asset
        /// </summary>
        private static void ConfigurePrefab(GameObject prefabGameObject)
        {
            // Get new model from the asset bundle
            GameObject newModelInstance =
                ModAssetUtils.GetPrefabInstanceFromAssetBundle(ModelPrefabAssetName, false);

            // Call the helper to replace and reconfigure the prefab
            BetterAquariumHelper helper = newModelInstance.GetComponent<BetterAquariumHelper>();
            helper.ConfigureAquariumPrefab(prefabGameObject);
            
            newModelInstance.SetActive(true);
        }
    }
}