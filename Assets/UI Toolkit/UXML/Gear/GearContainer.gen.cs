// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace ZenUI
{
    partial class GearContainer
    {
        private VisualElement gearContainerRoot;
        private VisualElement gearSubContainer;
        private VisualElement gearSubHeader;
        private Label gearSubHeaderLabel;
        private VisualElement gearSubOutline;
        private VisualElement gearSlotContainer;
    
        protected void AssignQueryResults(VisualElement root)
        {
            gearContainerRoot = root.Q<VisualElement>("GearContainerRoot");
            gearSubContainer = root.Q<VisualElement>("GearSubContainer");
            gearSubHeader = root.Q<VisualElement>("GearSubHeader");
            gearSubHeaderLabel = root.Q<Label>("GearSubHeaderLabel");
            gearSubOutline = root.Q<VisualElement>("GearSubOutline");
            gearSlotContainer = root.Q<VisualElement>("GearSlotContainer");
        }
    }
}
