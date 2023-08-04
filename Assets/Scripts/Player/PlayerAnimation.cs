using PickleMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject fishingRodContainer;
    [SerializeField] Animator animator;
    private PlayerStates playerStates;

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
        animator.SetInteger("currentFishingState", (int)playerStates.CurrentFishingState);
    }
}
