using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    public Dictionary<string, Quest> questMap;

    // quest start requirements
    private int currentPlayerLevel;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found more than one {this} in the scene.");
        }
        Instance = this;

        questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        // GameEventsManager.Instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        // GameEventsManager.Instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {

        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.questID).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.questID, QuestState.CAN_START);
            }
        }
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.questID, QuestState.IN_PROGRESS);
        DialogueManager.Instance.globalsStory.variablesState[quest.info.questID + "State"] = quest.currentQuestStepIndex;
        // quest.info.questStory.variablesState[quest.info.questID + "State"] = quest.currentQuestStepIndex;
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.questID, QuestState.CAN_FINISH);
            GameEventsManager.Instance.inputEvents.SubmitPressed();
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.questID, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        print("Claiming rewards, nothing here yet");
        // GameEventsManager.Instance.goldEvents.GoldGained(quest.info.goldReward);
        // GameEventsManager.Instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("ScriptableObjects/Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.questID))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.questID);
            }
            idToQuestMap.Add(questInfo.questID, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    public string GetCurrentQuestStepDescription(Quest quest)
    {
        return quest.state == QuestState.IN_PROGRESS ? quest.GetInstantiatedQuestStep().GetComponent<QuestStep>().questStepDescription 
                                                     : quest.info.questStartDescription;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);
            // saving to PlayerPrefs is just a quick example for this tutorial video,
            // you probably don't want to save this info there long-term.
            // instead, use an actual Save & Load system and write to a file, the cloud, etc..
            // PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.questID + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.questID) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.questID);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.questID + ": " + e);
        }
        return quest;
    }

    public Dictionary<string, Quest> GetAllVisibleQuests()
    {
        Dictionary<string, Quest> allVisibleQuestsDict = new();

        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.FINISHED)
                continue;

            allVisibleQuestsDict[quest.info.questID] = quest;
        }

        return allVisibleQuestsDict;
    }
}