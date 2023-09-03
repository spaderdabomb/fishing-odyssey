using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "ItemDataGeneral", menuName = "Fishing Odyssey/Items/ItemDataGeneral")]
public class ItemDataGeneral : SerializedScriptableObject
{
    public Dictionary<ItemType, string> itemTypeDisplayNameDict = new Dictionary<ItemType, string>();

    private void OnEnable()
    {

    }
}
