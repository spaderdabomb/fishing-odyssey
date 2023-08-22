using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Search;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

public partial class Inventory
{
    private int inventoryRows;
    private int inventoryCols;
    public List<InventorySlot> inventorySlots;
    private InventorySlot currentDraggedInventorySlot = null;

    private Vector2 startMousePosition;
    private Vector2 startElementPosition;

    public Inventory(VisualElement root, int inventoryRows, int inventoryCols)
    {
        AssignQueryResults(root);
        this.inventoryRows = inventoryRows;
        this.inventoryCols = inventoryCols;
        InitInventorySlots();
        AddCallbacks();

        // ghostIcon.AddManipulator(new DragAndDropManipulator());
        ghostIcon.RegisterCallback<PointerMoveEvent>(MoveDragHandler);
        ghostIcon.RegisterCallback<PointerUpEvent>(EndDragHandler, TrickleDown.TrickleDown);
    }

    private void AddCallbacks()
    {

    }

    private void InitInventorySlots()
    {
        inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < inventoryRows; i++)
        {
            for (int j = 0; j < inventoryCols; j++)
            {
                VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                InventorySlot inventorySlot = new InventorySlot(inventoryAsset, j + i*inventoryCols);
                inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => BeginDragHandler(evt, inventorySlot));
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
        InventorySlot dragEndSlot = inventorySlots[dragEndIndex];
        InventorySlot dragBeginSlot = inventorySlots[dragBeginIndex];

        if (dragEndSlot.currentItemData == null)
        {
            ItemData dragBeginItemData = dragBeginSlot.currentItemData;
            int itemsRemaining = AddItem(dragBeginItemData, dragEndIndex);
            RemoveItem(dragBeginIndex);
        }
        else
        {
            SwapItems(dragEndIndex, dragBeginIndex);
        }
    }

    public void SwapItems(int dragEndIndex, int dragBeginIndex)
    {

    }

    public void GetItemAtIndex()
    {

    }

    public int GetFirstFreeSlot(ItemData itemData)
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
                     inventorySlots[i].currentItemData.stackCount < inventorySlots[i].currentItemData.maxStackCount)
            {
                slotIndex = i;
                break;
            }
        }

        return slotIndex;
    }

    private void BeginDragHandler(PointerDownEvent evt, InventorySlot inventorySlot)
    {
        currentDraggedInventorySlot = inventorySlot;
        currentDraggedInventorySlot.SetTinted();

        ghostIcon.style.position = Position.Absolute;
        ghostIcon.style.visibility = Visibility.Visible;
        ghostIcon.style.backgroundImage = inventorySlot.currentItemData.itemSprite.texture;

        startMousePosition = evt.position;
        float positionLeft = ghostIcon.WorldToLocal(evt.position).x - ghostIcon.resolvedStyle.width / 2;
        float positionTop = ghostIcon.WorldToLocal(evt.position).y - ghostIcon.resolvedStyle.height / 2;
        ghostIcon.style.left = positionLeft;
        ghostIcon.style.top = positionTop;
        startElementPosition = new Vector2(positionLeft, positionTop);

        ghostIcon.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }

    private void MoveDragHandler(PointerMoveEvent evt)
    {
        if (ghostIcon.HasPointerCapture(evt.pointerId))
        {
            Vector2 displacement = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
            ghostIcon.style.left = startElementPosition.x + displacement.x;
            ghostIcon.style.top = startElementPosition.y + displacement.y;

        }
    }

    private void EndDragHandler(PointerUpEvent evt)
    {
        if (ghostIcon.HasPointerCapture(evt.pointerId))
        {
            InventorySlot closestSlot = GetClosestSlotToRelease();
            if (closestSlot != null)
            {
                Debug.Log(closestSlot.slotIndex);
                MoveItem(closestSlot.slotIndex, currentDraggedInventorySlot.slotIndex);
            }

            ghostIcon.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
            ghostIcon.style.left = 0f;
            ghostIcon.style.top = 0f;
            ghostIcon.style.visibility = Visibility.Hidden;

            currentDraggedInventorySlot.SetTintNormal();
            currentDraggedInventorySlot = null;
            Debug.Log("Ending Drag Handler");
        }
    }

    private InventorySlot GetClosestSlotToRelease()
    {
        InventorySlot closestSlot = null;
        IEnumerable<InventorySlot> slots = inventorySlots.Where(x => x.root.worldBound.Overlaps(ghostIcon.worldBound));
        if (slots.Count() != 0)
        {
            closestSlot = slots.OrderBy(x => Vector2.Distance(x.root.worldBound.position, ghostIcon.worldBound.position)).First();
        }

        return closestSlot;
    }
}
