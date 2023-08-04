using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PickleMan.PlayerStates;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using TMPro;

namespace PickleMan
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour, PlayerInput.IPlayerMovementActions
    {
        #region Class Members

        [Header("Player Acceleration")]
        public float walkAcceleration = 1.0f;
        public float sprintAcceleration = 2.0f;
        public float crouchAcceleration = 0.5f;
        public float slideAcceleration = 0.1f;
        public float swimAcceleration = 0.5f;
        public float inAirAcceleration = 0.5f;
        public float slideStartImpulse = 1.0f;

        [Header("Player Speeds")]
        public float walkMaxSpeed = 5.0f;
        public float sprintMaxSpeed = 10f;
        public float crouchMaxSpeed = 2.5f;
        public float swimMaxSpeed = 2.5f;
        public float jumpSpeed = 8.0f;
        public float slideSpeedStartCutoff = 5.0f; 
        public float slideSpeedEndCutoff = 2.0f;
        public float idlingSpeedCutoff = 0.01f;
        public float timeBetweenSlides = 0.75f;

        [Header("Camera setup")]
        [SerializeField] public Camera playerCamera;
        public float cameraOffsetY = 0.4f;
        public float crouchOffsetY = 0.15f;
        public float crouchScale = 0.75f;
        public float crouchScaleSpeed = 0.1f;
        public float lookLimitV = 89.0f;
        public float lookSenseH = 2.0f;
        public float lookSenseV = 2.0f;

        [Header("Settings")]
        public bool holdToSprint = true;
        public bool holdToCrouch = true;

        [Header("Environment setup")]
        public float maxIsGroundedAngle = 45.0f;
        public float defaultDrag = 3.0f;
        public float defaultInAirDrag = 1.0f;
        public float defaultStaticFriction = 0.6f;
        public float defaultDynamicFriction = 0.6f;

        // Player components
        private PlayerStates playerStates;
        private Player player;
        private Rigidbody rb;
        private CapsuleCollider capsuleCol;

        // Player movement
        private Vector3 moveImpulse = Vector3.zero;
        private Vector3 currentJumpImpulse = Vector3.zero;
        private Vector3 currentSlideImpulse = Vector3.zero;
        private Vector2 inputDirection = Vector2.zero;

        // Camera
        private float crouchLerpPosY = 0f;
        private float yaw = 0f;
        private float pitch = 0f;

        // Key presses
        private bool movePressed = false;
        private bool sprintPressed = false;
        private bool crouchPressed = false;
        private bool jumpPressed = false;

        // Player state checks
        private bool canJump = false;
        private bool canAccelerate = true;
        private bool canStand = true;
        private bool canSprint = false;
        private bool canCrouch = true;
        private bool canUncrouch = false;
        private bool canSlide = false;

        private bool isGrounded = false;
        private bool isOnMovingPlatform = false;
        private bool isInWater = false;
        private bool isOnIce = false;

        // Player states
        private bool isMoving = false;
        private bool isSprinting = false;
        private bool isCrouching = false;
        private bool isUncrouching = false;
        private bool isSliding = false;
        private bool isJumping = false;

        // Timers
        private float timeSinceLastSlide = 9999f;

        private Transform playerContainerParent = null;
        private Transform isGroundedTransform = null;

        // Game input
        public PlayerInput CustomGameInput { get; private set; }

        #endregion

        #region Initialization
        private void Awake()
        {
            CustomGameInput = new PlayerInput();
            CustomGameInput.Enable();
            CustomGameInput.PlayerMovement.SetCallbacks(this);
        }

        private void Start()
        {
            // Get components
            playerStates = GetComponent<PlayerStates>();
            player = GetComponent<Player>();
            rb = GetComponent<Rigidbody>();
            capsuleCol = GetComponent<CapsuleCollider>();

            // Set initial conditions
            SetNoDrag();
        }

        #endregion

        #region Update Logic

        private void Update()
        {
            UpdateMovementLogic();
            playerStates.UpdateMovementState(isInWater, isMoving, canAccelerate, isGrounded, isSprinting, isCrouching, isUncrouching, isSliding);
            playerStates.UpdateEnvironmentState(isInWater, isOnIce);

            if (playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Jumping))
            {
                print("Jumping");
            }
        }

        private void UpdateMovementLogic()
        {
            // No dependencies
            isMoving = IsMoving();
            isGrounded = IsGrounded();
            isOnMovingPlatform = IsOnMovingPlatform();
            isInWater = IsInWater();
            isOnIce = IsOnIce();

            // No dependencies
            canAccelerate = CanAccelerate();
            canStand = CanStand();
            canUncrouch = CanUncrouch();
            canSlide = CanSlide();

            // These values have dependencies; preserve order
            canSprint = CanSprint(); // Dependent on canUncrouch
            isSliding = IsSliding();
            isSprinting = IsSprinting();
            canCrouch = CanCrouch(); // Dependent on isSprinting and isSliding
            isCrouching = IsCrouching(); // Dependent on isSliding
            isUncrouching = IsUncrouching(); // Sets isCrouching

            canJump = CanJump();
            isJumping = IsJumping();

            // Get current speed multiplier based on movement state
            float impulseMultiplier = UpdateSpeedMultiplier();

            // Get current look direction and speed for movement input
            Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward);   
            Vector3 right = playerCamera.transform.TransformDirection(Vector3.right);
            float curSpeedX = impulseMultiplier * inputDirection.y;
            float curSpeedY = impulseMultiplier * inputDirection.x;

            moveImpulse = (forward * curSpeedX) + (right * curSpeedY);
            moveImpulse = isInWater ? moveImpulse : new Vector3(moveImpulse.x, 0f, moveImpulse.z);

            // Slide logic
            timeSinceLastSlide += Time.deltaTime;
            bool slideTimerReady = timeSinceLastSlide >= timeBetweenSlides;
            bool wasSliding = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Sliding);
            if (!wasSliding && isSliding)
            {
                currentSlideImpulse = forward * slideStartImpulse;
                timeSinceLastSlide = 0f;
            }

            // Jump logic
            if (isJumping)
                currentJumpImpulse = Vector3.up * jumpSpeed;
        }

        private float UpdateSpeedMultiplier()
        {
            float impulseMultiplier;

            if (!canAccelerate) impulseMultiplier = 0f;
            else if (isInWater) impulseMultiplier = swimAcceleration;
            else if (!isGrounded) impulseMultiplier = inAirAcceleration;
            else if (isSliding) impulseMultiplier = slideAcceleration;
            else if (isCrouching) impulseMultiplier = crouchAcceleration;
            else if (isSprinting) impulseMultiplier = sprintAcceleration;
            else impulseMultiplier = walkAcceleration;

            return impulseMultiplier;
        }

        private void LateUpdate()
        {
            // Crouching logic
            float currentCameraOffsetTarget = ((isCrouching || isSliding) && !isUncrouching) ? crouchOffsetY : cameraOffsetY;
            float crouchScaleDelta = crouchScaleSpeed * Time.deltaTime;
            float currentScaleY = ((isCrouching || isSliding) && !isUncrouching) ? Mathf.Clamp(transform.localScale.y - crouchScaleDelta, crouchScale, 1f) :
                                                                                   Mathf.Clamp(transform.localScale.y + crouchScaleDelta, crouchScale, 1f);

            // Player scale
            transform.localScale = new Vector3(transform.localScale.x, currentScaleY, transform.localScale.z);

            // Camera position and rotation
            playerCamera.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            crouchLerpPosY = Mathf.Lerp(crouchLerpPosY, currentCameraOffsetTarget, crouchScaleDelta);
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + crouchLerpPosY, transform.position.z);
        }

        #endregion

        #region Fixed Update Logic

        private void FixedUpdate()
        {
            if (!canAccelerate) return;

            // Turn off gravity if we're in water
            rb.useGravity = isInWater ? false : true;

            // Jump logic
            moveImpulse += currentJumpImpulse;
            currentJumpImpulse = Vector3.zero;

            // Moving platform logic
            if (isJumping)
            {
                float velocityY = isGroundedTransform?.GetComponent<TransformVelocity>()?.Velocity.y ?? 0f;
                rb.velocity = new Vector3(rb.velocity.x, velocityY, rb.velocity.z);
            }

            // Slide logic
            moveImpulse += currentSlideImpulse;
            currentSlideImpulse = Vector3.zero;

            // Drag and friction
            if (!isGrounded && !isInWater)
                SetInAirDrag();
            else if (isOnIce)
                SetNoDrag();
            else
                SetNormalDrag();


            // Clamp velocity logic
            ClampImpulse();

            // Move and rotate player rigidbody
            rb.AddForce(moveImpulse, ForceMode.Impulse);
            rb.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSenseH, 0);

            // Reset player logic after physics update
            jumpPressed = false;
        }

        private void SetInAirDrag()
        {
            rb.drag = defaultInAirDrag;
            capsuleCol.material.staticFriction = 0f;
            capsuleCol.material.dynamicFriction = 0f;
            capsuleCol.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        private void SetNoDrag()
        {
            rb.drag = 0f;
            capsuleCol.material.staticFriction = 0f;
            capsuleCol.material.dynamicFriction = 0f;
            capsuleCol.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        private void SetNormalDrag()
        {
            rb.drag = defaultDrag;
            capsuleCol.material.staticFriction = defaultStaticFriction;
            capsuleCol.material.dynamicFriction = defaultDynamicFriction;
            capsuleCol.material.frictionCombine = PhysicMaterialCombine.Average;
        }

        private void ClampImpulse()
        {
            if (!isGrounded && !isInWater) return;

            // We add new impulse to current velocity to check if we're speeding up or slowing down
            // We also ignore any impulses in the y-direction since we only care about clamping XZ speed
            Vector3 newVelocity = rb.velocity + moveImpulse / rb.mass;
            Vector3 newVelocityXZ = new Vector3(newVelocity.x, 0f, newVelocity.z);

            bool velocityIncreased = rb.velocity.magnitude < newVelocity.magnitude;
            bool exceedsWalkSpeed = newVelocityXZ.magnitude > walkMaxSpeed;
            bool exceedsSprintSpeed = newVelocityXZ.magnitude > sprintMaxSpeed;
            bool exceedsCrouchSpeed = newVelocityXZ.magnitude > crouchMaxSpeed;
            bool exceedsSwimSpeed = newVelocity.magnitude > swimMaxSpeed;
            bool walking = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Walking);

            // Walk/Sprint/Crouch conditions
            if (exceedsWalkSpeed && walking && velocityIncreased ||
                exceedsSprintSpeed && isSprinting && velocityIncreased ||
                exceedsCrouchSpeed && isCrouching && velocityIncreased)
            {
                moveImpulse = new Vector3(0f, moveImpulse.y, 0f);
            }
            // Swim conditions
            else if (exceedsSwimSpeed && isInWater && velocityIncreased)
            {
                moveImpulse = Vector3.zero;
            }
        }

        #endregion

        #region Movement State Checks

        private bool CanStand()
        {
            return true;
        }

        private bool CanAccelerate()
        {
            return (playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Immobile)) ? false : true;
        }

        private bool CanSprint()
        {
            bool crouching = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Crouching);
            return (!isInWater && isGrounded && canAccelerate && isMoving && movePressed && (!crouching || (crouching && canUncrouch))) ? true : false;
        }

        private bool CanCrouch()
        {
            bool sprinting = isSprinting;
            bool sliding = isSliding;
            return (!isInWater && canAccelerate && !sprinting && !sliding) ? true : false;
        }

        private bool CanUncrouch()
        {
            bool crouching = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Crouching);
            bool uncrouching = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Uncrouching);
            bool sliding = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Sliding);
            return (canStand && canAccelerate && (crouching || uncrouching || sliding)) ? true : false;
        }

        private bool CanJump()
        {
            bool crouching = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Crouching);
            bool uncrouching = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Uncrouching);
            bool sliding = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Sliding);

            return (isGrounded && canAccelerate && !isInWater && !crouching && (!uncrouching || sliding)) ? true : false;
        }

        private bool CanSlide()
        {
            Vector3 rbVelocityXZ = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            bool sliding = playerStates.CurrentMovementState.HasFlag(PlayerMovementState.Sliding);
            bool canStartSlide = (rbVelocityXZ.magnitude >= slideSpeedStartCutoff) && (!sliding);
            bool canContinueSlide = (rbVelocityXZ.magnitude >= slideSpeedEndCutoff) && (sliding);
            bool slideTimerReady = timeSinceLastSlide >= timeBetweenSlides;


            return (!isInWater && canAccelerate && isMoving && movePressed && isGrounded &&
                   (sliding || slideTimerReady) &&
                   (canStartSlide || canContinueSlide)) ? true : false;
        }


        private bool IsGrounded()
        {
            foreach (var keyValuePair in player.CollisionData)
            {
                float minAngle = 360f;

                foreach (ContactPoint contact in keyValuePair.Value.contacts)
                {
                    float currentAngle = Vector3.Angle(Vector3.up, contact.normal);
                    minAngle = Mathf.Min(currentAngle, minAngle);

                    if (minAngle < maxIsGroundedAngle)
                    {
                        isGroundedTransform = keyValuePair.Value.transform;
                        return true;
                    }
                }
            }

            RaycastHit hit;
            LayerMask playerMask = LayerMask.NameToLayer("Player");
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, capsuleCol.height / 2 + 0.2f, playerMask))
            {
                return true;
            }

            isGroundedTransform = null;
            return false;
        }

        public bool IsOnMovingPlatform()
        {
            if (player.MovingPlatforms.Count == 0 || !(isGroundedTransform?.CompareTag("MovingPlatform") ?? false))
            {
                // Remove player from moving platform transform
                if (playerContainerParent != null)
                {
                    playerContainerParent = null;
                    player.transform.parent.SetParent(null);
                }

                return false;
            }

            // Check if we're on a moving platform
            Rigidbody platformRigidbody = player.MovingPlatforms[0].gameObject.GetComponent<Rigidbody>();
            playerContainerParent = platformRigidbody.transform;
            player.transform.parent.SetParent(platformRigidbody.transform);

            return true;
        }

        public bool IsMoving()
        {
            return rb.velocity.magnitude > idlingSpeedCutoff;
        }

        public bool IsSprinting()
        {
            return (canSprint && sprintPressed && !isSliding) ? true : false;
        }

        public bool IsSliding()
        {
            return (canSlide && crouchPressed);
        }

        public bool IsCrouching()
        {
            return (canCrouch && crouchPressed) ? true : false;
        }

        private bool IsUncrouching()
        {
            float currentScaleY = transform.localScale.y;
            float targetScaleY = (isCrouching || isSliding) ? crouchScale : 1f;
            float scaleDiff = targetScaleY - currentScaleY;
            bool uncrouching = (scaleDiff > 0f || ((isCrouching || isSliding) && jumpPressed)) ? true : false; 

            if (uncrouching)
            {
                isCrouching = false;
                crouchPressed = false;
            }

            return uncrouching;
        }

        private bool IsJumping()
        {
            return (canJump && jumpPressed) ? true : false;
        }

        public bool IsInWater()
        {
            // If no active triggers, we're not in water, return false
            if (player.ActiveTriggers.Count == 0) return false;

            // Check triggers for water layer
            foreach (var trigger in player.ActiveTriggers)
            {
                if (trigger.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsOnIce()
        {
            // Return false if no active triggers
            if (player.ActiveColliders.Count == 0)
            {
                return false;
            }

            // Check triggers for ice layer
            foreach (var collider in player.ActiveColliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Ice"))
                {
                    return true;
                }
            }

            return false;
        }


        #endregion

        #region Player Input

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started) return;

            movePressed = context.performed ? true : false;
            inputDirection = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            // Player/Camera rotation
            yaw += lookSenseH * Input.GetAxis("Mouse X");
            pitch -= lookSenseV * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -lookLimitV, lookLimitV);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            jumpPressed = true;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                sprintPressed = holdToSprint ? true : !isSprinting;
            }
            else if (context.canceled)
            {
                sprintPressed = holdToSprint ? false : sprintPressed;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                crouchPressed = holdToCrouch ? true : !crouchPressed;
            }
            else if (context.canceled)
            {
                crouchPressed = holdToCrouch ? false : crouchPressed;
            }
        }

        #endregion
    }
}
