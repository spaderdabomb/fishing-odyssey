using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickleMan
{
    public class PlayerStates : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentMovementState { get; set; } = PlayerMovementState.None;
        [field: SerializeField] public PlayerUniqueMovementState CurrentUniqueMovementState { get; set; } = PlayerUniqueMovementState.None;
        [field: SerializeField] public PlayerEnvironmentState CurrentEnvironmentState { get; set; } = PlayerEnvironmentState.None;
        [field: SerializeField] public PlayerFishingState CurrentFishingState { get; set; } = PlayerFishingState.None;

        private Player player;
        private PlayerMovement playerMovement;
        private Rigidbody rb;
        private void Start()
        {
            player = GetComponent<Player>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
        }

        public void UpdateMovementState(bool isInWater, bool isMoving, bool canAccelerate, bool isGrounded, bool isSprinting, bool isCrouching, bool isUncrouching, bool isSliding)
        {
            CurrentMovementState = PlayerMovementState.None;

            if (!isMoving)
                CurrentMovementState |= PlayerMovementState.Idling;

            if (isMoving && !isInWater && !isSprinting && isGrounded && !isSliding)
                CurrentMovementState |= PlayerMovementState.Walking;

            if (isGrounded && isSprinting)
                CurrentMovementState |= PlayerMovementState.Sprinting;

            if (!isGrounded && !isInWater)
                CurrentMovementState |= PlayerMovementState.Jumping;

            if (isInWater)
                CurrentMovementState |= PlayerMovementState.Swimming;

            if (isCrouching)
                CurrentMovementState |= PlayerMovementState.Crouching;

            if (isUncrouching)
                CurrentMovementState |= PlayerMovementState.Uncrouching;

            if (isSliding)
                CurrentMovementState |= PlayerMovementState.Sliding;

            if (!canAccelerate)
            {
                CurrentMovementState |= PlayerMovementState.Immobile;
            }

            UpdateUniqueMovementState();
        }

        public void UpdateUniqueMovementState()
        {
            if (CurrentMovementState == PlayerMovementState.Idling)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Idling;
            else if (CurrentMovementState == PlayerMovementState.Walking)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Walking;
            else if (CurrentMovementState == PlayerMovementState.Sprinting)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Sprinting;
            else if (CurrentMovementState == PlayerMovementState.Jumping)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Jumping;
            else if (CurrentMovementState == PlayerMovementState.Sliding)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Sliding;
            else if (CurrentMovementState == PlayerMovementState.Swimming)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Swimming;
            else if (CurrentMovementState == PlayerMovementState.Immobile)
                CurrentUniqueMovementState = PlayerUniqueMovementState.Immobile;
            else if (CurrentMovementState.HasFlag(PlayerMovementState.Uncrouching))
                CurrentUniqueMovementState = PlayerUniqueMovementState.Uncrouching;
            else if (CurrentMovementState.HasFlag(PlayerMovementState.Crouching) && 
                     CurrentMovementState.HasFlag(PlayerMovementState.Walking))
                CurrentUniqueMovementState = PlayerUniqueMovementState.CrouchWalking;
            else if (CurrentMovementState.HasFlag(PlayerMovementState.Crouching) &&
                     CurrentMovementState.HasFlag(PlayerMovementState.Idling))
                CurrentUniqueMovementState = PlayerUniqueMovementState.CrouchIdling;
        }

        public void UpdateEnvironmentState(bool isInWater, bool isOnIce)
        {
            CurrentEnvironmentState = PlayerEnvironmentState.None;

            if (isInWater)
                CurrentEnvironmentState |= PlayerEnvironmentState.InWater;

            if (isOnIce)
                CurrentEnvironmentState |= PlayerEnvironmentState.OnIce;
        }

        [Flags]
        public enum PlayerMovementState
        {
            None = 0,
            Idling = 1,
            Walking = 2,
            Sprinting = 4,
            Jumping = 8,
            Crouching = 16,
            Uncrouching = 32,
            Sliding = 64,
            Swimming = 128,
            Immobile = 256
        }

        public enum PlayerUniqueMovementState
        {
            None,
            Idling,
            Walking,
            Sprinting,
            Jumping,
            Uncrouching,
            Sliding,
            Swimming,
            CrouchWalking,
            CrouchIdling,
            Immobile,
        }

        [Flags]
        public enum PlayerEnvironmentState
        {
            None = 0,
            InWater = 1,
            InLava = 2,
            OnIce = 4
        }

        public enum PlayerFishingState
        {
            None,
            Charging,
            Casting,
            Fishing
        }
    }
}
