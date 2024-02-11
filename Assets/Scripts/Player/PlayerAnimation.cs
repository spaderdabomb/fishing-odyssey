using PickleMan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject fishingRodContainer;
    [SerializeField] Animator animator;
    private PlayerStates playerStates;
    public PlayerFishingAnimationState PlayerFishingAnimationState { get; set; }

    private void OnEnable()
    {
        GameEventsManager.Instance.fishingEvents.onCastRod += CastRod;
        GameEventsManager.Instance.fishingEvents.onBobHitWater += BobHitWater;
        GameEventsManager.Instance.fishingEvents.onFishHooked += FishHooked;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing += StoppedFishing;

        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted += BeatNoteSubmitted;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.fishingEvents.onCastRod -= CastRod;
        GameEventsManager.Instance.fishingEvents.onBobHitWater -= BobHitWater;
        GameEventsManager.Instance.fishingEvents.onFishHooked -= FishHooked;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing -= StoppedFishing;

        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted -= BeatNoteSubmitted;
    }

    private void CastRod(GameObject @object)
    {
        PlayerFishingAnimationState = PlayerFishingAnimationState.Casting;
    }

    private void BobHitWater(GameObject currentBob, GameObject water)
    {
        PlayerFishingAnimationState = PlayerFishingAnimationState.Fishing;
    }

    private void FishHooked(FishData data)
    {
        animator.SetTrigger("fishHookedTrigger");
    }

    private void StoppedFishing()
    {
        print("stopping fishing");

        animator.ResetTrigger("fishHookedTrigger");
        animator.ResetTrigger("rodPulledTrigger");

        PlayerFishingAnimationState = PlayerFishingAnimationState.None;
    }

    private void BeatNoteSubmitted(BeatNoteRegion beatNoteRegion)
    {
        animator.SetTrigger("rodPulledTrigger");
    }

    void Start()
    {
        playerStates = GetComponent<PlayerStates>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string currentClipName = currentClipInfo[0].clip.name;

        animator.SetInteger("currentMoveState", (int)playerStates.CurrentUniqueMovementState);
        animator.SetInteger("currentFishingState", (int)PlayerFishingAnimationState);
    }
}

public enum PlayerFishingAnimationState
{
    None = 0,
    Charging = 1,
    Casting = 2,
    Fishing = 3,
    HookFish = 4,
    PullRod = 5,
}
