using DaftAppleGames.ModTools.Extensions;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.Pets
{
    internal static class CaveCrawlerPrefab
    {
        private const string ClassId = "CaveCrawlerPet";
        private const string IconTextureAssetName = "CaveCrawlerIcon_Small.png";
        private const string CloneClassId = "3e0a11f1-e2b2-4c4f-9a8e-0b0a77dcc065";

        internal static PrefabInfo Info;

        internal static void Register()
        {
            Info = PrefabInfo.WithTechType(ClassId, null, null, unlockAtStart: true)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(IconTextureAssetName) as Sprite);
            CustomPrefab prefab = new CustomPrefab(Info);
            CloneTemplate cloneTemplate = new CloneTemplate(Info, CloneClassId);
            cloneTemplate.ModifyPrefab += obj =>
            {
                obj.SetActive(false);
                PetPrefabConfigUtils.AddTechTag(obj, Info.TechType);
                obj.DestroyComponentsInChildren<LargeWorldEntity>();
                GameObject modelGameObject = obj.GetComponentInChildren<Animator>(true).gameObject;
                PetPrefabConfigUtils.AddVFXFabricating(obj, null, -0.2f, 0.5f, Vector3.zero, 1.0f, Vector3.zero);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside, modelGameObject);
                PetPrefabConfigUtils.AddPetHandTarget(obj);
                obj.DestroyComponentsInChildren<Pickupable>();
                PetPrefabConfigUtils.ConfigureSkyApplier(obj);
                PetPrefabConfigUtils.ConfigureAnimator(obj, false);
                PetPrefabConfigUtils.AddPetComponent(obj);
                obj.name = ClassId;
                obj.SetActive(false);
                ModDebugLog.LogDebug($"Done modifying {Info.TechType}");
            };
            prefab.SetGameObject(cloneTemplate);
            RecipeData recipe = ConfigFile.ModMode == ModMode.Adventure
                ? new RecipeData(
                    new Ingredient(TechType.Gold, 1),
                    new Ingredient(TechType.Sulphur, 1),
                    new Ingredient(TechType.Salt, 1),
                    new Ingredient(PetDnaPrefabs.CaveCrawlerDnaPrefab.Info.TechType, 3))
                : new RecipeData(new Ingredient(TechType.Titanium, 1));
            prefab.SetRecipe(recipe);
            prefab.Register();
        }
    }
}
