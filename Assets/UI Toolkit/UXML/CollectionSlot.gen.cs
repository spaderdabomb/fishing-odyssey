// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class CollectionSlot
{
    private VisualElement collectionSlot;
    private VisualElement fishIcon;
    private Label fishLabel;
    private Label rarityLabel;
    private Label collectedLabel;
    private VisualElement collectedIcon;
    
    protected void AssignQueryResults(VisualElement root)
    {
        collectionSlot = root.Q<VisualElement>("CollectionSlot");
        fishIcon = root.Q<VisualElement>("fish-icon");
        fishLabel = root.Q<Label>("fish-label");
        rarityLabel = root.Q<Label>("rarity-label");
        collectedLabel = root.Q<Label>("collected-label");
        collectedIcon = root.Q<VisualElement>("collected-icon");
    }
}
