using UnityEngine.UIElements;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;
using JSAM;

public partial class MissionContainer : ITabInterface
{
    public VisualElement root;
    public QuestInfoSO questInfo;
    public int currentIndex;
    private const string lightSelector = "mission-container-light";
    private const string darkSelector = "mission-container-dark";

    public MissionContainer(VisualElement root, QuestInfoSO newQuestInfo, int currentIndex)
    {
        this.root = root;
        this.questInfo = newQuestInfo;
        this.currentIndex = currentIndex;
        AssignQueryResults(root);
        InitMissionContainer();
        RegisterCallbacks();
    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
        GameEventsManager.Instance.questEvents.onStartQuest += StartedQuest;
    }

    public void UnregisterCallbacks()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= StartedQuest;
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);

    }

    private void InitMissionContainer()
    {
        if (currentIndex == 0)
        {
            tabRoot.value = true;
        }

        tabRoot.tabIndex = currentIndex;
        missionNameLabel.text = questInfo.displayName;

        if (currentIndex % 2 == 1)
        {
            tabRoot.RemoveFromClassList(lightSelector);
            tabRoot.AddToClassList(darkSelector);
        }
    }

    public void TabIndexChanged(ChangeEvent<bool> value)
    {
        if (!value.newValue) { return; }

        UIGameManager.Instance.uiGameScene.menuMissions.MissionContainerSelected(currentIndex);
        AudioManager.PlaySound(MainAudioLibrarySounds.ConfirmTick);
    }

    public void StartedQuest(string questID)
    {
        if (questInfo.questID == questID)
        {
            questStatusIcon.style.backgroundImage = UIGameManager.Instance.questStatusIconLight;
        }
    }

    public void SetTabSelectedValue(bool value)
    {
        tabRoot.value = value;
    }

    public void OnHover(PointerEnterEvent evt)
    {
        AudioManager.PlaySound(MainAudioLibrarySounds.WoodenTick);
    }
}
