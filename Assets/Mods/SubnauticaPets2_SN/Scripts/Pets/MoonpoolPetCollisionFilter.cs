using System.Collections.Generic;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPets.Pets
{
    /// <summary>
    /// Keeps the Moonpool blocker solid only for pets.
    /// </summary>
    internal class MoonpoolPetCollisionFilter : MonoBehaviour
    {
        private readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        private BoxCollider blockerCollider;
        private BoxCollider filterTrigger;

        internal void Init(BoxCollider moonpoolBlocker, BoxCollider moonpoolFilterTrigger)
        {
            blockerCollider = moonpoolBlocker;
            filterTrigger = moonpoolFilterTrigger;
        }

        internal void PrimeExistingOverlaps()
        {
            if (!blockerCollider || !filterTrigger)
            {
                return;
            }

            Physics.SyncTransforms();
            Collider[] overlaps = Physics.OverlapBox(filterTrigger.bounds.center, filterTrigger.bounds.extents,
                filterTrigger.transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            foreach (Collider overlap in overlaps)
            {
                IgnoreForNonPet(overlap);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IgnoreForNonPet(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other || !ignoredColliders.Remove(other) || !blockerCollider)
            {
                return;
            }

            Physics.IgnoreCollision(blockerCollider, other, false);
        }

        private void OnDisable()
        {
            RestoreIgnoredCollisions();
        }

        private void OnDestroy()
        {
            RestoreIgnoredCollisions();
        }

        private void IgnoreForNonPet(Collider other)
        {
            if (!other || other == blockerCollider || other == filterTrigger || other.isTrigger ||
                other.GetComponentInParent<Pet>() || !blockerCollider ||
                Physics.GetIgnoreCollision(blockerCollider, other))
            {
                return;
            }

            Physics.IgnoreCollision(blockerCollider, other, true);
            ignoredColliders.Add(other);
        }

        private void RestoreIgnoredCollisions()
        {
            if (blockerCollider)
            {
                foreach (Collider ignoredCollider in ignoredColliders)
                {
                    if (ignoredCollider)
                    {
                        Physics.IgnoreCollision(blockerCollider, ignoredCollider, false);
                    }
                }
            }

            ignoredColliders.Clear();
        }
    }
}
