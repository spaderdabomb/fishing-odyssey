// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class GearSlot
{
    private VisualElement slotContainer;
    private VisualElement slotIcon;
    private Label itemCountLabel;
    private VisualElement backingIcon;
    private Label slotNameLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        slotContainer = root.Q<VisualElement>("slotContainer");
        slotIcon = root.Q<VisualElement>("slotIcon");
        itemCountLabel = root.Q<Label>("itemCountLabel");
        backingIcon = root.Q<VisualElement>("backingIcon");
        slotNameLabel = root.Q<Label>("slotNameLabel");
    }
}
