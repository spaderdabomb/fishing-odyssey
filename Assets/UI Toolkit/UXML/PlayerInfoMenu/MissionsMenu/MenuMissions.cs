using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System;

public partial class MenuMissions
{
    public VisualElement root;
    public List<MissionContainer> missionContainerList;
    private int currentIndexSelected = 0;

    private Dictionary<string, int> questIDToIndexDict;
    public MenuMissions(VisualElement root)
    {
        this.root = root;
        AssignQueryResults(root);
        SetCallbacks();
        InitMenuMissions();
    }

    private void SetCallbacks()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;
    }

    private void RemoveCallbacks()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

    }

    private void InitMenuMissions()
    {
        questIDToIndexDict = new Dictionary<string, int>();
        missionContainerList = new();

        ClearMissionContainers();
        InitMissionContainers();
        UpdateQuestUI(0);
    }

    private void InitMissionContainers()
    {
        Dictionary<string, Quest> allVisibleQuests = QuestManager.Instance.GetAllVisibleQuests();
        int i = 0;
        foreach (Quest quest in allVisibleQuests.Values)
        {
            VisualElement missionContainerTree = UIGameManager.Instance.missionContainer.CloneTree();
            missionsGroupBox.Add(missionContainerTree);
            MissionContainer newMissionContainer = new MissionContainer(missionContainerTree, quest.info, i);
            missionContainerList.Add(newMissionContainer);
            questIDToIndexDict.Add(quest.info.questID, i);
            i++;
        }
    }

    private void ClearMissionContainers()
    {
        foreach (MissionContainer missionContainer in missionContainerList)
        {
            missionContainer.UnregisterCallbacks();
        }

        missionsGroupBox.Clear();
        missionContainerList.Clear();
        questIDToIndexDict.Clear();
    }

    public void MissionContainerSelected(int index)
    {
        // Update container selected values
        currentIndexSelected = index;
        foreach (MissionContainer missionContainer in missionContainerList)
        {
            if (missionContainer.currentIndex == index)
            {
                missionContainer.SetTabSelectedValue(true);
            }
            else
            {
                missionContainer.SetTabSelectedValue(false);
            }
        }

        UpdateQuestUI(index);
    }

    private void AdvanceQuest(string questID)
    {
        if (!questIDToIndexDict.ContainsKey(questID))
            return;

        int missionContainerIndex = questIDToIndexDict[questID];
        MissionContainer currentMissionContainer = missionContainerList[missionContainerIndex];

        UpdateQuestStep(currentMissionContainer);
    }

    private void FinishQuest(string questID)
    {
        ClearMissionContainers();
        InitMissionContainers();
        UpdateQuestUI(0);
    }

    private void UpdateQuestUI(int index)
    {
        MissionContainer currentMissionContainer = missionContainerList[index];
        missionHeaderNameLabel.text = currentMissionContainer.questInfo.displayName;
        currentMissionsLocationLabel.text = currentMissionContainer.questInfo.biomeLocation.GetDescription();
        questDescriptionLabel.text = currentMissionContainer.questInfo.questDescription;

        UpdateRewardsUI(index);
        UpdateQuestStep(currentMissionContainer);
    }

    private void UpdateRewardsUI(int index)
    {
        rewardsSlotContainer.Clear();

        int i = 0;
        int rewardsCount = missionContainerList[index].questInfo.rewards.Count;
        BaseSlotContainer rewardsContainer = new BaseSlotContainer(rewardsSlotContainer, 1, rewardsCount);
        foreach (var kvp in missionContainerList[index].questInfo.rewards)
        {
            VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
            InventorySlot inventorySlot = new InventorySlot(inventoryAsset, i, rewardsContainer);
            kvp.Key.stackCount = kvp.Value;
            inventorySlot.AddItemToSlot(kvp.Key);
            rewardsSlotContainer.Add(inventoryAsset);
            i++;
        }
    }

    private void UpdateQuestStep(MissionContainer currentMissionContainer)
    {
        if (currentMissionContainer.currentIndex != currentIndexSelected)
            return;

        // Check if quest is finished, if so, return
        Quest currentQuest = QuestManager.Instance.questMap[currentMissionContainer.questInfo.questID];
        if (currentQuest.state == QuestState.FINISHED)
            return;

        QuestStep currentQuestStep = currentMissionContainer.questInfo.questStepPrefabs[currentQuest.currentQuestStepIndex].GetComponent<QuestStep>();
        int totalQuestSteps = currentMissionContainer.questInfo.questStepPrefabs.Length;

        questStepLabel.text = QuestManager.Instance.GetCurrentQuestStepDescription(currentQuest);
        missionStepProgressLabel.text = currentQuest.currentQuestStepIndex.ToString() + "/" + (totalQuestSteps - 1).ToString();
        progressBar.style.width = Length.Percent((currentQuest.currentQuestStepIndex / (totalQuestSteps - 1)) * 100f);
    }
}
