using UnityEngine;
using static DaftAppleGames.BetterAquariums_SN.BetterAquariumsPlugin;

namespace DaftAppleGames.BetterAquariums_SN
{
    public enum BetterAquariumType
    {
        Double,
        Curved,
        LShaped
    }

    /// <summary>
    /// Component to allow switching out the new aquarium models on existing prefabs
    /// </summary>
    public class BetterAquariumHelper : MonoBehaviour
    {
        [Header("Main Objects")] [SerializeField] private MeshFilter aquariumMesh;
        [SerializeField] private MeshFilter aquariumGlassMesh;
        [SerializeField] private Transform bubbles1Transform;
        [SerializeField] private Transform bubbles2Transform;
        [SerializeField] private Transform coral1Transform;
        [SerializeField] private Transform coral2Transform;
        [SerializeField] private Transform[] newCoralTransforms;
        [SerializeField] private GameObject rocksObject;
        [SerializeField] private GameObject colliderObject;

        [Header("Fish Attach Points")] [SerializeField] private GameObject trackRoot1To4;
        [SerializeField] private GameObject trackRoot5To6;
        [SerializeField] private GameObject[] existingTrackObjects;
        [SerializeField] private GameObject[] newTrackObjects;

        /// <summary>
        /// Takes the "vanilla" aquarium prefab, and reconfigures it as the "Bigger" aquarium 
        /// </summary>
        internal void ConfigureAquariumPrefab(GameObject vanillaAquariumGo, BetterAquariumType aquariumType)
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

            // Reposition tracks and add new
            ModDebugLog.LogDebug("Configuring tracks...");
            ConfigureTracks(vanillaAquariumGo);
            
            // Add the new component
            AddAquariumComponent(vanillaAquariumGo, aquariumType);
        }

        /// <summary>
        /// Add the correct component
        /// </summary>
        private void AddAquariumComponent(GameObject vanillaAquariumGo, BetterAquariumType aquariumType)
        {
            switch (aquariumType)
            {
                case BetterAquariumType.Double:
                    vanillaAquariumGo.AddComponent<DoubleAquarium>();
                    break;
                case BetterAquariumType.Curved:
                    vanillaAquariumGo.AddComponent<CurvedAquarium>();
                    break;
                case BetterAquariumType.LShaped:
                    vanillaAquariumGo.AddComponent<LShapedAquarium>();
                    break;
            }
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
                    ModDebugLog.LogError(
                        $"Could not find coral gameobject named: {coralTransform.gameObject.name}! Aborting!");
                    return;
                }

                // Reposition to the new position
                origCoral.transform.localPosition = coralTransform.localPosition;
                origCoral.SetActive(coralTransform.gameObject.activeSelf);
            }
        }

        /// <summary>
        /// Reconfigures existing Fish Tracks (spawn points) and adds new ones
        /// </summary>
        private void ConfigureTracks(GameObject vanillaAquariumGo)
        {
            // We'll use this to reset the trackObjects on the Aquarium component
            GameObject[] updatedTrackObjects = new GameObject[16];

            // Reconfigure the existing 8 tracks
            Aquarium vanillaAquarium = vanillaAquariumGo.GetComponent<Aquarium>();
            ModDebugLog.LogDebug($"Finding track roots...");
            trackRoot1To4 = vanillaAquariumGo.transform.Find("model/Aquarium_animation2/root").gameObject;
            ModDebugLog.LogDebug($"Found track root 1 to 4");
            trackRoot5To6 = vanillaAquariumGo.transform.Find("model/Aquarium_animation/root").gameObject;
            ModDebugLog.LogDebug($"Found track root 5 to 6");
            int currTrackIndex = 0;
            foreach (GameObject existingTrackNewPos in existingTrackObjects)
            {
                ModDebugLog.LogDebug($"Configuring track: {existingTrackNewPos.name}");
                GameObject trackRoot = currTrackIndex < 4 ? trackRoot1To4 : trackRoot5To6;
                ModDebugLog.LogDebug($"Track root is: {trackRoot.name}");
                int currGoIndex = currTrackIndex < 4 ? currTrackIndex + 1: currTrackIndex - 3;
                string trackPath = $"fish{currGoIndex}";
                ModDebugLog.LogDebug($"Looking for track in path: {trackPath}");
                GameObject existingTrack = trackRoot.transform.Find(trackPath).gameObject;
                existingTrack.transform.localPosition = existingTrackNewPos.transform.localPosition;
                updatedTrackObjects[currTrackIndex] = existingTrack;
                currTrackIndex++;
            }

            // Create new Fish Tracks
            currTrackIndex = 0;
            foreach (GameObject newTrackNewPos in newTrackObjects)
            {
                ModDebugLog.LogDebug($"Creating new track: {newTrackNewPos.name}");
                GameObject trackRoot = currTrackIndex < 4 ? trackRoot1To4 : trackRoot5To6;
                GameObject newTrackParentGo = new GameObject($"fish{currTrackIndex + 8}");
                newTrackParentGo.transform.SetParent(trackRoot.transform);
                newTrackParentGo.transform.localPosition = newTrackNewPos.transform.localPosition;
                
                GameObject newTrackGo = new GameObject(newTrackNewPos.name);
                newTrackGo.transform.SetParent(newTrackParentGo.transform);
                newTrackGo.transform.localPosition = Vector3.zero;
                newTrackGo.transform.localRotation = Quaternion.identity;
                newTrackGo.transform.localScale = Vector3.one;
                updatedTrackObjects[currTrackIndex + 8] = newTrackGo;
                currTrackIndex++;
            }

            // Now set the trackObjects on the Aquarium component
            vanillaAquarium.trackObjects = updatedTrackObjects;
        }
    }
}