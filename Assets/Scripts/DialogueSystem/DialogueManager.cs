using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;

namespace YuzuValen
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
        private Story story;
        private bool isPlaying = false;

        public event System.Action OnDialogueBegin;
        public event System.Action OnDialogueExit;
        private void Start()
        {
            panel.SetActive(false);
            isPlaying = false;
        }
        public void BeginDialogue(TextAsset inkJson)
        {
            OnDialogueBegin?.Invoke();
            story = new Story(inkJson.text);
            isPlaying = true;
            panel.SetActive(true);

            ContinueStory();
        }

        private void ExitDialogue()
        {
            isPlaying = false;
            panel.SetActive(false);
            dialogueText.text = "";
            OnDialogueExit?.Invoke();
        }
        private void Update()
        {
            if (isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ContinueStory();
                }
            }
        }
        private void ContinueStory()
        {
            if (story.canContinue)
            {
                dialogueText.text = story.Continue(); // dequeues and returns the next line of text
            }
            else
            {
                ExitDialogue();
            }
        }
    }
}
