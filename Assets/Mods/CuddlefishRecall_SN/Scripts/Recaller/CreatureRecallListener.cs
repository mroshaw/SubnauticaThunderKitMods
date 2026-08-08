using UnityEngine;
using static DaftAppleGames.CuddlefishRecall_SN.CuddlefishRecallPlugin;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    internal class CreatureRecallListener : MonoBehaviour
    {
        private const float TeleportClearanceRadius = 0.75f;

        private CreatureRecallAction creatureRecallAction;
        private Rigidbody rigidbodyComponent;

        internal bool IsRecallInProgress => creatureRecallAction.IsRecalling;

        internal int RecallCreatureIndex => creatureRecallAction.CreatureIndex;

        internal float DistanceToPlayer => creatureRecallAction.DistanceToPlayer;

        /// <summary>
        /// Initialise the component
        /// </summary>
        private void Awake()
        {
            ModDebugLog.LogDebug("Finding CreatureRecallAction...");
            creatureRecallAction = GetComponent<CreatureRecallAction>();
            rigidbodyComponent = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Public method to recall the creature to the target transform
        /// </summary>
        internal void RecallCreature(float buffer, int creatureIndex)
        {
            // Teleport method
            if (ConfigFile.RecallMoveMethod == RecallMoveMethod.Teleport)
            {
                Vector3 targetPosition = Player.main.transform.position + (Camera.main.transform.forward * (buffer * 2));
                ModDebugLog.LogDebug($"Teleporting GameObject to: {targetPosition}");

                if (IsTeleportDestinationBlocked(targetPosition))
                {
                    ErrorMessage.AddMessage($"Cuddlefish {creatureIndex} is blocked and cannot be recalled to this location!");
                    return;
                }

                transform.position = targetPosition;
                rigidbodyComponent.velocity = Vector3.zero;
                rigidbodyComponent.angularVelocity = Vector3.zero;
                creatureRecallAction.CompleteTeleportRecall(creatureIndex);
                ModDebugLog.LogDebug("GameObject teleported.");
            }

            // Swim to method
            if (ConfigFile.RecallMoveMethod == RecallMoveMethod.SwimTo)
            {
                ModDebugLog.LogDebug($"Swimming to Player position");
                ModDebugLog.LogDebug("Swimming to player in progress...");
                creatureRecallAction.BeginRecall(creatureIndex);
            }
        }

        private bool IsTeleportDestinationBlocked(Vector3 targetPosition)
        {
            int numColliders = UWE.Utils.OverlapSphereIntoSharedBuffer(
                targetPosition,
                TeleportClearanceRadius,
                -1,
                QueryTriggerInteraction.Ignore);

            foreach (Collider obstacleCollider in UWE.Utils.sharedColliderBuffer)
            {
                if (numColliders <= 0)
                {
                    break;
                }

                numColliders--;

                if (!obstacleCollider ||
                    obstacleCollider.transform.IsChildOf(transform) ||
                    obstacleCollider.transform.IsChildOf(Player.main.transform))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
