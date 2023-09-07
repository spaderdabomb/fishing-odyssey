using UnityEngine.UIElements;
using UnityEngine;

namespace Dialogue
{
    public partial class DialogueNPC
    {
        public VisualElement root;
        public DialogueNPC(VisualElement root)
        {
            this.root = root;
            AssignQueryResults(root);
        }

        public void SetCurrentStoryText(string currentText)
        {
            // dialogueCurrentText.text = currentText;
        }

        public void SetVisible(bool isVisible)
        {

            root.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
