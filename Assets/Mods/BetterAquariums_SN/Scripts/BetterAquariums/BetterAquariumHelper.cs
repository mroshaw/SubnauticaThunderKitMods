using System;
using UnityEngine;
using static DaftAppleGames.BetterAquariums_SN.BetterAquariumsPlugin;

namespace DaftAppleGames.BetterAquariums_SN
{
    public enum BetterAquariumType
    {
        Double,
        Corner,
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

        [Header("Fish Attach Points")]
        [SerializeField] private Animator animator1;
        [SerializeField] private Animator animator2;
        [SerializeField] private FishTrackContainer[] existingTrackObjects;
        [SerializeField] private FishTrackContainer[] newTrackObjects;

        internal FishTrackContainer[] ExistingContainer => existingTrackObjects;
        
        [Serializable]
        public class FishTrackContainer
        {
            [SerializeField] private GameObject trackObject;
            [SerializeField] private GameObject trackAttachObject;
            
            public string TrackName => trackObject.name;
            public string AttachName => trackAttachObject.name;
            
            public Vector3 TrackPosition => trackObject.transform.position;
            public Vector3 TrackLocalPosition => trackObject.transform.localPosition;
            public Vector3 AttachPosition => trackAttachObject.transform.position;
            public Vector3 AttachLocalPosition => trackAttachObject.transform.localPosition;
            
            public override string ToString()
            {
                return ($"{trackObject.name}/{trackAttachObject.name}");
            }
        }
        
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
                case BetterAquariumType.Corner:
                    vanillaAquariumGo.AddComponent<CornerAquarium>();
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

            ModDebugLog.LogDebug($"Finding animators...");
            GameObject animatorGo1 = vanillaAquariumGo.transform.Find("model/Aquarium_animation2").gameObject;
            GameObject animatorGo2 = vanillaAquariumGo.transform.Find("model/Aquarium_animation").gameObject;
            ModDebugLog.LogDebug($"Found animators");
            
            ModDebugLog.LogDebug($"Finding track roots...");
            GameObject trackRoot1To4 = animatorGo1.transform.Find("root").gameObject;
            GameObject trackRoot5To8 = animatorGo2.transform.Find("root").gameObject;
            ModDebugLog.LogDebug($"Found track roots");
            
            ModDebugLog.LogDebug($"Finding geometry...");
            GameObject geometry1 = animatorGo1.transform.Find("Aquarium_geo").gameObject;
            GameObject geometry2 = animatorGo2.transform.Find("Aquarium_geo").gameObject;
            ModDebugLog.LogDebug($"Found geometry");

            if (existingTrackObjects == null)
            {
                ModDebugLog.LogError($"ExistingTrackObjects is null!");
                return;
            }
            
            if (existingTrackObjects[0] == null)
            {
                ModDebugLog.LogError($"ExistingTrackObjects items are null!");
                return;
            }
            
            // Update positions of existing track objects
            int currTrackIndex = 0;
            foreach (FishTrackContainer existingContainer in existingTrackObjects)
            {
                ModDebugLog.LogDebug("Processing track containers...");
                
                ModDebugLog.LogDebug($"Configuring track: {existingContainer.ToString()}");
                GameObject trackRoot = currTrackIndex < 4 ? trackRoot1To4 : trackRoot5To8;
                
                ModDebugLog.LogDebug($"Looking for track in root: {existingContainer.TrackName}");
                GameObject existingTrackGo = trackRoot.transform.Find(existingContainer.TrackName).gameObject;
                existingTrackGo.transform.localPosition = existingContainer.TrackLocalPosition;
                
                ModDebugLog.LogDebug($"Looking for attach in track: {existingContainer.AttachName}");
                GameObject existingAttachGo = existingTrackGo.transform.Find(existingContainer.AttachName).gameObject;
                existingAttachGo.transform.localPosition = existingContainer.AttachLocalPosition;
                
                updatedTrackObjects[currTrackIndex] = existingAttachGo;
                currTrackIndex++;
            }

            // Create new Fish Tracks
            ModDebugLog.LogDebug($"Creating new tracks...");
            currTrackIndex = 0;
            foreach (FishTrackContainer newContainer in newTrackObjects)
            {
                GameObject trackRoot = currTrackIndex < 4 ? trackRoot1To4 : trackRoot5To8;
                
                ModDebugLog.LogDebug($"Creating track: {trackRoot.name}/{newContainer.ToString()}");
                GameObject newTrackGo = new GameObject(newContainer.TrackName);
                newTrackGo.transform.SetParent(trackRoot.transform);
                newTrackGo.transform.localPosition = newContainer.TrackLocalPosition;
                
                GameObject newAttachGo = new GameObject(newContainer.AttachName);
                newAttachGo.transform.SetParent(newTrackGo.transform);
                newAttachGo.transform.localPosition = newContainer.AttachLocalPosition;
                
                ModDebugLog.LogDebug($"Track created successfully");
                
                updatedTrackObjects[currTrackIndex + 8] = newAttachGo;
                currTrackIndex++;
            }

            // Now set the trackObjects on the Aquarium component
            vanillaAquarium.trackObjects = updatedTrackObjects;
            
            // Update the animators
            // Move the animator gameobject, unparent/reparent children to avoid move
            trackRoot1To4.transform.SetParent(null);
            geometry1.transform.SetParent(null);
            trackRoot5To8.transform.SetParent(null);
            geometry2.transform.SetParent(null);

            // Move
            animatorGo1.transform.localPosition = animator1.gameObject.transform.position;
            animatorGo2.transform.localPosition = animator2.gameObject.transform.position;
            
            // Reparent
            trackRoot1To4.transform.SetParent(animatorGo1.transform);
            geometry1.transform.SetParent(animatorGo1.transform);
            trackRoot5To8.transform.SetParent(animatorGo2.transform);
            geometry2.transform.SetParent(animatorGo2.transform);
            
            // Get current prefab animators
            ModDebugLog.LogDebug($"Updating animators...");
            Animator anim1 = animatorGo1.GetComponent<Animator>();
            Animator anim2 = animatorGo2.GetComponent<Animator>();

            // Set new ones with additional fish tracks
            anim1.runtimeAnimatorController = animator1.runtimeAnimatorController;
            anim2.runtimeAnimatorController = animator2.runtimeAnimatorController;
            ModDebugLog.LogDebug($"Animators updated");
            
            ModDebugLog.LogDebug($"Done configuring new aquarium!");
        }
    }
}