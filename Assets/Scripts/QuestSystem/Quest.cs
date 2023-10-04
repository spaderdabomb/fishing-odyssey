using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Ink.Runtime;
using Dialogue;

public class Quest
{
    // static info
    public QuestInfoSO info;

    // state info
    public QuestState state;
    public int currentQuestStepIndex { get; private set; }
    private QuestStepState[] questStepStates;
    private GameObject currentQuestStep;

    public Quest(QuestInfoSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        info.questStory = new Story(info.questStoryAsset.text);
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.info.questID);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
        currentQuestStep = null;
        DialogueManager.Instance.globalsStory.variablesState[info.questID + "State"] = currentQuestStepIndex;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            currentQuestStep = UnityEngine.Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
            QuestStep questStep = currentQuestStep.GetComponent<QuestStep>();
            questStep.InitializeQuestStep(this, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                + "there's no current step: QuestId=" + info.questID + ", stepIndex=" + currentQuestStepIndex);
        }
        return questStepPrefab;
    }

    public GameObject GetInstantiatedQuestStep()
    {
        return currentQuestStep;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
        }
        else
        {
            Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range: "
                + "Quest Id = " + info.questID + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }
}