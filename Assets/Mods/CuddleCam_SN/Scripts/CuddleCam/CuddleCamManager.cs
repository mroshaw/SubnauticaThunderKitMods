using System;
using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    public class CuddleCamManager : MonoBehaviour
    {
        private readonly List<CuddleCamSource> activeSources = new List<CuddleCamSource>();

        internal static CuddleCamManager Instance { get; private set; }
        internal IReadOnlyList<CuddleCamSource> ActiveSources => activeSources;
        internal event Action SourcesChanged;

        private void Awake()
        {
            ModDebugLog.LogDebug(
                $"CuddleCamManager.Awake on '{gameObject.name}'. ExistingInstance={Instance}, " +
                $"ActiveSelf={gameObject.activeSelf}, ActiveInHierarchy={gameObject.activeInHierarchy}.");

            if (Instance && Instance != this)
            {
                ModDebugLog.LogDebug(
                    $"Destroying duplicate CuddleCamManager '{gameObject.name}'. Existing manager is '{Instance.gameObject.name}'.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            ModDebugLog.LogDebug($"CuddleCamManager singleton assigned to '{gameObject.name}'.");
        }

        internal void RegisterSource(CuddleCamSource source)
        {
            if (!source)
            {
                ModDebugLog.LogDebug("Ignored an attempt to register a missing CuddleCam source.");
                return;
            }

            if (activeSources.Contains(source))
            {
                ModDebugLog.LogDebug($"CuddleCam source '{source.name}' is already registered.");
                return;
            }

            activeSources.Add(source);
            string cuddlefishName = source.Cuddlefish ? source.Cuddlefish.name : "<missing>";
            ModDebugLog.LogDebug(
                $"Registered CuddleCam source '{source.name}' for '{cuddlefishName}'. Active source count={activeSources.Count}.");
            SourcesChanged?.Invoke();
        }

        internal void UnregisterSource(CuddleCamSource source)
        {
            if (!source)
            {
                ModDebugLog.LogDebug("Ignored an attempt to unregister a missing CuddleCam source.");
                return;
            }

            if (!activeSources.Remove(source))
            {
                ModDebugLog.LogDebug($"CuddleCam source '{source.name}' was not registered.");
                return;
            }

            ModDebugLog.LogDebug(
                $"Unregistered CuddleCam source '{source.name}'. Active source count={activeSources.Count}.");
            SourcesChanged?.Invoke();
        }

        internal CuddleCamSource GetAdjacentSource(CuddleCamSource currentSource, int direction)
        {
            if (activeSources.Count == 0)
            {
                return null;
            }

            int currentIndex = activeSources.IndexOf(currentSource);
            if (currentIndex < 0)
            {
                return direction < 0
                    ? activeSources[activeSources.Count - 1]
                    : activeSources[0];
            }

            int nextIndex = currentIndex + direction;
            if (nextIndex < 0 || nextIndex >= activeSources.Count)
            {
                return null;
            }

            return activeSources[nextIndex];
        }

        internal int GetSourceNumber(CuddleCamSource source)
        {
            int sourceIndex = activeSources.IndexOf(source);
            return sourceIndex < 0 ? 0 : sourceIndex + 1;
        }

        private void OnDestroy()
        {
            ModDebugLog.LogDebug(
                $"CuddleCamManager.OnDestroy on '{gameObject.name}'. IsCurrentInstance={Instance == this}.");

            if (Instance == this)
            {
                Instance = null;
                ModDebugLog.LogDebug("CuddleCamManager singleton reference cleared.");
            }
        }
    }
}
