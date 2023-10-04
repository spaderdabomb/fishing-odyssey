// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class MissionContainer
{
    private QuickEye.UIToolkit.Tab tabRoot;
    private VisualElement missionIcon;
    private Label missionNameLabel;
    private VisualElement questStatusContainer;
    private VisualElement questStatusIcon;
    
    protected void AssignQueryResults(VisualElement root)
    {
        tabRoot = root.Q<QuickEye.UIToolkit.Tab>("TabRoot");
        missionIcon = root.Q<VisualElement>("MissionIcon");
        missionNameLabel = root.Q<Label>("MissionNameLabel");
        questStatusContainer = root.Q<VisualElement>("QuestStatusContainer");
        questStatusIcon = root.Q<VisualElement>("QuestStatusIcon");
    }
}
