using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class CollectFishQuestStep : QuestStep
{
    [SerializeField] private int fishCollected = 0;
    [SerializeField] private int fishToComplete = 2;

    public override void InitializeQuestStep(Quest quest, int stepIndex, string questStepState)
    {
        base.InitializeQuestStep(quest, stepIndex, questStepState);

        object questStepValue = quest.info.questStepInfos[stepIndex].GetValue();
        if (questStepValue is int)
        {
            fishToComplete = (int)questStepValue;
            questStepDescription = $"Collect {fishToComplete} fish";
        }
        else
        {
            Debug.LogError($"Quest Step Value Type mismatch: Object type is {questStepValue.GetType()}, cast type is {fishToComplete.GetType()}");
        }
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onFishCollected += FishCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onFishCollected -= FishCollected;
    }

    private void FishCollected()
    {
        if (fishCollected < fishToComplete)
        {
            fishCollected++;
            UpdateState();
        }

        if (fishCollected >= fishToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = fishCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.fishCollected = System.Int32.Parse(state);
        UpdateState();
    }
}