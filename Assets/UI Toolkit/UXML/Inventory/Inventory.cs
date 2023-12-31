using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using System.Linq;

public partial class Inventory : BaseSlotContainer
{
    public BaseItemData[] inventoryItemData;
    public Inventory(VisualElement root, int inventoryRows, int inventoryCols) : base(root, inventoryRows, inventoryCols)
    {
        AssignQueryResults(root);
        InitInventorySlots();
    }

    private void InitInventorySlots()
    {
        // Init slots
        inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < inventoryRows; i++)
        {
            for (int j = 0; j < inventoryCols; j++)
            {
                VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                InventorySlot inventorySlot = new InventorySlot(inventoryAsset, j + i*inventoryCols, this);
                inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
                inventorySlots.Add(inventorySlot);
                inventoryContainer.Add(inventoryAsset);
            }
        }

        // Load item data
        //DataManager.Instance.inventoryItemData = DataManager.Instance.Load(nameof(DataManager.Instance.inventoryItemData), DataManager.Instance.inventoryItemData);

        inventoryItemData = ES3.Load(nameof(inventoryItemData), defaultValue: inventoryItemData);
        for (int i = 0; i < DataManager.Instance.inventoryItemData.Length; i++)
        {
            BaseItemData baseItem = DataManager.Instance.inventoryItemData[i];
            if (baseItem != null)
            {
                ItemData itemDataAsset = ItemExtensions.GetItemData(baseItem.itemID);
                ItemData newItem = itemDataAsset.GetItemDataInstantiated();
                newItem.SetItemDataToBaseItemData(baseItem);
                AddItem(newItem, inventorySlots[i]);
            }
        }
    }

    public void TrySplitItem(bool splitHalf)
    {
        if (!ItemExistsInCurrentSlot())
            return;

        ItemData newItemData = currentHoverSlot.currentItemData.CloneItemData();
        int firstSlot = GetFirstFreeSlot(newItemData, mergeSameItems: false);
        if (firstSlot == -1 || currentHoverSlot.currentItemData.stackCount == 1)
            return;

        int newStackCount;
        if (splitHalf)
            newStackCount = Mathf.CeilToInt(currentHoverSlot.currentItemData.stackCount / 2f);
        else
            newStackCount = 1;

        int oldStackCount = currentHoverSlot.currentItemData.stackCount - newStackCount;
        currentHoverSlot.SetStackCount(oldStackCount);
        newItemData.stackCount = newStackCount;

        AddItem(newItemData, inventorySlots[firstSlot]);
    }

    public void DropItem()
    {
        if (!ItemExistsInCurrentSlot())
            return;

        InventoryManager.Instance.InstantiateItemSpawned(currentHoverSlot.currentItemData);
        RemoveItem(currentHoverSlot);
        AudioManager.PlaySound(MainAudioLibrarySounds.ItemDrop);
    }

    public bool ItemExistsInCurrentSlot()
    {
        if (currentHoverSlot == null)
            return false;

        if (currentHoverSlot.currentItemData == null)
            return false;

        return true;
    }

    public void GetItemAtIndex()
    {

    }

    public override bool CanMoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
    {
        // If base method can't move item, return false
        if (!base.CanMoveItem(dragEndSlot, dragBeginSlot))
        {
            return false;
        }

        // If beginning slot is not a gear slot or we're dragging to an empty slot, return true
        if (dragBeginSlot is not GearSlot || dragEndSlot.currentItemData == null)
            return true;
        
        // Gear slot checks
        GearSlot dragBeginGearSlot = (GearSlot)dragBeginSlot;

        // Swapping from gear container to inventory container
        bool canMoveItem = true;
        bool isSwappingNonIdenticalItem = (dragBeginGearSlot.currentItemData != null && dragBeginGearSlot.currentItemData.itemID != dragEndSlot.currentItemData.itemID);
        if (isSwappingNonIdenticalItem)
        {
            bool validItemType = dragBeginGearSlot.itemType == dragEndSlot.currentItemData.itemType;
            bool validHandsType = InventoryManager.Instance.IsValidHandsSlotItem(dragEndSlot.currentItemData);
            bool isHandsContainer = dragBeginGearSlot.gearContainer.gearContainerType == GearContainerType.Hands;

            canMoveItem = validItemType || (validHandsType && isHandsContainer);
        }

        return canMoveItem;
    }

    public InventorySlot GetCurrentSlotMouseOver(PointerMoveEvent evt)
    {
        InventorySlot currentSlot = null;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.root.worldBound.Contains(evt.position))
            {
                currentSlot = slot;
                break;
            }
        }

        return currentSlot;
    }

    public void SaveData()
    {
        ES3.Save(nameof(inventoryItemData), inventoryItemData);
    }
}
