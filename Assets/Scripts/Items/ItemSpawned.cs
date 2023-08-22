using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ItemSpawned : InteractingObject
{
    public ItemData itemDataAsset;
    public ItemData itemData;
    protected override string DisplayString { get; set; } = string.Empty; 

    protected override void Start()
    {
        base.Start();
        itemData = Instantiate(itemDataAsset);
        itemData.InitDefaults();
        DisplayString = itemData.displayName + " x" + itemData.stackCount.ToString();
    }

}
