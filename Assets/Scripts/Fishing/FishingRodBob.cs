using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodBob : MonoBehaviour
{
    private bool hitWater = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && !hitWater)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            hitWater = true;

            GameManager.Instance.BobHitWater(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);

        GameManager.Instance.DestroyCurrentBob();
    }
}
