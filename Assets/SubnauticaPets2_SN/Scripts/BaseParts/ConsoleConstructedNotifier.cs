using System;
using DaftAppleGames.SubnauticaPets.Utils;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPets.BaseParts
{ 
    public class ConsoleConstructedNotifier : MonoBehaviour, IConstructable
    {
        private PetConsole _petConsole;

        private void Awake()
        {
            _petConsole = GetComponentInChildren<PetConsole>(true);
        }
        
        public bool IsDeconstructionObstacle()
        {
            return false;
        }

        public bool CanDeconstruct(out string reason)
        {
            reason = String.Empty;
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {
            LogUtils.LogDebug(LogArea.Utilities, $"ConstructedChanged to: {constructed} on {gameObject.name}... Enable screen");
            _petConsole.SetConstructedState(constructed);
        }
    }
}