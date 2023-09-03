// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class Inventory
{
    private VisualElement inventoryBg;
    private VisualElement sortingContainer;
    private VisualElement sortByRodsContainer;
    private VisualElement sortByEquipmentContainer;
    private VisualElement inventoryContainer;
    private VisualElement pageContainer;
    private VisualElement inventoryPageLeftArrow;
    private Label inventoryPageLabel;
    private VisualElement inventoryPageRightArrow;
    private VisualElement ghostIcon;
    private Label ghostIconLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        inventoryBg = root.Q<VisualElement>("InventoryBg");
        sortingContainer = root.Q<VisualElement>("SortingContainer");
        sortByRodsContainer = root.Q<VisualElement>("SortByRodsContainer");
        sortByEquipmentContainer = root.Q<VisualElement>("SortByEquipmentContainer");
        inventoryContainer = root.Q<VisualElement>("InventoryContainer");
        pageContainer = root.Q<VisualElement>("PageContainer");
        inventoryPageLeftArrow = root.Q<VisualElement>("InventoryPageLeftArrow");
        inventoryPageLabel = root.Q<Label>("InventoryPageLabel");
        inventoryPageRightArrow = root.Q<VisualElement>("InventoryPageRightArrow");
        ghostIcon = root.Q<VisualElement>("ghostIcon");
        ghostIconLabel = root.Q<Label>("ghostIconLabel");
    }
}
