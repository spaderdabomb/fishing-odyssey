using UnityEngine.UIElements;
using UnityEngine;
using ZenUI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public partial class GearSlot : InventorySlot
{
    public GearContainer gearContainer;
    public GearSlotData gearSlotData;

    private Color labelColorDefault;
    private Color iconTintColorDefault;
    private Color slotContainerColorDefault;

    public GearSlot(VisualElement newRoot, int slotIndex, GearContainer gearContainer, GearSlotData gearSlotData) : base(newRoot, slotIndex, gearContainer)
    {
        this.gearContainer = gearContainer;
        this.gearSlotData = gearSlotData;
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
