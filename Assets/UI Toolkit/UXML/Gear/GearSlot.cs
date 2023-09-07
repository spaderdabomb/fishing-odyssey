using UnityEngine.UIElements;
using UnityEngine;
using ZenUI;
using ItemType = ItemData.ItemType;

public partial class GearSlot : InventorySlot
{
    public GearContainer gearContainer;
    public GearSlotData gearSlotData;
    public ItemType itemType;

    public GearSlot(VisualElement newRoot, int slotIndex, GearContainer gearContainer, GearSlotData gearSlotData, ItemType itemType) : base(newRoot, slotIndex, gearContainer)
    {
        this.gearContainer = gearContainer;
        this.gearSlotData = gearSlotData;
        this.itemType = itemType;
        AssignQueryResults(root);
        InitGearSlot();
    }

    private void InitGearSlot()
    {
        if (gearSlotData != null)
        {
            slotNameLabel.text = gearSlotData.displayName;
            slotNameLabel.style.display = DisplayStyle.Flex;
            backingIcon.style.backgroundImage = gearSlotData.backingIcon;
        }
        else
        {
            slotNameLabel.text = string.Empty;
            slotNameLabel.style.display = DisplayStyle.None;
        }
    }
}
