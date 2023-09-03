using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Search;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class Inventory
{
    public VisualElement root; 
    private int inventoryRows;
    private int inventoryCols;
    public List<InventorySlot> inventorySlots;
    public InventorySlot currentDraggedInventorySlot { get; set; } = null;
    public InventorySlot currentHoverSlot { get; set; } = null;

    public Inventory(VisualElement root, int inventoryRows, int inventoryCols)
    {
        this.root = root;
        AssignQueryResults(root);
        this.inventoryRows = inventoryRows;
        this.inventoryCols = inventoryCols;
        InitInventorySlots();
        AddCallbacks();

        ghostIcon.RegisterCallback<PointerMoveEvent>(InventoryManager.Instance.MoveDragHandler);
        ghostIcon.RegisterCallback<PointerUpEvent>(InventoryManager.Instance.EndDragHandler);
    }

    private void AddCallbacks()
    {
        this.root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void InitInventorySlots()
    {
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
    }

    // Returns number of remaining items in stack
    public int TryAddItem(ItemData itemData)
    {
        int slotIndex = GetFirstFreeSlot(itemData);
        if (slotIndex == -1)
            return itemData.stackCount;

        bool itemsRemainingBool = true;
        int itemsRemaining = 0;
        while (itemsRemainingBool)
        {
            itemsRemaining = AddItem(itemData, slotIndex);
            itemsRemainingBool = (itemsRemaining > 0) ? true : false;

            slotIndex = GetFirstFreeSlot(itemData);
            if (slotIndex == -1)
            {
                break;
            }
        }

        return itemsRemaining;
    }

    private int AddItem(ItemData itemData, int slotIndex)
    {
        InventorySlot inventorySlot = inventorySlots[slotIndex];
        int numItemsRemaining = inventorySlot.AddItemToSlot(itemData);

        return numItemsRemaining;
    }

    public void RemoveItem(int slotIndex)
    {
        InventorySlot inventorySlot = inventorySlots[slotIndex];
        inventorySlot.RemoveItemFromSlot();
    }

    public void MoveItem(int dragEndIndex, int dragBeginIndex)
    {
        if (dragEndIndex == dragBeginIndex)
            return;

        InventorySlot dragEndSlot = inventorySlots[dragEndIndex];
        InventorySlot dragBeginSlot = inventorySlots[dragBeginIndex];
        ItemData dragBeginItemData = InventoryManager.Instance.InstantiateItem(dragBeginSlot.currentItemData.itemDataAsset);

        // If target slot has no items
        if (dragEndSlot.currentItemData == null)
        {
            AddItem(dragBeginItemData, dragEndIndex);
            RemoveItem(dragBeginIndex);
        }
        // If items in both slots are the same itemID
        else if (dragEndSlot.currentItemData.itemID == dragBeginSlot.currentItemData.itemID)
        {
            int totalItemCount = dragEndSlot.currentItemData.stackCount + dragBeginSlot.currentItemData.stackCount;
            bool exeedsStackCount = totalItemCount > dragEndSlot.currentItemData.maxStackCount;
            if (exeedsStackCount)
            {
                SwapItems(dragEndIndex, dragBeginIndex);
            }
            else
            {
                AddItem(dragBeginItemData, dragEndIndex);
                RemoveItem(dragBeginIndex);
            }
        }
        // If both slots have items but are not same itemID
        else
        {
            SwapItems(dragEndIndex, dragBeginIndex);
        }
    }

    public void SwapItems(int dragEndIndex, int dragBeginIndex)
    {
        InventorySlot dragBeginSlot = inventorySlots[dragBeginIndex];
        InventorySlot dragEndSlot = inventorySlots[dragEndIndex];

        if (dragBeginSlot.currentItemData != null && dragEndSlot.currentItemData != null)
        {
            ItemData dragBeginItemData = dragBeginSlot.currentItemData;
            ItemData dragEndItemData = dragEndSlot.currentItemData;
            RemoveItem(dragBeginIndex);
            RemoveItem(dragEndIndex);
            AddItem(dragEndItemData, dragBeginIndex);
            AddItem(dragBeginItemData, dragEndIndex);
        }
    }

    public void TrySplitItem(bool splitHalf)
    {
        if (currentHoverSlot == null)
            return;

        ItemData newItemData = InventoryManager.Instance.InstantiateItem(currentHoverSlot.currentItemData.itemDataAsset);
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

        AddItem(newItemData, firstSlot);
    }

    public void DropItem()
    {
        if (currentHoverSlot == null)
            return;

        InventoryManager.Instance.InstantiateItemSpawned(currentHoverSlot.currentItemData);
        RemoveItem(currentHoverSlot.slotIndex);
    }

    public void GetItemAtIndex()
    {

    }

    public int GetFirstFreeSlot(ItemData itemData, bool mergeSameItems = true)
    {
        int slotIndex = -1;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            // Add item to free slot
            if (!inventorySlots[i].slotFilled)
            {
                slotIndex = i;
                break;
            }
            // Unfilled slot with identical item
            else if (inventorySlots[i].currentItemData.itemID == itemData.itemID && 
                     inventorySlots[i].currentItemData.stackCount < inventorySlots[i].currentItemData.maxStackCount && 
                     mergeSameItems)
            {
                slotIndex = i;
                break;
            }
        }

        return slotIndex;
    }

    public InventorySlot GetClosestSlotToRelease()
    {
        InventorySlot closestSlot = null;
        IEnumerable<InventorySlot> slots = inventorySlots.Where(x => x.root.worldBound.Overlaps(ghostIcon.worldBound));
        if (slots.Count() != 0)
        {
            closestSlot = slots.OrderBy(x => Vector2.Distance(x.root.worldBound.position, ghostIcon.worldBound.position)).First();
        }

        return closestSlot;
    }

    // WARNING: Only works with pointer/mouse events, does not work with general Input.mousePosition
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

    public VisualElement GetGhostIconRef()
    {
        return ghostIcon;
    }

    public Label GetGhostIconLabelRef()
    {
        return ghostIconLabel;
    }

    public void SetCurrentSlot(InventorySlot newSlot)
    {
        currentHoverSlot = newSlot;
    }
}
