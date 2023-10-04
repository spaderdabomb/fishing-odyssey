using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQuestStep : QuestStep
{
    [SerializeField] private NPCData npcData;

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onNPCDialogueFinish += NPCDialogueFinish;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onNPCDialogueFinish -= NPCDialogueFinish;
    }

    private void NPCDialogueFinish(string npcID)
    {
        print("dialogue finished");

        if (npcID == npcData.npcID)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
/*        string state = fishCollected.ToString();
        ChangeState(state);*/
    }

    protected override void SetQuestStepState(string state)
    {
        UpdateState();
    }
}