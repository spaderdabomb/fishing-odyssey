using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ItemSpawned : InteractingObject
{
    public ItemData itemDataAsset;
    [ReadOnly] public ItemData itemData;

    private bool itemDataInitialized = false;
    protected override string DisplayString { get; set; } = string.Empty;

    private void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
            print($"Item {itemData.displayName} does not have a default layer assigned, assigning to 'item'");
        }
    }

    protected override void Start()
    {
        base.Start();

        if (!itemDataInitialized)
        {
            InitItemData();
            itemDataInitialized = true;
        }
    }

    public void InitItemData()
    {
        itemData = Instantiate(itemDataAsset);
        DisplayString = itemData.displayName + " x" + itemData.stackCount.ToString();
        itemDataInitialized = true;
    }

}
