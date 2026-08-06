using DaftAppleGames.ModTools.Extensions;
using UnityEngine;
using static DaftAppleGames.MoreAquariums.MoreAquariumsPlugin;

namespace DaftAppleGames.MoreAquariums
{
    /// <summary>
    /// MonoBehaviour class for custom aquarium functionality
    /// </summary>
    public class CustomAquarium : MonoBehaviour
    {
        [SerializeField] private string prefabId;
        [SerializeField] private AquariumType aquariumType;
        [SerializeField] private bool isGhost;
        [SerializeField] private string prefabName;

        private static GameObject _prefabGameObject;
        
        private AquariumConfigurator _configurator; 
        
        public string PrefabId => _prefabIdentifier ? _prefabIdentifier.Id : "";
        public AquariumType AquariumType => aquariumType;
        public string PrefabName => prefabName;
        
        private PrefabIdentifier _prefabIdentifier;

        
        /// <summary>
        /// Initialise components
        /// </summary>
        private void Awake()
        {
            _prefabIdentifier = GetComponent<PrefabIdentifier>();

            if (!_prefabGameObject)
            {
                _prefabGameObject = ModAssetUtils.GetPrefabInstanceFromAssetBundle(prefabName, true);
            }

            if (!_configurator)
            {
                _configurator = _prefabGameObject.GetComponent<AquariumConfigurator>();
            }

            // SwapModel();
        }

        /// <summary>
        /// Swap the old model with the new aquarium model
        /// </summary>
        private void SwapModel()
        {
            GameObject newModelGameObject = _configurator.NewAquariumModel;

            Transform oldModelTransform = _prefabGameObject.transform.Find(_configurator.OldGhostModelPath);
            if (!oldModelTransform)
            {
                ModDebugLog.LogError($"Could not find old model at: {_configurator.OldGhostModelPath} on {_configurator.name}! ABORTING!");
                foreach (Transform childTransform in _prefabGameObject.transform)
                {
                    ModDebugLog.LogDebug($"Child Transform: {childTransform.name}");                    
                }
                return;
            }
            
            ModDebugLog.LogDebug($"Replacing old model at: {_configurator.OldGhostModelPath} with new model: {_configurator.NewAquariumModel.name}...");
            newModelGameObject.transform.SetParent(oldModelTransform.parent);
            newModelGameObject.transform.LocalZero();
            oldModelTransform.gameObject.SetActive(false);
            newModelGameObject.SetActive(true);
        }
        
        /// <summary>
        /// Public setter for AquariumType
        /// </summary>
        internal void SetAquariumType(AquariumType newAquariumType)
        {
            aquariumType = newAquariumType;
        }

        /// <summary>
        /// Public setter for IsGhost
        /// </summary>
        internal void SetIsGhost(bool newIsGhost)
        {
            isGhost = newIsGhost;
        }

        internal void SetPrefabName(string newPrefabName)
        {
            prefabName = newPrefabName;
        }
        
        /// <summary>
        /// Wait for data to be loaded, then update if this is a loaded pet
        /// </summary>
        internal void LoadData()
        {
            foreach (AquariumSaver.AquariumDetails aquariumDetails in LoadedAquariumDetailsHashSet)
            {
                if (aquariumDetails.PrefabId == PrefabId)
                {
                    ModDebugLog.LogDebug( $"Aquarium: Found {aquariumDetails.PrefabId}, configuring Aquarium Type to:: {aquariumDetails.AquariumType}");
                    aquariumType = aquariumDetails.AquariumType;
                    break;
                }
            }
        }
    }
}