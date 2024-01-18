using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnedExclamation : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private FaceCamera faceCamera;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        GameEventsManager.Instance.fishingEvents.onFishSpawned += FishSpawned;
        GameEventsManager.Instance.fishingEvents.onFishHooked += FishHooked;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing += StoppedFishing;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.fishingEvents.onFishSpawned -= FishSpawned;
        GameEventsManager.Instance.fishingEvents.onFishHooked -= FishHooked;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing -= StoppedFishing;
    }

    private void Start()
    {
        HideElements();
    }

    private void FishSpawned(FishData fishData, GameObject currentBob)
    {
        faceCamera.enabled = true;
        spriteRenderer.enabled = true;
        animator.enabled = true;

        AnimationClip currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        animator.Play(currentClip.name, 0, 0f);

        transform.position = currentBob.transform.position + offset;
    }

    private void FishHooked(FishData fishData)
    {
        HideElements();
    }

    private void StoppedFishing()
    {
        HideElements();
    }

    private void HideElements()
    {
        faceCamera.enabled = false;
        spriteRenderer.enabled = false;
        animator.enabled = false;
    }
}
