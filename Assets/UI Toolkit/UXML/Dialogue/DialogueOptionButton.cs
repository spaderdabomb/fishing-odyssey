using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using Ink.Runtime;

namespace Dialogue
{
    public partial class DialogueOptionButton
    {
        public DialogueOptionButton(VisualElement root)
        {
            AssignQueryResults(root);
        }

        public void SetOptionsText(string newText)
        {
            optionsButton.text = newText;
        }

        public void AddButtonClickedCallback(Choice choice)
        {
            optionsButton.clickable.clicked += () => OptionsButtonClicked(choice);
        }

        public void RemoveButtonClickedCallback(Choice choice)
        {
            optionsButton.clickable.clicked -= () => OptionsButtonClicked(choice);
        }

        private void OptionsButtonClicked(Choice choice)
        {
            DialogueManager.Instance.ChooseChoice(choice.index);
            AudioManager.PlaySound(MainAudioLibrarySounds.digi_plink);
            Debug.Log("Playing sounds");
        }
    }
}
