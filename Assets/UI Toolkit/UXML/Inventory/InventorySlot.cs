using UnityEngine.UIElements;
using UnityEngine;

public partial class InventorySlot : VisualElement
{
    public VisualElement root;
    public ItemData currentItemData;
    public Inventory parentContainer;
    public int slotIndex;
    public bool slotFilled = false;

    private Color labelColorDefault;
    private Color iconTintColorDefault;
    private Color slotContainerColorDefault;
    public InventorySlot(VisualElement newRoot, int slotIndex, Inventory parentContainer)
    {
        this.root = newRoot;
        this.slotIndex = slotIndex;
        this.parentContainer = parentContainer;
        AssignQueryResults(root);
        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        this.root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        root.RegisterCallback<PointerEnterEvent>(PointerEnterInventorySlot);
        root.RegisterCallback<PointerLeaveEvent>(PointerLeaveInventorySlot);
    }

    private void UnRegisterCallbacks()
    {

    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        labelColorDefault = itemCountLabel.resolvedStyle.color;
        iconTintColorDefault = slotIcon.resolvedStyle.unityBackgroundImageTintColor;
        slotContainerColorDefault = slotContainer.resolvedStyle.unityBackgroundImageTintColor;
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    // Returns number of items in stack remaining
    public int AddItemToSlot(ItemData itemData)
    {
        int itemsRemaining = 0;
        if (currentItemData != null)
        {
            int stackCountRemaining = currentItemData.maxStackCount - currentItemData.stackCount;
            if (itemData.stackCount > stackCountRemaining)
            {
                currentItemData.stackCount = currentItemData.maxStackCount;
                itemsRemaining = itemData.stackCount - stackCountRemaining;
                itemData.stackCount = itemsRemaining;
            }
            else
            {
                currentItemData.stackCount += itemData.stackCount;
                itemData.stackCount = 0;
            }
        }
        else
        {
            currentItemData = itemData;
        }

        slotFilled = true;
        SetSlotUI();

        return itemsRemaining;
    }

    public void SetStackCount(int newCount)
    {
        currentItemData.stackCount = newCount;
        itemCountLabel.text = newCount.ToString();
    }

    public void RemoveItemFromSlot()
    {
        currentItemData = null;
        slotIcon.style.backgroundImage = null;
        itemCountLabel.text = string.Empty;
        slotFilled = false;
    }

    private void SetSlotUI()
    {
        slotIcon.style.backgroundImage = currentItemData.itemSprite.texture;
        itemCountLabel.text = currentItemData.stackCount.ToString();
    }

    public void SetTinted()
    {
        slotIcon.style.unityBackgroundImageTintColor = GameManager.Instance.gameData.standardTintColor;
        itemCountLabel.style.color = GameManager.Instance.gameData.standardTintColor;
        slotContainer.style.unityBackgroundImageTintColor = GameManager.Instance.gameData.standardTintColor;
    }

    public void SetTintNormal()
    {
        slotIcon.style.unityBackgroundImageTintColor = iconTintColorDefault;
        itemCountLabel.style.color = labelColorDefault;
        slotContainer.style.unityBackgroundImageTintColor = slotContainerColorDefault;
    }

    public void PointerEnterInventorySlot(PointerEnterEvent evt)
    {
        InventoryManager.Instance.UpdateCurrentHoverSlot(this, true);
        parentContainer.currentHoverSlot = this;
    }

    public void PointerLeaveInventorySlot(PointerLeaveEvent evt)
    {
        InventoryManager.Instance.UpdateCurrentHoverSlot(this, false);
        parentContainer.currentHoverSlot = null;

    }

}
