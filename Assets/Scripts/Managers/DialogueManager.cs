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
            if (currentStory.canContinue)
            {
                dialogueNPC.SetCurrentStoryText(currentStory.Continue());
            }
            else
            {
                ExitDialogueMode();
            }
        }
    }
}
