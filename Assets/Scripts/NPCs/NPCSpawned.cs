using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawned : InteractingObject
{
    public NPCData npcDataAsset;
    [ReadOnly] public NPCData npcData;
    protected override string DisplayString { get; set; } = string.Empty;

    protected override void Start()
    {
        base.Start();
        npcData = Instantiate(npcDataAsset);
        DisplayString = "Talk to " + npcData.displayName;
    }
}
