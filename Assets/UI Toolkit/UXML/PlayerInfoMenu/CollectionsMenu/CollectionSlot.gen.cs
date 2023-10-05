// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class CollectionSlot
{
    private QuickEye.UIToolkit.Tab tabRoot;
    private VisualElement fishIcon;
    private VisualElement starContainer;
    private VisualElement starCommon;
    private VisualElement starUncommon;
    private VisualElement starRare;
    private VisualElement starEpic;
    private VisualElement starLegendary;
    private VisualElement starMythic;
    
    protected void AssignQueryResults(VisualElement root)
    {
        tabRoot = root.Q<QuickEye.UIToolkit.Tab>("tabRoot");
        fishIcon = root.Q<VisualElement>("FishIcon");
        starContainer = root.Q<VisualElement>("StarContainer");
        starCommon = root.Q<VisualElement>("StarCommon");
        starUncommon = root.Q<VisualElement>("StarUncommon");
        starRare = root.Q<VisualElement>("StarRare");
        starEpic = root.Q<VisualElement>("StarEpic");
        starLegendary = root.Q<VisualElement>("StarLegendary");
        starMythic = root.Q<VisualElement>("StarMythic");
    }
}
