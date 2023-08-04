using PickleMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickleMan
{
    public class InAirCheck : MonoBehaviour
    {
        public Dictionary<Collider, Collision> CollisionData { get; private set; } = new();
        [field: SerializeField] public List<Collider> ActiveTriggers { get; private set; } = new();
        [field: SerializeField] public List<Collider> ActiveColliders { get; private set; } = new();

        [HideInInspector] public CapsuleCollider capsuleCol;
        private void Start()
        {
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
        }

        private void OnCollisionExit(Collision collision)
        {
            if (CollisionData.ContainsKey(collision.collider))
            {
                CollisionData.Remove(collision.collider);
                ActiveColliders.Remove(collision.collider);
            }
        }
    }
}
