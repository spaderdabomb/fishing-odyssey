// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace Dialogue
{
    partial class DialogueNPC
    {
        private VisualElement dialogueContainer;
        private VisualElement dialogueLabelContainer;
        private Label dialogueHeaderLabel;
        private Label dialogueCurrentText;
    
        protected void AssignQueryResults(VisualElement root)
        {
            dialogueContainer = root.Q<VisualElement>("DialogueContainer");
            dialogueLabelContainer = root.Q<VisualElement>("DialogueLabelContainer");
            dialogueHeaderLabel = root.Q<Label>("DialogueHeaderLabel");
            dialogueCurrentText = root.Q<Label>("DialogueCurrentText");
        }
    }
}
