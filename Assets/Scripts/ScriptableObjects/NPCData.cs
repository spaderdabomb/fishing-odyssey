using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Fishing Odyssey/NPCs/NPCData")]
public class NPCData : SerializedScriptableObject
{
    [Header("Details")]
    [ReadOnly] public string npcID;
    public string baseName;
    public string displayName;
    public string description;

    public TextAsset npcStory;
}
