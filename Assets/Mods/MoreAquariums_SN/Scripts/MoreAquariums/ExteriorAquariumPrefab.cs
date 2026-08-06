using System;
using DaftAppleGames.ModTools.Extensions;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums
{
    /// <summary>
    /// This class can be inherited by Aquarium prefab classes that provide a full model prefab.
    /// This means the prefab provided by prefabAsset is a "fully configured" aquarium, that will replace
    /// the "model" in the cloneTechType prefab.
    /// For example, use this to clone Base Piece prefabs that behave like Base Pieces, but have the Aquarium
    /// functionality overlayed.
    /// </summary>
    public abstract class ExteriorAquariumPrefab
    {
        internal static PrefabInfo RegisterBase(string classId, string displayName, string description,
            string iconAssetName, string prefabAssetName, RecipeData recipeData,
            TechType cloneTechType, TechGroup techGroup, TechCategory techCategory,
            AquariumType aquariumType, Action<GameObject> postConfigAction = null)
        {
            PrefabInfo info = PrefabInfo
                .WithTechType(classId, displayName, description, unlockAtStart: true)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(iconAssetName) as Sprite);
            CustomPrefab aquariumPrefab = new CustomPrefab(info);
            
            // Clone the existing Aquarium
            PrefabTemplate aquariumTemplate = new CloneTemplate(aquariumPrefab.Info, cloneTechType)
            {
                // Reconfigure the prefab, once it's been created
                ModifyPrefab = go => TagGhostBase(go, aquariumType, prefabAssetName, postConfigAction)
            };
            
            // Set the recipe, unlock and register
            aquariumPrefab.SetGameObject(aquariumTemplate);
            aquariumPrefab.SetRecipe(recipeData);
            aquariumPrefab.SetUnlock(TechType.Aquarium)
                .WithPdaGroupCategory(techGroup, techCategory);
            aquariumPrefab.Register();
            ModDebugLog.LogDebug($"{displayName} registered successfully!");

            return info;
        }

        /// <summary>
        /// Tags the GhostBase so that we can manipulate the Ghost and Base prefabs when they are spawned in
        /// the Base class
        /// </summary>
        private static void TagGhostBase(GameObject prefabGameObject, AquariumType aquariumType, string prefabAssetName, Action<GameObject> postConfigAction)
        {
            ModDebugLog.LogDebug($"Tagging: {prefabGameObject.name} with CustomAquarium type: {aquariumType}");
            Base baseComponent = prefabGameObject.GetComponentInChildren<Base>();
            if (!baseComponent)
            {
                ModDebugLog.LogError($"No Base component found on {prefabGameObject.name}! ABORTING!");
                return;
            }
            
            if (baseComponent)
            {
                CustomAquarium customAquarium = baseComponent.gameObject.AddComponent<CustomAquarium>();
                customAquarium.SetAquariumType(aquariumType);
                customAquarium.SetPrefabName(prefabAssetName);
            }
            postConfigAction?.Invoke(prefabGameObject);
        }
    }
}