using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.BiggerAquariums.BiggerAquariumsPlugin;

namespace DaftAppleGames.BiggerAquariums
{
    public class FishManager : MonoBehaviour
    {
        [SerializeField] private FishSettings fishSettings;
        [SerializeField] private List<Collider> movementColliders;
        
        internal List<BiggerAquariumFish> FishList => _fishList;

        private readonly List<BiggerAquariumFish> _fishList = new List<BiggerAquariumFish>();
        
        /// <summary>
        /// Get and configure all child fish objects
        /// </summary>
        private void Awake()
        {
            // Refresh attached fish
            foreach (BiggerAquariumFish fish in GetComponentsInChildren<BiggerAquariumFish>(true))
            {
                fish.SetColliders(movementColliders);
                fish.SetFishSettings(fishSettings);
                fish.SetFishManager(this);
            }
        }
        
        internal void AddFish(BiggerAquariumFish newFish)
        {
            _fishList.Add(newFish);
        }

        internal void RemoveFish(BiggerAquariumFish fishToRemove)
        {
            _fishList.Remove(fishToRemove);
        }
    }
}