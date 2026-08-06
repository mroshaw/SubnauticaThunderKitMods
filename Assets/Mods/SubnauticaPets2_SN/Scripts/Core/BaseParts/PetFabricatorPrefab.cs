using DaftAppleGames.ModTools.Extensions;
using DaftAppleGames.SubnauticaPets.Pets;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.BaseParts
{
    /// <summary>
    /// Static class for creating the new Pet Fabricator
    /// </summary>
    internal static class PetFabricatorPrefab
    {
        // Pubic PrefabInfo, for anything that needs it
        public static PrefabInfo Info;
        private const string ClassId = "PetFabricator";
        private const string IconAssetName = "PetFabricatorIconTexture.png";
        private const string EncPath = "Tech/Habitats";
        private const string DatabankPopupImageAssetName = "PetFabricatorDataBankPopupImageTexture.png";
        private const string DatabankMainImageAssetName = "PetFabricatorDataBankMainImageTexture.png";
        
        /// <summary>
        /// Makes the new Pet Fabricator available for use.
        /// </summary>
        public static void Register()
        {
            // Unlock at start if in Creative mode
            Info = PrefabInfo
                .WithTechType(ClassId, null, null, unlockAtStart: false)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(IconAssetName) as Sprite);

            CustomPrefab fabricatorPrefab = new CustomPrefab(Info);

            FabricatorGadget fabGadget = fabricatorPrefab.CreateFabricator(out CraftTree.Type treeType)
                .AddCraftNode(AlienRobotPrefab.Info.TechType)
                .AddCraftNode(BloodCrawlerPrefab.Info.TechType)
                .AddCraftNode(CaveCrawlerPrefab.Info.TechType)
                .AddCraftNode(CrabSquidPrefab.Info.TechType);

            // If enabled, add the "bonus pets" to the fabricator
            if (ConfigFile.EnableBonusPets)
            {
                fabGadget
                .AddCraftNode(CustomPetPrefabs.CatPetPrefab.Info.TechType)
                .AddCraftNode(CustomPetPrefabs.DogPetPrefab.Info.TechType)
                .AddCraftNode(CustomPetPrefabs.RabbitPetPrefab.Info.TechType)
                .AddCraftNode(CustomPetPrefabs.SealPetPrefab.Info.TechType)
                .AddCraftNode(CustomPetPrefabs.WalrusPetPrefab.Info.TechType)
                .AddCraftNode(CustomPetPrefabs.FoxPetPrefab.Info.TechType);
            }

            FabricatorTemplate fabPrefab = new FabricatorTemplate(Info, treeType)
            {
                FabricatorModel = FabricatorTemplate.Model.Workbench,
                ModifyPrefab = obj =>
                {
                    obj.SetActive(false);
                    obj.AddComponent<PetFabricator>();
                    obj.ApplyNewMeshTexture("PetFabricatorTexture", "", ModAssetUtils);
                    obj.SetActive(false);
                }
            };

            fabricatorPrefab.SetGameObject(fabPrefab);

            // Define the recipe for the new Fabricator, depends on whether in "Adventure" or "Creative" mode.
            RecipeData recipe = null;
            if (ConfigFile.ModMode == ModMode.Adventure)
            {
                recipe = new RecipeData(
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.CopperWire, 2),
                    new Ingredient(PetDnaPrefabs.CrabSquidDnaPrefab.Info.TechType, 1),
                    new Ingredient(PetDnaPrefabs.AlienRobotDnaPrefab.Info.TechType, 1),
                    new Ingredient(PetDnaPrefabs.BloodCrawlerDnaPrefab.Info.TechType, 1),
                    new Ingredient(PetDnaPrefabs.CaveCrawlerDnaPrefab.Info.TechType, 1));
            }
            else
            {
                // Only costs 1 titanium in "Easy" mode
                recipe = new RecipeData(new Ingredient(TechType.Titanium, 1));
            }

            // Set the recipe
            fabricatorPrefab.SetRecipe(recipe);

            // Set up the scanning and fragment unlocks
            fabricatorPrefab.SetUnlock(Info.TechType, 3)
                .WithPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule)
                .WithAnalysisTech(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(DatabankPopupImageAssetName) as Sprite, null,
                    null)
                .WithEncyclopediaEntry(EncPath,
                    ModAssetUtils.GetObjectFromAssetBundle<Sprite>(DatabankPopupImageAssetName) as Sprite,
                    ModAssetUtils.GetObjectFromAssetBundle<Texture2D>(DatabankMainImageAssetName) as Texture2D);
            fabricatorPrefab.Register();
        }
    }
}
