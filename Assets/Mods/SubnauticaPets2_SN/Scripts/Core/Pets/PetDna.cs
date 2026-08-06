using System.Collections;
using ProtoBuf;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPets.Pets
{
    /// <summary>
    /// Settles newly distributed DNA samples onto suitable Subnautica terrain.
    /// </summary>
    [ProtoContract]
    internal class PetDna : MonoBehaviour, IProtoEventListener
    {
        private const int CurrentPlacementVersion = 1;
        private const float GroundProbeOffset = 0.25f;
        private const float GroundProbeRadius = 1.25f;
        private const float GroundSearchDistance = 25.0f;
        private const float GroundClearance = 0.03f;
        private const float MinimumFloorNormalY = 0.5f;
        private const float MaximumFloorHeightAboveSlot = 0.5f;
        private const int GroundPlacementAttemptCount = 120;

        private static readonly Vector3[] GroundProbeDirections =
        {
            Vector3.zero, Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            new Vector3(0.7071068f, 0.0f, 0.7071068f),
            new Vector3(-0.7071068f, 0.0f, 0.7071068f),
            new Vector3(0.7071068f, 0.0f, -0.7071068f),
            new Vector3(-0.7071068f, 0.0f, -0.7071068f)
        };

        [ProtoMember(1)] private int placementVersion;
        private bool wasDeserialized;

        private IEnumerator Start()
        {
            if (wasDeserialized && placementVersion >= CurrentPlacementVersion)
            {
                yield break;
            }

            Bounds streamingBounds = new Bounds(transform.position, Vector3.one);
            while (LargeWorldStreamer.main == null ||
                   !LargeWorldStreamer.main.IsRangeActiveAndBuilt(streamingBounds))
            {
                yield return null;
            }

            for (int attempt = 0; attempt < GroundPlacementAttemptCount; attempt++)
            {
                if (TrySettleOnGround())
                {
                    placementVersion = CurrentPlacementVersion;
                    yield break;
                }

                yield return null;
            }

            Debug.LogWarningFormat(this,
                "Pet DNA at {0} could not find a suitable floor after terrain loading and will not be spawned.",
                transform.position);
            Destroy(gameObject);
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            wasDeserialized = true;
        }

        private bool TrySettleOnGround()
        {
            Vector3 slotPosition = transform.position;
            RaycastHit hit;
            if (!TryFindFloor(slotPosition, out hit))
            {
                return false;
            }

            ApplyRestingPose(hit.normal, slotPosition);
            Bounds bounds = UWE.Utils.GetEncapsulatedAABB(gameObject);
            float boundsBottomOffset = bounds.min.y - transform.position.y;
            float rootHeight = hit.point.y + GroundClearance - boundsBottomOffset;
            transform.position = new Vector3(hit.point.x, rootHeight, hit.point.z);
            return true;
        }

        private static bool TryFindFloor(Vector3 slotPosition, out RaycastHit floorHit)
        {
            floorHit = default(RaycastHit);
            float bestDistanceSquared = float.MaxValue;
            bool foundFloor = false;
            foreach (Vector3 direction in GroundProbeDirections)
            {
                Vector3 horizontalOffset = direction * GroundProbeRadius;
                Vector3 upperOrigin = slotPosition + horizontalOffset + Vector3.up * GroundProbeOffset;
                foundFloor |= TryUseCloserFloor(upperOrigin, slotPosition, ref floorHit, ref bestDistanceSquared);
                Vector3 lowerOrigin = slotPosition + horizontalOffset - Vector3.up * GroundProbeOffset;
                foundFloor |= TryUseCloserFloor(lowerOrigin, slotPosition, ref floorHit, ref bestDistanceSquared);
            }

            return foundFloor;
        }

        private static bool TryUseCloserFloor(Vector3 origin, Vector3 slotPosition, ref RaycastHit floorHit,
            ref float bestDistanceSquared)
        {
            RaycastHit candidate;
            if (!Physics.Raycast(origin, Vector3.down, out candidate, GroundSearchDistance,
                    Voxeland.GetTerrainLayerMask(), QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            if (candidate.normal.y < MinimumFloorNormalY ||
                candidate.point.y > slotPosition.y + MaximumFloorHeightAboveSlot)
            {
                return false;
            }

            float distanceSquared = (candidate.point - slotPosition).sqrMagnitude;
            if (distanceSquared >= bestDistanceSquared)
            {
                return false;
            }

            floorHit = candidate;
            bestDistanceSquared = distanceSquared;
            return true;
        }

        private void ApplyRestingPose(Vector3 floorNormal, Vector3 slotPosition)
        {
            System.Random random = CreatePoseRandom(slotPosition);
            float poseSelection = (float)random.NextDouble();
            float tilt = poseSelection < 0.15f
                ? 0.0f
                : poseSelection < 0.35f
                    ? Mathf.Lerp(20.0f, 60.0f, (float)random.NextDouble())
                    : Mathf.Lerp(80.0f, 100.0f, (float)random.NextDouble());
            float yaw = (float)random.NextDouble() * 360.0f;
            Quaternion surfaceAlignment = Quaternion.FromToRotation(Vector3.up, floorNormal);
            Quaternion yawRotation = Quaternion.AngleAxis(yaw, floorNormal);
            Vector3 tiltAxis = yawRotation * (surfaceAlignment * Vector3.forward);
            transform.rotation = Quaternion.AngleAxis(tilt, tiltAxis) * yawRotation * surfaceAlignment;
        }

        private static System.Random CreatePoseRandom(Vector3 slotPosition)
        {
            int seed;
            unchecked
            {
                seed = 17;
                seed = seed * 31 + Mathf.RoundToInt(slotPosition.x * 100.0f);
                seed = seed * 31 + Mathf.RoundToInt(slotPosition.y * 100.0f);
                seed = seed * 31 + Mathf.RoundToInt(slotPosition.z * 100.0f);
            }

            return new System.Random(seed);
        }
    }
}
