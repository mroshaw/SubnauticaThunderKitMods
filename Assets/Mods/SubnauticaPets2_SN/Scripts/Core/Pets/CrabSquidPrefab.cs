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
    internal static class CrabSquidPrefab
    {
        private const string ClassId = "CrabSquidPet";
        private const string IconTextureAssetName = "CrabSquidIcon_Small.png";
        private const string CloneClassId = "4c2808fe-e051-44d2-8e64-120ddcdc8abb";

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
                PetPrefabConfigUtils.AddVFXFabricating(obj, null, -0.2f, 1.2f, Vector3.zero, 0.07f, Vector3.zero);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside, modelGameObject);
                obj.DestroyComponentsInChildren<Pickupable>();
                obj.DestroyComponentsInChildren<EMPAttack>();
                obj.DestroyComponentsInChildren<AggressiveWhenSeeTarget>();
                obj.DestroyComponentsInChildren<MeleeAttack>();
                PetPrefabConfigUtils.AddPetHandTarget(obj);
                PetPrefabConfigUtils.ConfigureSkyApplier(obj);
                PetPrefabConfigUtils.ConfigureAnimator(obj, false);
                PetPrefabConfigUtils.AddScaleOnStart(obj, 0.07f);
                PetPrefabConfigUtils.AddPetComponent(obj);
                obj.name = ClassId;
                obj.SetActive(false);
                ModDebugLog.LogDebug($"Done modifying {Info.TechType}");
            };
            prefab.SetGameObject(cloneTemplate);
            RecipeData recipe = ConfigFile.ModMode == ModMode.Adventure
                ? new RecipeData(
                    new Ingredient(TechType.Gold, 1),
                    new Ingredient(TechType.JellyPlant, 1),
                    new Ingredient(TechType.Salt, 1),
                    new Ingredient(PetDnaPrefabs.CrabSquidDnaPrefab.Info.TechType, 3))
                : new RecipeData(new Ingredient(TechType.Titanium, 1));
            prefab.SetRecipe(recipe);
            prefab.Register();
        }
    }
}
