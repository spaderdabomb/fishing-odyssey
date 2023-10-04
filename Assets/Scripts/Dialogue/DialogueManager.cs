using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using JSAM;
using System;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;

        [Header("Dialogue UI")]
        public UIDocument mainDialogueDocument;
        private VisualElement root;
        [SerializeField] private TextAsset globalsInk;
        [SerializeField] private TextAsset loadGlobalsJSON;
        [HideInInspector] public Story globalsStory;
        public DialogueNPC dialogueNPC;
        private DialogueVariables dialogueVariables;
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

            dialogueVariables = new DialogueVariables(loadGlobalsJSON);
            globalsStory = new Story(loadGlobalsJSON.text);
        }

        private void OnEnable()
        {
            dialogueVariables.StartListening(globalsStory);
        }

        private void OnDisable()
        {
            dialogueVariables.StopListening(globalsStory);
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
         
        public void EnterDialogueMode(Story inkStory)
        {
            currentStory = inkStory;
            currentStory.state.GoToStart();
            dialogueIsPlaying = true;
            dialogueNPC.SetVisible(true);
            dialogueVariables.StartListening(currentStory);
            print(currentStory.variablesState["GettingStartedState"]);

            try
            {
                currentStory.BindExternalFunction("NPCDialogueFinish", (string npcID) => GameEventsManager.Instance.miscEvents.NPCDialogueFinish(npcID));
                currentStory.BindExternalFunction("StartQuest", GameEventsManager.Instance.inputEvents.SubmitPressed);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }

            ContinueStory();
        }

        public void ExitDialogueMode()
        {
            dialogueIsPlaying = false;
            dialogueNPC.SetVisible(false);
            currentStory.ChoosePathString((string)currentStory.variablesState["currentKnot"]);
            // GameEventsManager.Instance.miscEvents.NPCDialogueFinish("WiseOldMan");
            dialogueVariables.StopListening(currentStory);

        }

        public void ContinueStory()
        {
            if (currentStory == null)
                return;

            if (currentStory.canContinue && dialogueOptionButtons.Count <= 0)
            {
                dialogueNPC.SetCurrentStoryText(currentStory.Continue());
                DisplayChoices();
                AudioManager.PlaySound(MainAudioLibrarySounds.DialogueNext);
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
