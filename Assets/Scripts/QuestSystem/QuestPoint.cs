using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;
    [SerializeField] private GameObject[] npcsInQuest;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;
    [SerializeField] private bool nextStepPoint = true;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.questID;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.Instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.Instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        }

        // start or finish a quest
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManager.Instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManager.Instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.questID.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player near");

            playerIsNear = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}