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
    [Header("Details")]
    [ReadOnly] public string itemID = Guid.NewGuid().ToString();
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
        itemStats = new();
        foreach (IGenericItemStat genericItemStat in itemStatList)
        {
            itemStats.SetStat(genericItemStat.ItemStat, genericItemStat.GetValue());
        }
    }

    public void SetItemDataToBaseItemData(BaseItemData baseItemData)
    {
        itemID = baseItemData.itemID;
        stackCount = baseItemData.stackCount;
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
        None = 0,
        // Fishing techniques
        FishingRod = 1,
        SmallFishingNet = 2,
        BigFishingNet = 3,
        Harpoon = 4,
        // Misc
        Food = 5,
        Potion = 6,
        // Outfit
        Boots = 7,
        Gloves = 8,
        Head = 9,
        Body = 10,
        Legs = 11,
        Cape = 12,
        // Tackle
        Hook = 13,
        Bait = 14,
        Line = 15,
        Reel = 16,
        Lure = 17,
        Special = 18,
        // Acccessories
        Amulet = 19,
        Ring = 20,
        Socks = 21,
        Badge = 22,
        Tattoo = 23,
        Earrings = 24,
    }

    [Flags]
    public enum ItemCategory
    {
        None = 0,
        Wieldable = 1,
        Fishing = 2,
        Consumable = 4,
        Weapon = 8,
        Outfit = 16,
        Accessory = 32,
        Tackle = 64,
    }
}

public class BaseItemData
{
    public string itemID;
    public int stackCount;

    public BaseItemData(ItemData itemData)
    {
        itemID = itemData.itemID;
        stackCount = itemData.stackCount;
    }
}

public static class ItemExtensions
{
    private static readonly Dictionary<string, ItemData> _items = new Dictionary<string, ItemData>();
    public static ItemData GetItemData(string uniqueID)
    {
        if (_items.Count <= 0)
        {
            var items = Resources.LoadAll<ItemData>("ScriptableObjects/Items");
            foreach ( var item in items )
            {
                _items.Add(item.itemID, item);
            }
        }

        if (_items.TryGetValue(uniqueID, out ItemData itemData))
        {
            return itemData;
        }

        Debug.LogWarning($"Item not found: {uniqueID}");
        return null;
    }

    public static ItemData GetItemDataInstantiated(this ItemData itemData)
    {
        ItemData originalItemData = GetItemData(itemData.itemID);
        if (originalItemData == null)
            return null;

        ItemData spawnedItemData = ScriptableObject.Instantiate(originalItemData);
        Debug.Log(spawnedItemData.stackCount);
        return spawnedItemData;
    }

    public static ItemData CloneItemData(this ItemData data)
    {
        ItemData spawnedData = ScriptableObject.Instantiate(data);
        return spawnedData;
    }
}