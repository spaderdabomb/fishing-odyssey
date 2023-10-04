using Ink.Runtime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : SerializedScriptableObject
{
    [field: SerializeField] public string questID { get; private set; }

    [Header("General")]
    public string displayName;
    public string questDescription;
    public string questStartDescription;
    public BiomeType biomeLocation;
    public TextAsset questStoryAsset;
    [HideInInspector] public Story questStory;

    [Header("Requirements")]
    public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;
    public List<IGenericQuestStep> questStepInfos;


    [Header("Rewards")]
    public Dictionary<ItemData, int> rewards;
    public int goldReward;
    public int experienceReward;

    // ensure the id is always the name of the Scriptable Object asset
    private void OnValidate()
    {
#if UNITY_EDITOR
        questID = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

public static class QuestInfoExtensions
{
    private static Dictionary<string, QuestInfoSO> allQuestInfoSO = new();

    public static QuestInfoSO GetQuestInfoByID(string uniqueID)
    {
        if (allQuestInfoSO.Count <= 0)
        {
            var quests = Resources.LoadAll<QuestInfoSO>("ScriptableObjects/Quests");
            foreach (var quest in quests)
            {
                allQuestInfoSO.Add(quest.questID, quest);
            }
        }

        if (allQuestInfoSO.TryGetValue(uniqueID, out QuestInfoSO questData))
        {
            return questData;
        }

        Debug.LogWarning($"Item not found: {uniqueID}");
        return null;
    }

    public static Dictionary<string, QuestInfoSO> GetAllQuestData()
    {
        if (allQuestInfoSO.Count <= 0)
        {
            var quests = Resources.LoadAll<QuestInfoSO>("ScriptableObjects/Quests");
            foreach (var quest in quests)
            {
                allQuestInfoSO.Add(quest.questID, quest);
            }
        }

        return allQuestInfoSO;
    }
}

public interface IGenericQuestStep
{
    object GetValue();
}

[Serializable]
public class CollectItemQuestStep : IGenericQuestStep
{
    public ItemData itemData;
    public int myInt;

    public object GetValue() => myInt;
}

[Serializable]
public class CollectItemTypeQuestStep : IGenericQuestStep
{
    public ItemData.ItemType itemData;
    public int myInt;
    public object GetValue() => myInt;

}

[Serializable]
public class CollectFishTypeQuestStep : IGenericQuestStep
{
    public FishType fishType;
    public int myInt;
    public object GetValue() => myInt;
}

[Serializable]
public class NPCDialogueQuestStep : IGenericQuestStep
{
    public NPCData npc;
    public object GetValue() => npc;

}