using System;
using UnityEngine;
using static DaftAppleGames.CuddlefishRecall_SN.CuddlefishRecallPlugin;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    internal class CreatureRecallListener : MonoBehaviour
    {
        // Determines how close to the player before considered "arrived"
        private const float ArrivalTolerance = 1.5f;
        private SwimBehaviour _swimBehaviour;
        private SwimRandom _swimRandom;

        private int _creatureIndex;
        
        // Parameters for the "Swim To" function
        private bool _isBeingRecalled;

        /// <summary>
        /// Initialise the component
        /// </summary>
        private void Start()
        {
            Log.LogDebug("Finding SwimBehaviour...");
            _swimBehaviour = GetComponent<SwimBehaviour>();
            _swimRandom = GetComponent<SwimRandom>();
        }

        /// <summary>
        /// Used to call the SwimTo behaviour, if enabled
        /// </summary>
        private void Update()
        {
            if (!_isBeingRecalled)
            {
                return;
            }

            // Check to see if we've arrived
            if (Vector3.Distance(transform.position, Player.main.transform.position) < ArrivalTolerance)
            {
                ErrorMessage.AddMessage($"Cuddlefish {_creatureIndex} has arrived!");
                _swimRandom.enabled = true;
                _isBeingRecalled = false;
                return;
            }

            // Swim to target
            if (ConfigFile.RecallMoveMethod == RecallMoveMethod.SwimTo)
            {
                _swimBehaviour.SwimTo(Player.main.transform.position, ConfigFile.RecallSwimVelocity);
            }
        }

        /// <summary>
        /// Public method to recall the creature to the target transform
        /// </summary>
        internal void RecallCreature(float buffer, int creatureIndex)
        {
            // Already being recalled
            if (_isBeingRecalled)
            {
                return;
            }

            _isBeingRecalled = true;
            _creatureIndex = creatureIndex;
            
            // Teleport method
            if (ConfigFile.RecallMoveMethod == RecallMoveMethod.Teleport)
            {
                Vector3 targetPosition = Player.main.transform.position + (Vector3.forward * buffer);
                Log.LogDebug($"Teleporting GameObject to: {targetPosition}");

                if (Player.main.GetBiomeString().StartsWith("precursor", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage.AddMessage($"Cuddlefish {_creatureIndex} cannot be recalled to this location!");
                    return;
                }

                // Check if there are any obstacles blocking the cuddle fish
                int num = UWE.Utils.OverlapSphereIntoSharedBuffer(transform.position, 5f, -1, QueryTriggerInteraction.UseGlobal);
                for (int i = 0; i < num; i++)
                {
                    if (UWE.Utils.sharedColliderBuffer[i].GetComponentInParent<SubRoot>())
                    {
                        ErrorMessage.AddMessage($"Cuddlefish {_creatureIndex} is blocked and cannot be recalled to this location!");
                        return;
                    }
                }

                gameObject.transform.position = targetPosition;
                Log.LogDebug("GameObject teleported.");
            }

            // Swim to method
            if (ConfigFile.RecallMoveMethod == RecallMoveMethod.SwimTo)
            {
                Log.LogDebug($"Swimming to Player position");
                Log.LogDebug("Swimming to player in progress...");
                _swimRandom.enabled = false;
            }
        }
    }
}