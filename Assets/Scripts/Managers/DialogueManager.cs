using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;

        [Header("Dialogue UI")]
        public UIDocument mainDialogueDocument;
        private VisualElement root;
        public DialogueNPC dialogueNPC;

        private Story currentStory = null;
        private bool dialogueIsPlaying = false;

        [Header("Choices UI")]
        [SerializeField] private VisualTreeAsset dialogOptionsButtonAsset;
        private List<Choice> currentChoices = new();
        private List<DialogueOptionButton> dialogueOptionButtons = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one DialogueManager");
            }
            Instance = this;
        }

        private void Start()
        {
            root = mainDialogueDocument.rootVisualElement;
            dialogueNPC = new DialogueNPC(root);
            dialogueNPC.SetVisible(false);
        }

        private void Update()
        {
            if (!dialogueIsPlaying)
                return;
        }

        public void EnterDialogueMode(TextAsset inkJSON)
        {
            currentStory = new Story(inkJSON.text);
            dialogueIsPlaying = true;
            dialogueNPC.SetVisible(true);

            ContinueStory();
        }

        private void ExitDialogueMode()
        {
            dialogueIsPlaying = false;
            dialogueNPC.SetVisible(false);
        }

        public void ContinueStory()
        {
            if (currentStory == null)
                return;

            print(currentStory.canContinue);
            print(dialogueOptionButtons.Count <= 0);
            if (currentStory.canContinue && dialogueOptionButtons.Count <= 0)
            {
                dialogueNPC.SetCurrentStoryText(currentStory.Continue());
                DisplayChoices();
            }
            else if (dialogueOptionButtons.Count > 0)
            {
                print("dialogue options showing");

            }
            else
            {
                ExitDialogueMode();
                UIGameManager.Instance.SetPlayerInMenuOptions(MenuType.GameScene);
            }
        }

        public void DisplayChoices()
        {
            currentChoices = currentStory.currentChoices;

            for (int i = 0; i < currentChoices.Count; i++)
            {
                Choice choice = currentChoices[i];
                VisualElement newDialogOptionsElement = dialogOptionsButtonAsset.CloneTree();
                DialogueOptionButton dialogOptionButton = new DialogueOptionButton(newDialogOptionsElement);
                dialogOptionButton.SetOptionsText(choice.text);
                dialogOptionButton.AddButtonClickedCallback(choice);
                dialogueOptionButtons.Add(dialogOptionButton);
                dialogueNPC.AddToOptionsGroupBox(newDialogOptionsElement);
            }
        }

        public void RemoveChoices()
        {
            for (int i = 0; i < dialogueOptionButtons.Count; i++)
            {
                DialogueOptionButton dialogueOptionButton = dialogueOptionButtons[i];
                Choice choice = currentChoices[i];
                dialogueOptionButton.RemoveButtonClickedCallback(choice);
            }

            dialogueOptionButtons.Clear();
            currentChoices.Clear();
            dialogueNPC.ClearOptionsGroupBox();
        }

        public void ChooseChoice(int index)
        {
            currentStory.ChooseChoiceIndex(index);
            RemoveChoices();
            ContinueStory();
        }
    }
}
