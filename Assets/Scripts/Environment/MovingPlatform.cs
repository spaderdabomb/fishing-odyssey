using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickleMan
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovingPlatform : MonoBehaviour
    {
        public float speed = 5f;
        public float acceleration = 2f;
        public Vector3[] waypoints;
    
        private Rigidbody rb;
        private int currentWaypointIndex = 0;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Vector3 currentVelocity;

        bool increaseIndex = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            startPosition = transform.position;

            if (waypoints.Length > 0)
                targetPosition = startPosition + waypoints[0];
        }

        private void FixedUpdate()
        {
            if (waypoints.Length == 0)
                return;

            Vector3 currentPosition = transform.position;

            if (Vector3.Distance(currentPosition, targetPosition) <= 0.1f)
            {
                currentWaypointIndex = increaseIndex ? currentWaypointIndex++ : currentWaypointIndex--;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    increaseIndex = false;
                }
                else if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    increaseIndex = true;
                }

                targetPosition = startPosition + waypoints[currentWaypointIndex];
            }

            Vector3 direction = (targetPosition - currentPosition).normalized;
            float currentSpeed = speed * Mathf.Clamp01(Vector3.Distance(currentPosition, targetPosition) / 2f);
            Vector3 targetVelocity = direction * currentSpeed;

            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, acceleration);
        }
    }
}
