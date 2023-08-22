using Bitgem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemData", menuName = "Fishing Odyssey/Item")]
public class ItemData : ScriptableObject
{
    [Header("Details")]
    [ReadOnly] public string itemID;
    public string baseName;
    public string displayName;
    public string description;
    public int stackCount;
    public int maxStackCount = 50;

    [Header("Classification")]
    public ItemType itemType = ItemType.None;
    public ItemRarity itemRarity = ItemRarity.None;
    public ItemCategory itemCategories = ItemCategory.None;

    [Header("Assets")]
    public GameObject item3DPrefab;
    public Sprite itemSprite;
    public GameObject itemHeldPrefab;

    public void InitDefaults()
    {
        itemID = baseName + itemRarity.ToString();
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
        HealthPotion,
        Food,
    }

    [Flags]
    public enum ItemCategory
    {
        None = 0,
        Wieldable = 1,
        Fishing = 2,
        Consumable = 4,
        Weapon = 8,
    }
}
