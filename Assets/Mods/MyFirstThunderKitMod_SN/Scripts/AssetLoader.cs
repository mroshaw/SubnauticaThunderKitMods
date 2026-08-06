using UnityEngine;

namespace DaftAppleGames.MyFirstThunderKitMod
{
    internal static class AssetLoader
    {
        private const string AssetBundlePath = "assets/myfirstassetbundle";

        private static AssetBundle _assetBundle;
        
        // Load the Asset Bundle file
        internal static void LoadAssetBundle()
        {
            _assetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }

        // Loads and returns the prefab
        // Note, this returns the actual prefab - not an INSTANCE of the prefab
        internal static GameObject GetPrefabFromAssetBundle(string prefabName)
        {
            GameObject prefab = _assetBundle.LoadAsset<GameObject>(prefabName);
            return prefab;
        }

        // Loads and returns an instantiated instance of a prefab
        internal static GameObject GetPrefabInstanceFromAssetBundle(string prefabName)
        {
            GameObject prefab = GetPrefabFromAssetBundle(prefabName);
            GameObject newPrefabInstance = Object.Instantiate(prefab);
            
            return newPrefabInstance;
        }

        // Load, instantiate, position, rotate and scale instance of a prefab
        internal static GameObject InstantiateAndPlacePrefabInstance(string prefabName, Vector3 position,
            Quaternion rotation, Vector3 scale)
        {
            GameObject newPrefabInstance = GetPrefabInstanceFromAssetBundle(prefabName);
            newPrefabInstance.transform.position = position;
            newPrefabInstance.transform.rotation = rotation;
            newPrefabInstance.transform.localScale = scale;
            return newPrefabInstance;
        }
    }
}