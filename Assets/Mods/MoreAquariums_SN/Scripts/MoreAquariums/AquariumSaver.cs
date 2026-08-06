using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums
{
    /// <summary>
    /// MonoBehaviour class to save and load custom external Aquariums
    /// </summary>
    internal class AquariumSaver : MonoBehaviour
    {
        internal List<CustomAquarium> AquariumList = new List<CustomAquarium>();

        /// <summary>
        /// Abstract instance stub for UnityEvent
        /// </summary>
        internal class OnAquariumRegisteredEvent : UnityEvent<CustomAquarium>
        {
        }

        /// <summary>
        /// Abstract instance stub for UnityEvent 
        /// </summary>
        internal class OnAquariumUnRegisteredEvent : UnityEvent<CustomAquarium>
        {
        }

        internal OnAquariumRegisteredEvent OnAquariumRegistered = new OnAquariumRegisteredEvent();
        internal OnAquariumUnRegisteredEvent OnAquariumUnRegistered = new OnAquariumUnRegisteredEvent();
        internal UnityEvent OnAquariumListUpdated = new UnityEvent();

        /// <summary>
        /// Subscribe to Scene Load, to trigger the loading process
        /// </summary>
        private void OnEnable()
        {
            Init();
            SceneManager.sceneLoaded += SceneLoadedHandler;
        }

        /// <summary>
        /// Unsubscribe from Scene Load
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoadedHandler;
        }

        /// <summary>
        /// Hook into the Scene Looder to re-initialise the list each time
        /// a the Main Menu is loaded
        /// </summary>
        private void SceneLoadedHandler(Scene scene, LoadSceneMode loadSceneMode)
        {
            ModDebugLog.LogDebug( $"Scene Loaded: {scene.name}");
            if (scene.name == "MenuEnvironment")
            {
                ClearList();
            }
        }

        /// <summary>
        /// Initialise the Saver
        /// </summary>
        internal void Init()
        {
            AquariumList = new List<CustomAquarium>();
        }

        /// <summary>
        /// Register a new Aquarium the HashList
        /// </summary>
        internal void RegisterAquarium(CustomAquarium newAquarium)
        {
            if (AquariumList == null)
            {
                AquariumList = new List<CustomAquarium>();
            }

            if (!AquariumList.Contains(newAquarium))
            {
                AquariumList.Add(newAquarium);
                ModDebugLog.LogDebug( $"AquariumSaver: Added new Aquarium: {newAquarium}");
                OnAquariumRegistered?.Invoke(newAquarium);
                OnAquariumListUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Remove an Aquarium from the HashList
        /// </summary>
        internal void UnregisterAquarium(CustomAquarium existingAquarium)
        {
            if (AquariumList.Contains(existingAquarium))
            {
                AquariumList.Remove(existingAquarium);
                ModDebugLog.LogDebug( $"AquariumSaver: Removed Aquarium: {existingAquarium}");
                OnAquariumUnRegistered?.Invoke(existingAquarium);
                OnAquariumListUpdated?.Invoke();
            }
        }

        internal void ForceRefresh()
        {
            OnAquariumListUpdated.Invoke();
        }

        /// <summary>
        /// Creates a HashSet of current Aquariums, suitable for using in a save game
        /// </summary>
        internal HashSet<AquariumDetails> GetAquariumListAsHashSet()
        {
            HashSet<AquariumDetails> hashSet = new HashSet<AquariumDetails>();

            if (AquariumList == null)
            {
                AquariumList = new List<CustomAquarium>();
            }

            foreach (CustomAquarium aquarium in AquariumList)
            {
                if (aquarium)
                {
                    AquariumDetails newAquariumDetails = new AquariumDetails(aquarium.PrefabId, aquarium.AquariumType);
                    hashSet.Add(newAquariumDetails);
                }
            }
            return hashSet;
        }

        /// <summary>
        /// Internal AquariumDetails class, used to store "minimum" attributes for a custom aquarium
        /// so we can serialize and deserialize for saving and loading
        /// </summary>
        public class AquariumDetails
        {
            public string PrefabId { get; }
            public AquariumType AquariumType { get; }

            /// <summary>
            /// Constructor
            /// </summary>
            [JsonConstructor]
            public AquariumDetails(string prefabId, AquariumType aquariumType)
            {
                PrefabId = prefabId;
                AquariumType = aquariumType;
            }
        }

        /// <summary>
        /// Load and update Aquariums
        /// </summary>
        internal void LoadData()
        {
            StartCoroutine(WaitForDataLoad());
        }

        /// <summary>
        /// Wait for the world to settle, then init aquariums
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForDataLoad()
        {
            LargeWorldStreamer streamer = null;

            while (streamer == null)
            {
                streamer = FindObjectOfType<LargeWorldStreamer>();
                yield return new WaitForEndOfFrame();
            }

            while (!streamer.IsWorldSettled())
            {
                yield return new WaitForEndOfFrame();
            }
            InitLoadedAquariums();
        }

        /// <summary>
        /// Iterate through and Init aquariums loaded, once scene is loaded
        /// </summary>
        private void InitLoadedAquariums()
        {
            ModDebugLog.LogDebug( $"Loading Pet Data...");
            foreach (CustomAquarium aquarium in FindObjectsOfType<CustomAquarium>())
            {
                aquarium.LoadData();
            }
        }

        /// <summary>
        /// Removes everything from the  List
        /// </summary>
        private void ClearList()
        {
            if (AquariumList != null)
            {
                AquariumList.Clear();
            }
        }
    }
}
