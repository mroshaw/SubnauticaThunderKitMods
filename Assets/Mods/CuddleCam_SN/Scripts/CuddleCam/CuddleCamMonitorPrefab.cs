using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using UWE;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    /// <summary>
    /// Static class for creating the CuddleCam Monitor.
    /// </summary>
    internal static class CuddleCamMonitorPrefab
    {
        internal static PrefabInfo Info;
        private const string ClassId = "CuddleCamMonitor";
        private const string IconAssetName = "CuddleCamMonitorIcon.png";
        private const string CuddleCamMonitorPrefabName = "CuddleCamMonitor.prefab";
        private const string MonitorTextureAssetName = "CuddleCamMonitorTexture.png";
        private const string PictureFrameModelName = "mesh";
        private const string PictureFrameRendererName = "submarine_Picture_Frame";
        private const string PictureFrameButtonRendererName = "submarine_Picture_Frame_button";

        /// <summary>
        /// Registers the CuddleCam Monitor prefab.
        /// </summary>
        internal static void Register()
        {
            Info = PrefabInfo
                .WithTechType(
                    ClassId,
                    "CuddleCam Monitor",
                    "A monitor that displays images beamed from an active Cuddlefish 'CuddleCam' camera.",
                    unlockAtStart: true)
                .WithIcon(ModAssetUtils.GetObjectFromAssetBundle<Sprite>(IconAssetName) as Sprite);

            RecipeData recipe = new RecipeData(
                new Ingredient(TechType.Titanium, 1),
                new Ingredient(TechType.ComputerChip, 1),
                new Ingredient(TechType.CopperWire, 2),
                new Ingredient(TechType.Glass, 1));

            CustomPrefab cuddleMonitorPrefab = new CustomPrefab(Info);
            cuddleMonitorPrefab.SetRecipe(recipe);
            cuddleMonitorPrefab.SetPdaGroupCategory(
                TechGroup.Miscellaneous,
                TechCategory.Misc);
            cuddleMonitorPrefab.SetGameObject(CreateMonitorPrefabAsync);
            cuddleMonitorPrefab.Register();
        }

        private static IEnumerator CreateMonitorPrefabAsync(IOut<GameObject> prefabResult)
        {
            GameObject bundledPrefab =
                ModAssetUtils.GetObjectFromAssetBundle<GameObject>(CuddleCamMonitorPrefabName) as GameObject;

            if (!bundledPrefab)
            {
                ModDebugLog.LogError(
                    $"Could not find CuddleCamMonitor prefab named {CuddleCamMonitorPrefabName} in Asset Bundle!");
                yield break;
            }

            Texture2D monitorTexture =
                ModAssetUtils.GetObjectFromAssetBundle<Texture2D>(MonitorTextureAssetName) as Texture2D;

            if (!monitorTexture)
            {
                ModDebugLog.LogError(
                    $"Could not find monitor texture named {MonitorTextureAssetName} in Asset Bundle!");
                yield break;
            }

            CoroutineTask<GameObject> pictureFrameTask =
                CraftData.GetPrefabForTechTypeAsync(TechType.PictureFrame);
            yield return pictureFrameTask;

            GameObject pictureFramePrefab = pictureFrameTask.GetResult();
            if (!pictureFramePrefab)
            {
                ModDebugLog.LogError("Could not load the vanilla Picture Frame prefab.");
                yield break;
            }

            GameObject prefabGameObject =
                ModAssetUtils.GetPrefabInstanceFromAssetBundle(CuddleCamMonitorPrefabName, false);
            CuddleCamMonitor cuddleCamMonitor = prefabGameObject.GetComponent<CuddleCamMonitor>();
            if (!cuddleCamMonitor)
            {
                ModDebugLog.LogError(
                    $"The bundled prefab '{CuddleCamMonitorPrefabName}' has no CuddleCamMonitor component.");
                Object.Destroy(prefabGameObject);
                yield break;
            }

            GameObject modelPlaceholder = cuddleCamMonitor.ModelPlaceholder;
            if (!modelPlaceholder)
            {
                ModDebugLog.LogError(
                    "Could not find the custom Model placeholder.");
                Object.Destroy(prefabGameObject);
                yield break;
            }
            
            Transform pictureFrameModel = pictureFramePrefab.transform.Find(PictureFrameModelName);
            if (!pictureFrameModel)
            {
                ModDebugLog.LogError(
                    "Could not find the vanilla Picture Frame mesh.");
                Object.Destroy(prefabGameObject);
                yield break;
            }

            GameObject monitorModel = Object.Instantiate(
                pictureFrameModel.gameObject,
                modelPlaceholder.transform,
                false);
            ApplyMonitorTexture(monitorModel, monitorTexture);

            PrefabUtils.AddBasicComponents(
                prefabGameObject,
                ClassId,
                Info.TechType,
                LargeWorldEntity.CellLevel.Medium);
            PrefabUtils.AddConstructable(
                prefabGameObject,
                Info.TechType,
                ConstructableFlags.Base | ConstructableFlags.Wall,
                modelPlaceholder);

            prefabResult.Set(prefabGameObject);
            ModDebugLog.LogDebug(
                "Created CuddleCam Monitor prefab using the vanilla Picture Frame model.");
        }

        private static void ApplyMonitorTexture(GameObject monitorModel, Texture2D monitorTexture)
        {
            Renderer[] renderers = monitorModel.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer.gameObject.name != PictureFrameRendererName &&
                    renderer.gameObject.name != PictureFrameButtonRendererName)
                {
                    continue;
                }

                Material[] materials = renderer.materials;
                foreach (Material material in materials)
                {
                    material.SetTexture(ShaderPropertyID._MainTex, monitorTexture);
                }
            }
        }
    }
}
