using Bitgem.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static ItemData;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "ItemData", menuName = "Fishing Odyssey/Items/ItemData")]
public class ItemData : SerializedScriptableObject
{
    public ItemData itemDataAsset;

    [Header("Details")]
    [ReadOnly] public string itemID;
    public string baseName;
    public string displayName;
    public string description;
    public int stackCount;
    public int maxStackCount = 50;
    public int baseSellValue = 1;

    [Header("Classification")]
    public ItemType itemType = ItemType.None;
    public ItemRarity itemRarity = ItemRarity.None;
    public ItemCategory itemCategories = ItemCategory.None;

    [Header("Assets")]
    public GameObject item3DPrefab;
    public Sprite itemSprite;
    public GameObject itemHeldPrefab;

    [Header("Stats")]
    public List<IGenericItemStat> itemStatList;
    [HideInInspector] public ItemStats itemStats;

    private void OnEnable()
    {
        itemID = baseName + itemRarity.ToString();

        itemStats = new();
        foreach (IGenericItemStat genericItemStat in itemStatList)
        {
            itemStats.SetStat(genericItemStat.ItemStat, genericItemStat.GetValue());
        }
    }

    public interface IGenericItemStat
    {
        ItemStat ItemStat { get; }
        object GetValue();
    }

    [Serializable]
    public class IntValue : IGenericItemStat
    {
        public ItemStat itemStat;
        public int value;

        public ItemStat ItemStat => itemStat;
        public object GetValue() => value;
    }

    [Serializable]
    public class FloatValue : IGenericItemStat
    {
        public ItemStat itemStat;
        public float value;

        public ItemStat ItemStat => itemStat;
        public object GetValue() => value;
    }

    [Serializable]
    public class StatTypeValue : IGenericItemStat
    {
        public ItemStat itemStat;
        public FishingMethod value;

        public ItemStat ItemStat => itemStat;
        public object GetValue() => value;
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
        FishHook,
        Bait,
        Sinker,
        Lure,
        SmallFishingNet,
        BigFishingNet,
        Harpoon,
        Food,
        Potion,
        Boots,
        Gloves,
        Hat,
        Body,
        Legs,
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
