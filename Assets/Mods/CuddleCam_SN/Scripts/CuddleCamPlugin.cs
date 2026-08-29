using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using DaftAppleGames.ModTools;
using UnityEngine;

namespace DaftAppleGames.CuddleCam_SN
{
    [BepInPlugin(MyGuid, PluginName, VersionString)] public class CuddleCamPluginPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.mroshaw.cuddlecamsn";
        private const string PluginName = "CuddleCam SN";
        private const string VersionString = "1.0.1";

        private const string AssetBundleName = "cuddlecamassetbundle";
        private const string ManagerPrefabName = "CuddleCamManager.prefab";
        
        private static readonly Harmony Harmony = new Harmony(MyGuid);
        internal static ModAssetBundleUtils ModAssetUtils;
        
        // Config file / Log initialisation
#if !UNITY_EDITOR
        internal static ModConfigFile ConfigFile = OptionsPanelHandler.RegisterModOptions<ModConfigFile>();
        internal static ModLog ModDebugLog;
#else
        internal static readonly ModConfigFile ConfigFile;
        internal static ModLog ModDebugLog = new ModLog(null, true);
#endif

        private void Awake()
        {
            // Initialise Logger
            ModDebugLog = new ModLog(Logger, ConfigFile.DetailedLogging);

            // Initialise AssetBundle
            ModAssetUtils =
                new ModAssetBundleUtils(AssetBundleName, Assembly.GetExecutingAssembly(), true, ModDebugLog);

            // Set up custom prefabs
            CuddleCamMonitorPrefab.Register();

            // Set up gameplay lifecycle
            WaitScreenHandler.RegisterLoadTask(PluginName, HandleGameLoad);
            SaveUtils.RegisterOnQuitEvent(DestroyManager);
            
            // Patch in our MOD
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
        }

        private void HandleGameLoad(WaitScreenHandler.WaitScreenTask task)
        {
            ModDebugLog.LogDebug("CuddleCam gameplay load task started.");
            EnsureManager();
        }

        private void EnsureManager()
        {
            if (CuddleCamManager.Instance)
            {
                ModDebugLog.LogDebug(
                    $"CuddleCam manager already exists as '{CuddleCamManager.Instance.gameObject.name}'.");
                return;
            }

            ModDebugLog.LogDebug($"Creating CuddleCam manager from '{ManagerPrefabName}'.");

            GameObject managerGameObject =
                ModAssetUtils.GetPrefabInstanceFromAssetBundle(ManagerPrefabName, true);

            if (!managerGameObject)
            {
                ModDebugLog.LogError($"Could not create manager prefab '{ManagerPrefabName}'.");
                return;
            }

            CuddleCamManager manager = managerGameObject.GetComponent<CuddleCamManager>();
            ModDebugLog.LogDebug(
                $"Created manager GameObject '{managerGameObject.name}'. " +
                $"ActiveSelf={managerGameObject.activeSelf}, ActiveInHierarchy={managerGameObject.activeInHierarchy}, " +
                $"HasManagerComponent={manager}, InstanceAssigned={CuddleCamManager.Instance}.");
        }

        private void DestroyManager()
        {
            if (!CuddleCamManager.Instance)
            {
                ModDebugLog.LogDebug("CuddleCam quit cleanup found no manager to destroy.");
                return;
            }

            GameObject managerGameObject = CuddleCamManager.Instance.gameObject;
            ModDebugLog.LogDebug($"Destroying CuddleCam manager '{managerGameObject.name}' during quit cleanup.");
            Destroy(managerGameObject);
        }
    }
}
