using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    public string questStepDescription;
    private bool isFinished = false;
    private int stepIndex;
    private Quest quest;

    public virtual void InitializeQuestStep(Quest quest, int stepIndex, string questStepState)
    {
        this.quest = quest;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            GameEventsManager.Instance.questEvents.AdvanceQuest(quest.info.questID);
            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        GameEventsManager.Instance.questEvents.QuestStepStateChange(quest.info.questID, stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}