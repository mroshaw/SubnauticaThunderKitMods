using System;
using UnityEngine;
using UnityEngine.Events;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.Utils
{
    /// <summary>
    /// Add to a Constructable component to trigger related events
    /// </summary>
    public class ConstructableListener : MonoBehaviour, IConstructable
    {
        public UnityEvent onConstructed = new UnityEvent();
        
        public bool IsDeconstructionObstacle()
        {
            return false;
        }

        public bool CanDeconstruct(out string reason)
        {
            reason = String.Empty;
            return false;
        }

        public void OnConstructedChanged(bool constructed)
        {
            if (constructed)
            {
                ModDebugLog.LogDebug( $"ConstructedChanged to: {constructed} on {gameObject.name}. Invoking Events...");
                onConstructed.Invoke();
            }
        }
    }
}