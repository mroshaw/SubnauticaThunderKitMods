using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    /// <summary>
    /// Static class for creating the new Pet Console
    /// </summary>
    internal static class CuddleCamMonitorPrefab
    {
        internal static PrefabInfo Info;
        private const string ClassId = "CuddleCamMonitor";
        private const string IconAssetName = "CuddleCamMonitorIcon.png";
        private const string CuddleCamMonitorPrefabName = "CuddleCamMonitor.prefab";

        /// <summary>
        /// Register the Prefab
        /// </summary>
        internal static void Register()
        {
            Info = PrefabInfo
                .WithTechType(ClassId, "CuddleCam Monitor", "A monitor that displays images beamed from an active Cuddlefish 'CuddleCam' camera.", unlockAtStart: true)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(IconAssetName) as Sprite);

            RecipeData recipe = new RecipeData(
                new Ingredient(TechType.Titanium, 1),
                new Ingredient(TechType.ComputerChip, 1),
                new Ingredient(TechType.CopperWire, 2),
                new Ingredient(TechType.Glass, 1));
            
            CustomPrefab cuddleMonitorPrefab = new CustomPrefab(Info);
            cuddleMonitorPrefab.SetRecipe(recipe);
            cuddleMonitorPrefab
                .SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            
            GameObject prefabGameObject =
                ModAssetUtils.GetObjectFromAssetBundle<GameObject>(CuddleCamMonitorPrefabName) as GameObject;

            CuddleCamConfigurator configurator = prefabGameObject.GetComponent<CuddleCamConfigurator>();
            
            PrefabUtils.AddBasicComponents(prefabGameObject, ClassId, Info.TechType, LargeWorldEntity.CellLevel.Medium);
            Constructable constructable = PrefabUtils.AddConstructable(
                prefabGameObject,
                Info.TechType,
                ConstructableFlags.Base | ConstructableFlags.Wall,
                configurator.model);
            
            MaterialUtils.ApplySNShaders(configurator.model);
            
            cuddleMonitorPrefab.SetGameObject(prefabGameObject);
            
            cuddleMonitorPrefab.Register();
        }
        
        private static void ConfigureMonitorPrefab(GameObject prefabGameObject)
        {
            Constructable constructable =
                prefabGameObject.GetComponent<Constructable>();

            GameObject monitorVisuals =
                ModAssetUtils.GetPrefabInstanceFromAssetBundle(
                    CuddleCamMonitorPrefabName,
                    true);

            MaterialUtils.ApplySNShaders(monitorVisuals);

            constructable.model = monitorVisuals.transform
                .Find("Model")
                .gameObject;
        }
    }
}