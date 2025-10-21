using UnityEngine;
using static DaftAppleGames.BiggerAquariums.BiggerAquariumsPlugin;

namespace DaftAppleGames.BiggerAquariums
{
    public class BiggerAquariumFish : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] internal Collider aquariumBounds;
        [SerializeField] internal FishManager fishManager;
        [SerializeField] private FishSettings fishSettings;
        
        [Header("Debug")]
        [SerializeField]  private Vector3 currentDirection;
        [SerializeField]  private float currentSpeed;
        [SerializeField]  private Vector3 targetPosition =  Vector3.zero;
        [SerializeField] private Vector3 directionToTargetRaw;
        [SerializeField] private Vector3 directionToTargetModified;

        [SerializeField]  private bool isDarting;
        [SerializeField]  private float dartEndTime;
        [SerializeField]  private float nextDartTime;
        [SerializeField]  private bool atInitialisePosition;
        
        private float _noiseOffsetX, _noiseOffsetY, _noiseOffsetZ;

        private void OnEnable()
        {
            if (fishManager)
            {
                fishManager.AddFish(this);
            }
        }

        private void OnDisable()
        {
            if (fishManager)
            {
                fishManager.RemoveFish(this);
            }
        }
        
        private void Start()
        {
            currentDirection = Random.onUnitSphere;
            currentSpeed = fishSettings.baseSpeed + (Random.Range(0, fishSettings.randomSpeedModifier));

            _noiseOffsetX = Random.value * 100f;
            _noiseOffsetY = Random.value * 100f;
            _noiseOffsetZ = Random.value * 100f;

            if (!aquariumBounds)
            {
                return;
            }

            PickNewTarget();
            ScheduleNextDart();
        }

        private void Update()
        {
            if (!aquariumBounds)
                return;

            HandleDarting();

            // Calculate desired direction
            directionToTargetRaw = (targetPosition - transform.position).normalized;
            if (directionToTargetRaw.sqrMagnitude < 0.0001f)
            {
                // Too close to compute a valid direction; just keep moving forward
                transform.position += currentDirection * (currentSpeed * Time.deltaTime);
                return;
            }
            
            // Combine influences
            directionToTargetModified = directionToTargetRaw;

            if (fishSettings.steerFromBounds)
            {
                directionToTargetModified += GetBoundsDirection();
            }
            
            if (fishSettings.avoidOtherFish)
            {
                directionToTargetModified += AvoidOtherFish();
            }
            
            if (fishSettings.applyNoise)
            {
                directionToTargetModified += GetNoiseDirection() * fishSettings.noiseInfluence;
            }
            
            directionToTargetModified = directionToTargetModified.normalized;
            
            // Apply steering and movement
            currentDirection = Vector3.Slerp(currentDirection, directionToTargetModified, fishSettings.turnSpeed * Time.deltaTime);

            if (fishSettings.limitPitch)
            {
                // Limit roll and pitch so fish mostly rotate around the Y axis
                Quaternion targetRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
                Vector3 euler = targetRotation.eulerAngles;

                // Allow a little pitch when moving vertically
                float verticalPitch = -Mathf.Clamp(Vector3.Dot(currentDirection.normalized, Vector3.up) * fishSettings.pitchScaling, fishSettings.minPitch, fishSettings.maxPitch);

                targetRotation = Quaternion.Euler(verticalPitch, euler.y, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, fishSettings.turnSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(currentDirection, Vector3.up);
            }

            // Move towards target
            transform.position += currentDirection * (currentSpeed * Time.deltaTime);
            
            // Clamp position to remain inside collider bounds
            if (fishSettings.clampToBounds)
            {
                transform.position = GetBoundaryClampPosition(transform.position);
            }

            // Select new target if needed
            if (Vector3.Distance(transform.position, targetPosition) < fishSettings.arrivalDistance)
            {
                PickNewTarget();
            }
        }

        /// <summary>
        /// Public setting for FishSettings SO asset
        /// </summary>
        public void SetFishSettings(FishSettings newFishSettings)
        {
            fishSettings = newFishSettings;
        }

        /// <summary>
        /// Public setter for Fish Manager
        /// </summary>
        public void SetFishManager(FishManager newFishManager)
        {
            fishManager = newFishManager;
        }
        
        /// <summary>
        /// Set the bounding collider 
        /// </summary>
        public void SetCollider(Collider newCollider)
        {
            aquariumBounds = newCollider;
        }

        /// <summary>
        /// Sets a new target within the bounds of the collider
        /// </summary>
        private void PickNewTarget()
        {
            if (aquariumBounds is BoxCollider box)
            {
                // Correctly compute local random point
                Vector3 localRandom = new Vector3(
                    Random.Range(-box.size.x * 0.5f, box.size.x * 0.5f),
                    Random.Range(-box.size.y * 0.5f, box.size.y * 0.5f),
                    Random.Range(-box.size.z * 0.5f, box.size.z * 0.5f)
                );

                // Offset by the collider's local center
                localRandom += box.center;

                // Convert to world space
                Vector3 newTarget = box.transform.TransformPoint(localRandom);

                if (!atInitialisePosition)
                {
                    transform.position = newTarget;
                    atInitialisePosition = true;
                    PickNewTarget();
                    return;
                }

                targetPosition = newTarget;
            }
            else if (aquariumBounds is SphereCollider sphere)
            {
                Vector3 localRandom = Random.insideUnitSphere * sphere.radius + sphere.center;
                targetPosition = sphere.transform.TransformPoint(localRandom);
            }
            else
            {
                targetPosition = aquariumBounds.bounds.center;
            }
        }

        /// <summary>
        /// Gets some Perlin noise direction to add organic movement
        /// </summary>
        private Vector3 GetNoiseDirection()
        {
            float t = Time.time * fishSettings.noiseSpeed;
            float nx = Mathf.PerlinNoise(_noiseOffsetX, t) * 2f - 1f;
            float ny = Mathf.PerlinNoise(_noiseOffsetY, t + 10f) * 2f - 1f;
            float nz = Mathf.PerlinNoise(_noiseOffsetZ, t + 20f) * 2f - 1f;
            return new Vector3(nx, ny, nz).normalized;
        }

        /// <summary>
        /// Avoid other fish
        /// </summary>
        private Vector3 AvoidOtherFish()
        {
            Vector3 avoidance = Vector3.zero;
            
            int count = 0;

            foreach (BiggerAquariumFish fish in fishManager.FishList)
            {
                // Don't avoid self
                if (fish == this)
                {
                    continue;
                }
                float dist = Vector3.Distance(transform.position, fish.transform.position);
                if (dist < fishSettings.avoidanceRadius)
                {
                    avoidance += (transform.position - fish.transform.position).normalized / dist;
                    count++;
                }
            }

            if (count > 0)
            {
                avoidance = (avoidance / count) * fishSettings.avoidanceStrength;
            }

            return avoidance;
        }

        /// <summary>
        /// Returns a direction Vector away from the boundary
        /// </summary>
        private Vector3 GetBoundsDirection()
        {
            if (aquariumBounds is BoxCollider box)
            {
                Vector3 localPos = box.transform.InverseTransformPoint(transform.position);
                Vector3 halfSize = box.size * 0.5f;
                Vector3 steer = Vector3.zero;

                // Gradually steer back when approaching walls
                if (Mathf.Abs(localPos.x) > halfSize.x - fishSettings.boundaryMargin)
                    steer.x = -Mathf.Sign(localPos.x);
                if (Mathf.Abs(localPos.y) > halfSize.y - fishSettings.boundaryMargin)
                    steer.y = -Mathf.Sign(localPos.y);
                if (Mathf.Abs(localPos.z) > halfSize.z - fishSettings.boundaryMargin)
                    steer.z = -Mathf.Sign(localPos.z);

                return box.transform.TransformDirection(steer.normalized * fishSettings.boundarySteerStrength);
            }

            if (aquariumBounds is SphereCollider sphere)
            {
                Vector3 center = sphere.transform.TransformPoint(sphere.center);
                float radius = sphere.radius * Mathf.Max(
                    sphere.transform.lossyScale.x,
                    sphere.transform.lossyScale.y,
                    sphere.transform.lossyScale.z
                );

                Vector3 toCenter = center - transform.position;
                float dist = toCenter.magnitude;
                if (dist > radius - fishSettings.boundaryMargin)
                {
                    float strength = Mathf.InverseLerp(radius, radius - fishSettings.boundaryMargin, dist);
                    return toCenter.normalized * (strength * fishSettings.boundarySteerStrength);
                }
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Returns a clamped position on the collider bounds
        /// </summary>
        private Vector3 GetBoundaryClampPosition(Vector3 pos)
        {
            if (aquariumBounds is BoxCollider box)
            {
                // Convert to local collider space
                Vector3 local = box.transform.InverseTransformPoint(pos);
                Vector3 half = box.size * 0.5f;
                Vector3 center = box.center;

                // Clamp relative to collider's local center
                local.x = Mathf.Clamp(local.x, center.x - half.x, center.x + half.x);
                local.y = Mathf.Clamp(local.y, center.y - half.y, center.y + half.y);
                local.z = Mathf.Clamp(local.z, center.z - half.z, center.z + half.z);

                // Convert back to world space
                return box.transform.TransformPoint(local);
            }
            else if (aquariumBounds is SphereCollider sphere)
            {
                Vector3 center = sphere.transform.TransformPoint(sphere.center);
                float radius = sphere.radius * Mathf.Max(
                    sphere.transform.lossyScale.x,
                    sphere.transform.lossyScale.y,
                    sphere.transform.lossyScale.z
                );

                Vector3 dir = pos - center;
                float dist = dir.magnitude;

                if (dist > radius)
                    return center + dir.normalized * (radius - 0.01f);

                return pos;
            }

            return pos;
        }

        /// <summary>
        /// Random darting behaviour
        /// </summary>
        private void HandleDarting()
        {
            if (!fishSettings.randomDarting)
            {
                return;
            }
            
            if (isDarting && Time.time > dartEndTime)
            {
                isDarting = false;
                currentSpeed = fishSettings.baseSpeed;
                ScheduleNextDart();
            }
            else if (!isDarting && Time.time > nextDartTime)
            {
                isDarting = true;
                currentSpeed = fishSettings.baseSpeed * fishSettings.dartSpeedMultiplier;
                dartEndTime = Time.time + fishSettings.dartDuration;
            }
        }

        /// <summary>
        /// Schedule next darting behaviour
        /// </summary>
        private void ScheduleNextDart()
        {
            nextDartTime = Time.time + Random.Range(fishSettings.dartIntervalMin, fishSettings.dartIntervalMax);
        }

        /// <summary>
        /// Debug visualisations
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (targetPosition != Vector3.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(targetPosition, 0.1f);
                Gizmos.DrawLine(transform.position, targetPosition);
            }
        }
    }
}