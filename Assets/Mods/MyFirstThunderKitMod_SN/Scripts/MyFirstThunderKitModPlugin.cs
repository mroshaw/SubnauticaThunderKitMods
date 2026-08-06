using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;

namespace DaftAppleGames.MyFirstThunderKitMod
{
    [BepInDependency("com.snmodding.nautilus")]
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class MyFirstThunderKitModPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.daftapplegames.myfirstthunderkitmod";
        private const string PluginName = "MyFirstThunderKitMod";
        private const string VersionString = "1.0.0";
        
        // Asset Bundle code here
        private const string AssetBundleName = "myfirstassetbundle";
        // Maintain a static reference to AssetBundle, so it can be used throughout your mod code
        public static AssetBundle MyAssetBundle { get; private set; }        
        
        private static readonly Harmony Harmony = new Harmony(MyGuid);

        // Static Logger reference that we can use throughout our mode code
        internal static ManualLogSource ModLogger;
        
        private void Awake()
        {
            ModLogger =  Logger;
            ModLogger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            ModLogger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

            // Use the Nautilus API to load the Asset Bundle
            ModLogger.LogInfo($"Loading AssetBundle from {AssetBundleName}...");
            MyAssetBundle =
                AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), AssetBundleName);
            if (!MyAssetBundle)
            {
                ModLogger.LogInfo($"Failed to load AssetBundle from {AssetBundleName}! Check the path!");
            }
            ModLogger.LogInfo($"Asset Bundle Loaded!");
            
            ModLogger.LogInfo($"Welcome to my first ThunderKit Plugin!");
        }
    }
}
