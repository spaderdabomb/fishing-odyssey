using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ink.Runtime;

[CreateAssetMenu(fileName = "NPCData", menuName = "Fishing Odyssey/NPCs/NPCData")]
public class NPCData : SerializedScriptableObject
{
    [Header("Details")]
    [ReadOnly] public string npcID;
    public string baseName;
    public string displayName;
    public string description;

    //public TextAsset npcStory;
    public TextAsset npcStoryAsset;
    [ReadOnly] public Story currentStory = null;
    public int currentStoryIndex = 0;

    private void OnValidate()
    {
#if UNITY_EDITOR
        npcID = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

}

public static class NPCExtensions
{
    public static bool DoesKnotExist(string knotName, NPCData npcData)
    {
        Story _inkStory = new Story(npcData.npcStoryAsset.text);
        Dictionary<string, Ink.Runtime.Object> knotDict = _inkStory.mainContentContainer.namedOnlyContent;

        return knotDict.ContainsKey(knotName);
    }
}
