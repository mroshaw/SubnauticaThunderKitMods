using Nautilus.Utility;
using UnityEngine;
using static DaftAppleGames.BetterAquariums_SN.BetterAquariumsPlugin;

namespace DaftAppleGames.BetterAquariums_SN
{
    /// <summary>
    /// Component to allow switching out the new aquarium models on existing prefabs
    /// </summary>
    public class BetterAquariumHelper : MonoBehaviour
    {
        [Header("Main Objects")]
        [SerializeField] private MeshFilter aquariumMesh;
        [SerializeField] private MeshFilter aquariumGlassMesh;
        [SerializeField] private Transform bubbles1Transform;
        [SerializeField] private Transform bubbles2Transform;
        [SerializeField] private Transform coral1Transform;
        [SerializeField] private Transform coral2Transform;
        [SerializeField] private Transform[] newCoralTransforms;
        [SerializeField] private GameObject rocksObject;
        [SerializeField] private GameObject colliderObject;
        
        [Header("Fish Attach Points")]
        [SerializeField] private GameObject fishAttachRoot;
        [SerializeField] private GameObject fishAttach1;
        [SerializeField] private GameObject fishAttach2;
        [SerializeField] private GameObject fishAttach3;
        [SerializeField] private GameObject fishAttach4;
        [SerializeField] private GameObject fishAttach5;
        [SerializeField] private GameObject fishAttach6;
        [SerializeField] private GameObject fishAttach7;
        [SerializeField] private GameObject fishAttach8;
        
        
        /// <summary>
        /// Takes the "vanilla" aquarium prefab, and reconfigures it as the "Bigger" aquarium 
        /// </summary>
        public void ConfigureAquariumPrefab(GameObject vanillaAquariumGo)
        {
            ModDebugLog.LogDebug($"Configuring aquarium prefab: {vanillaAquariumGo}");
            
            // Get vanilla references
            Aquarium vanillaAquarium = vanillaAquariumGo.GetComponent<Aquarium>();
            Constructable vanillaConstructable = vanillaAquarium.GetComponent<Constructable>();
            
            // Replace the model
            ModDebugLog.LogDebug("Finding model...");
            GameObject aquariumModel = vanillaConstructable.model;
            
            ModDebugLog.LogDebug("Replacing meshes...");
            MeshFilter[] meshFilters = aquariumModel.GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter meshFilter in meshFilters)
            {
                ModDebugLog.LogDebug($"Checking mesh on: {meshFilter.gameObject.name}");
                if (meshFilter.gameObject.name == "Aquarium")
                {
                    ModDebugLog.LogDebug($"Replacing aquarium mesh on: {meshFilter.gameObject.name}");
                    meshFilter.mesh = aquariumMesh.sharedMesh;
                    continue;
                }

                if (meshFilter.gameObject.name == "Aquarium_glass" || meshFilter.gameObject.name == "Aquarium_glass_1")
                {
                    ModDebugLog.LogDebug($"Replacing aquarium glass mesh on: {meshFilter.gameObject.name}");
                    meshFilter.mesh = aquariumGlassMesh.sharedMesh;
                    continue;
                }
            }
            
            // Duplicate the bubbles
            ModDebugLog.LogDebug("Duplicating bubbles...");
            GameObject bubbles = vanillaAquariumGo.transform.Find("Bubbles").gameObject;
            if (!bubbles)
            {
                ModDebugLog.LogError("Could not find aquarium Bubbles gameobject! Aborting!");
                return;
            }
            GameObject newBubbles = Instantiate(bubbles, bubbles.transform.parent, true);
            bubbles.transform.localPosition = bubbles1Transform.localPosition;
            newBubbles.transform.localRotation = bubbles1Transform.localRotation;
            // newBubbles.transform.RotateAround (newBubbles.transform.position, newBubbles.transform.up, 180f);
            newBubbles.transform.localScale = bubbles1Transform.localScale;
            
            // Duplicate and reposition coral
            DuplicateCoral(aquariumModel);
            
            // Add the rocks
            GameObject newRocks = Instantiate(rocksObject, vanillaAquariumGo.transform, true);
            newRocks.transform.localPosition = rocksObject.transform.localPosition;
            newRocks.transform.localScale = Vector3.one;
            
            // Replace the collider
            ModDebugLog.LogDebug("Replacing collider...");
            GameObject oldCollider = vanillaAquariumGo.transform.Find("Collider").gameObject;
            oldCollider.SetActive(false);
            GameObject newCollider = Instantiate(colliderObject, oldCollider.transform.parent, true);
            newCollider.transform.localPosition = oldCollider.transform.localPosition;
            newCollider.transform.localRotation = oldCollider.transform.localRotation;
            newCollider.transform.localScale = oldCollider.transform.localScale;
            ModDebugLog.LogDebug("Done configuring prefab!");
        }

        /// <summary>
        /// Copies then repositions the second coral object to make things a bit more natural
        /// </summary>
        private void DuplicateCoral(GameObject aquariumModelGo)
        {
            // Duplicate coral
            ModDebugLog.LogDebug("Duplicating coral...");
            GameObject coral = aquariumModelGo.transform.Find("Coral").gameObject;
            if (!coral)
            {
                ModDebugLog.LogError("Could not find aquarium Coral gameobject! Aborting!");
                return;
            }
            GameObject newCoral = Instantiate(coral, coral.transform.parent, true);
            
            // Existing coral moves, no rotation changes
            coral.transform.localPosition = coral1Transform.localPosition;
            
            // New coral moved to new location, rotation and scale matches existing coral model
            newCoral.transform.localPosition = coral2Transform.localPosition;
            newCoral.transform.localRotation = coral.transform.localRotation;
            newCoral.transform.localScale = coral.transform.localScale;

            // Iterate through each coral game object, find it and reposition it
            foreach (Transform coralTransform in newCoralTransforms)
            {
                ModDebugLog.LogDebug($"Setting new position of: {coralTransform.gameObject.name}");
                GameObject origCoral = newCoral.transform.Find(coralTransform.gameObject.name).gameObject;
                if (!origCoral)
                {
                    ModDebugLog.LogError($"Could not find coral gameobject named: {coralTransform.gameObject.name}! Aborting!");
                    return;
                }
                
                // Reposition to the new position
                origCoral.transform.localPosition = coralTransform.localPosition;
                origCoral.SetActive(coralTransform.gameObject.activeSelf);
            }
        }
    }
}