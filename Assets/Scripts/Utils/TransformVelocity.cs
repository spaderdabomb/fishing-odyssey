using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PickleMan
{
    public abstract class TransformVelocity : MonoBehaviour
    {
        private Vector3 previousPosition;
        private float deltaTime;
        public Vector3 Velocity { get; private set; }

        protected void UpdateVelocity()
        {
            if (transform == null) return;

            Vector3 currentPosition = transform.position;
            Velocity = (currentPosition - previousPosition) / deltaTime;

            previousPosition = currentPosition;
            deltaTime = Time.deltaTime;
        }
    }
}
