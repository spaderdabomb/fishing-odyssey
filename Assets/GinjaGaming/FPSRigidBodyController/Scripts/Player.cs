using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PickleMan
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Player : MonoBehaviour
    {
        public Dictionary<Collider, Collision> CollisionData { get; private set; } = new();
        [field: SerializeField] public List<Collider> ActiveTriggers { get; private set; } = new();
        [field: SerializeField] public List<Collider> ActiveColliders { get; private set; } = new();
        [field: SerializeField] public List<Collider> MovingPlatforms { get; private set; } = new();

        private PlayerStates playerStates;
        private PlayerMovement playerMovement;
        private Rigidbody rb;
        private CapsuleCollider capsuleCol;

        public PlayerInputActions playerInputActions;

        private void OnEnable()
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            InputManager.Instance.SetInputActionRef(playerInputActions);
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            playerStates = GetComponent<PlayerStates>();
            playerMovement = GetComponent<PlayerMovement>();
            rb = GetComponent<Rigidbody>();
            capsuleCol = GetComponent<CapsuleCollider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            ActiveTriggers.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            ActiveTriggers.Remove(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollisionData[collision.collider] = collision;
            ActiveColliders.Add(collision.collider);

            if (collision.transform.CompareTag("MovingPlatform"))
                MovingPlatforms.Add(collision.collider);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (CollisionData.ContainsKey(collision.collider))
            {
                CollisionData.Remove(collision.collider);
                ActiveColliders.Remove(collision.collider);

                if (collision.transform.CompareTag("MovingPlatform"))
                    MovingPlatforms.Remove(collision.collider);
            }
        }
    }
}
