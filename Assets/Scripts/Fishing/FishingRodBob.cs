using PickleMan;
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

    public void OnInstantiate(PlayerData playerData)
    {
        rb.position = GameManager.Instance.bobStartLocation.transform.position;

        Vector3 lookDirection = GameManager.Instance.playerCamera.transform.forward;
        lookDirection = lookDirection.normalized;
        lookDirection = new Vector3(lookDirection.x, lookDirection.y, lookDirection.z);
        rb.velocity = lookDirection * playerData.FishPowerCurrent * playerData.currentFishingRod.fishingRodStats.maxCastDistance * 0.0075f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && !hitWater)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            hitWater = true;

            GameManager.Instance.BobHitWater(gameObject, other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);

        GameManager.Instance.StoppedFishing();
    }
}
