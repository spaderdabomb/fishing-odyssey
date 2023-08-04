using Bitgem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemData", menuName = "Fishing Odyssey/Item")]
public class ItemData : ScriptableObject
{
    public GameObject item3DPrefab;
    public GameObject item2DPrefab;
    public GameObject itemHeldPrefab;

    public ItemType itemType = ItemType.None;
    public ItemRarity itemRarity = ItemRarity.None;
    public ItemCategory itemCategories = ItemCategory.None;

    public void InitDefaults()
    {

    }

    public enum ItemRarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Special,
        Epic,
        Legendary,
        Mythic
    }

    public enum ItemType
    {
        None,
        FishingRod,
        FishingLine,
        Hook,
        Bait,
        Sinkers,
        FishingNetSmall,
        FishingNetBig,
        HealthPotion
    }

    [Flags]
    public enum ItemCategory
    {
        None,
        Wieldable,
        Fishing,
        Consumable
    }
}
