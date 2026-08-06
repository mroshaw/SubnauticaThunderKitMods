using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.SubnauticaPets.Pets
{
    /// <summary>
    /// Simple movement using Unity CharacterController, constrained to the pet's base floor.
    /// </summary>
    [RequireComponent(typeof(PetAnimator), typeof(Pet), typeof(PetStateController))]
    internal class SimpleMovement : MonoBehaviour
    {
        private const float BoundaryEventCooldown = 0.25f;
        private const float GroundProbeHeight = 0.4f;
        private const float GroundProbeDistance = 0.6f;
        private const float GroundProbeRadiusScale = 0.45f;
        private const float LookAheadDistance = 0.2f;
        private const float SafePositionInterval = 0.2f;
        private const float SpawnSettlementMinimumDistance = 0.75f;
        private const float SpawnSettlementMinimumDrop = 0.35f;
        private const float SpawnSettlementTimeout = 6.0f;
        private const float UngroundedRecoveryDelay = 0.5f;
        private const float StuckCheckInterval = 1.0f;
        private const float StuckDistanceThreshold = 0.05f;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 0.8f;
        [SerializeField] private float rotateSpeed = 4.0f;
        [SerializeField] private float arrivalTolerance = 0.05f;

        [Header("Debug Movement")]
        [SerializeField] private Transform targetMarker;
        [SerializeField] private bool isGrounded;
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private Vector3 moveTarget;
        [SerializeField] private float distanceToTarget;
        [SerializeField] private bool isMoving;
        [SerializeField] private Vector3 lastSafePosition;
        [SerializeField] private bool hasSafePosition;
        [SerializeField] private bool isSettlingAfterSpawn;

        [SerializeField] internal UnityEvent onArrived = new UnityEvent();
        [SerializeField] internal ControllerColliderHitEvent OnHitObstacle = new ControllerColliderHitEvent();
        [SerializeField] internal MovementBoundaryEvent OnUnsafeBoundary = new MovementBoundaryEvent();

        private readonly RaycastHit[] groundHits = new RaycastHit[8];
        private CharacterController characterController;
        private PetAnimator petAnimator;
        private Pet pet;
        private PetStateController stateController;
        private Rigidbody rigidbodyComponent;
        private float boundaryEventTimer;
        private float safePositionTimer;
        private float stuckCheckTimer;
        private float ungroundedTimer;
        private Vector3 lastStuckCheckPosition;
        private Vector3 spawnSettlementDirection;
        private Vector3 spawnSettlementStartPosition;
        private float spawnSettlementStartHeight;
        private float spawnSettlementTimer;
        private bool hasLeftSpawnSurface;

        private bool IsMoving
        {
            get => isMoving;
            set
            {
                isMoving = value;
                petAnimator.SetMoving(value);
            }
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            petAnimator = GetComponent<PetAnimator>();
            pet = GetComponent<Pet>();
            stateController = GetComponent<PetStateController>();
            lastStuckCheckPosition = transform.position;
        }

        private void OnEnable()
        {
            rigidbodyComponent = GetComponent<Rigidbody>();
            rigidbodyComponent.isKinematic = true;
            rigidbodyComponent.useGravity = false;
        }

        private void Update()
        {
            boundaryEventTimer -= Time.deltaTime;
            Vector3 velocity = Vector3.zero;

            if (isSettlingAfterSpawn)
            {
                if (!IsMoving)
                {
                    IsMoving = true;
                }

                moveDirection = spawnSettlementDirection;
                velocity = moveDirection * moveSpeed;
                RotateToTarget();
            }
            else if (IsMoving)
            {
                SetMoveDirection();
                if (HasFloorAhead())
                {
                    velocity = moveDirection * moveSpeed;
                    RotateToTarget();
                }
                else
                {
                    NotifyUnsafeBoundary(-moveDirection);
                }
            }

            characterController.SimpleMove(velocity);
            if (isSettlingAfterSpawn)
            {
                UpdateSpawnSettlement();
                if (isSettlingAfterSpawn)
                {
                    return;
                }
            }

            CheckGroundSafety();
            CheckForStuckMovement();

            if (IsMoving && HasArrived())
            {
                IsMoving = false;
                onArrived.Invoke();
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < 45.0f || boundaryEventTimer > 0.0f)
            {
                return;
            }

            boundaryEventTimer = BoundaryEventCooldown;
            OnHitObstacle.Invoke(Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized);
        }

        internal void SetMoveSpeed(float newMoveSpeed)
        {
            moveSpeed = newMoveSpeed;
        }

        internal void MoveToNewTarget(Vector3 target)
        {
            moveTarget = target;
            IsMoving = true;
            if (targetMarker)
            {
                targetMarker.position = target;
            }
        }

        internal void Stop()
        {
            IsMoving = false;
        }

        internal void DisableForDeath()
        {
            isSettlingAfterSpawn = false;
            Stop();
            if (characterController)
            {
                characterController.enabled = false;
            }

            enabled = false;
        }

        internal void BeginSpawnSettlement(Vector3 exitDirection)
        {
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(exitDirection, Vector3.up).normalized;
            if (horizontalDirection == Vector3.zero)
            {
                horizontalDirection = transform.forward;
            }

            spawnSettlementDirection = horizontalDirection;
            spawnSettlementStartPosition = transform.position;
            spawnSettlementStartHeight = transform.position.y;
            spawnSettlementTimer = 0.0f;
            hasLeftSpawnSurface = false;
            hasSafePosition = false;
            isSettlingAfterSpawn = true;
            IsMoving = true;
        }

        private void SetMoveDirection()
        {
            moveTarget.y = transform.position.y;
            moveDirection = (moveTarget - transform.position).normalized;
            moveDirection.y = 0.0f;
        }

        private bool HasFloorAhead()
        {
            if (moveDirection == Vector3.zero)
            {
                return true;
            }

            float footprintOffset = characterController.radius * 0.55f;
            Vector3 probeCenter = transform.position + moveDirection * (characterController.radius + LookAheadDistance);
            Vector3 sideDirection = Vector3.Cross(Vector3.up, moveDirection).normalized;
            return HasValidFloor(probeCenter) &&
                   HasValidFloor(probeCenter + sideDirection * footprintOffset) &&
                   HasValidFloor(probeCenter - sideDirection * footprintOffset);
        }

        private void RotateToTarget()
        {
            if (moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
            }
        }

        private bool HasValidFloor(Vector3 position)
        {
            Vector3 origin = position + Vector3.up * GroundProbeHeight;
            float probeRadius = Mathf.Max(0.03f, characterController.radius * GroundProbeRadiusScale);
            int hitCount = Physics.SphereCastNonAlloc(origin, probeRadius, Vector3.down, groundHits,
                GroundProbeHeight + GroundProbeDistance, ~0, QueryTriggerInteraction.Ignore);

            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                RaycastHit hit = groundHits[hitIndex];
                if (!hit.collider || hit.collider.transform.IsChildOf(transform) ||
                    Vector3.Angle(hit.normal, Vector3.up) > characterController.slopeLimit)
                {
                    continue;
                }

                if (BelongsToPetBase(hit.collider))
                {
                    return true;
                }
            }

            return false;
        }

        private bool BelongsToPetBase(Collider floorCollider)
        {
            if (!pet || !pet.Base)
            {
                return true;
            }

            if (floorCollider.transform.IsChildOf(pet.Base.transform))
            {
                return true;
            }

            Base floorBase = floorCollider.GetComponentInParent<Base>();
            return floorBase && floorBase == pet.Base;
        }

        private void CheckGroundSafety()
        {
            isGrounded = characterController.isGrounded && HasValidFloor(transform.position);
            if (isGrounded)
            {
                ungroundedTimer = 0.0f;
                safePositionTimer += Time.deltaTime;
                if (safePositionTimer >= SafePositionInterval)
                {
                    lastSafePosition = transform.position;
                    hasSafePosition = true;
                    safePositionTimer = 0.0f;
                }

                return;
            }

            safePositionTimer = 0.0f;
            ungroundedTimer += Time.deltaTime;
            if (hasSafePosition && ungroundedTimer >= UngroundedRecoveryDelay)
            {
                RecoverToLastSafePosition();
            }
        }

        private void UpdateSpawnSettlement()
        {
            spawnSettlementTimer += Time.deltaTime;
            Vector3 displacement = Vector3.ProjectOnPlane(transform.position - spawnSettlementStartPosition, Vector3.up);
            float horizontalDistance = displacement.magnitude;
            float verticalDrop = spawnSettlementStartHeight - transform.position.y;
            if (!characterController.isGrounded || verticalDrop >= SpawnSettlementMinimumDrop)
            {
                hasLeftSpawnSurface = true;
            }

            bool landed = hasLeftSpawnSurface && characterController.isGrounded &&
                          horizontalDistance >= SpawnSettlementMinimumDistance &&
                          verticalDrop >= SpawnSettlementMinimumDrop;
            bool timedOut = spawnSettlementTimer >= SpawnSettlementTimeout;
            if (!landed && !timedOut)
            {
                return;
            }

            isSettlingAfterSpawn = false;
            IsMoving = false;
            ungroundedTimer = 0.0f;
            safePositionTimer = 0.0f;
            lastStuckCheckPosition = transform.position;
            if (stateController)
            {
                stateController.SetNewState(landed ? PetState.Wandering : PetState.Idle);
            }
        }

        private void RecoverToLastSafePosition()
        {
            Vector3 recoveryDirection = Vector3.ProjectOnPlane(lastSafePosition - transform.position, Vector3.up);
            characterController.enabled = false;
            transform.position = lastSafePosition;
            characterController.enabled = true;
            ungroundedTimer = 0.0f;
            NotifyUnsafeBoundary(recoveryDirection.normalized);
        }

        private void CheckForStuckMovement()
        {
            stuckCheckTimer += Time.deltaTime;
            if (stuckCheckTimer < StuckCheckInterval)
            {
                return;
            }

            if (IsMoving && Vector3.Distance(transform.position, lastStuckCheckPosition) < StuckDistanceThreshold)
            {
                NotifyUnsafeBoundary(-moveDirection);
            }

            lastStuckCheckPosition = transform.position;
            stuckCheckTimer = 0.0f;
        }

        private void NotifyUnsafeBoundary(Vector3 safeDirection)
        {
            if (boundaryEventTimer > 0.0f)
            {
                return;
            }

            boundaryEventTimer = BoundaryEventCooldown;
            OnUnsafeBoundary.Invoke(safeDirection);
        }

        private bool HasArrived()
        {
            distanceToTarget = Vector3.Distance(transform.position, moveTarget);
            return distanceToTarget < arrivalTolerance;
        }

        internal class ControllerColliderHitEvent : UnityEvent<Vector3>
        {
        }

        internal class MovementBoundaryEvent : UnityEvent<Vector3>
        {
        }
    }
}
