using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseSlotContainer
{
    public VisualElement root;
    private int inventoryRows;
    private int inventoryCols;
    public List<InventorySlot> inventorySlots;
    public InventorySlot currentDraggedInventorySlot { get; set; } = null;
    public InventorySlot currentHoverSlot { get; set; } = null;

    public BaseSlotContainer(VisualElement root, int inventoryRows, int inventoryCols)
    {
        this.root = root;
        this.inventoryRows = inventoryRows;
        this.inventoryCols = inventoryCols;
        AddCallbacks();
    }

    private void AddCallbacks()
    {
        this.root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
}
