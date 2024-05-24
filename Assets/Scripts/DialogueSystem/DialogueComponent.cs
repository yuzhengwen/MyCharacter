using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YuzuValen.DialogueSystem
{
    /// <summary>
    /// Attach this component to a GameObject to allow triggering dialogue with this game object<br/>
    /// </summary>
    public class DialogueComponent : MonoBehaviour, IDialogueFunctionBinder
    {
        [SerializeField] private TextAsset inkJson;
        private Story story;
        [SerializeField] private SpeakerProfile[] speakerProfiles;

        [SerializeField] private InputAction triggerDialogue;
        private void OnEnable()
        {
            if (triggerDialogue != null)
            {
                triggerDialogue.Enable();
                triggerDialogue.performed += OnTriggerDialogueInput;
            }
        }
        private void Awake()
        {
            story = new Story(inkJson.text);
        }
        public void OnTriggerDialogueInput(InputAction.CallbackContext ctx) => OnTriggerDialogueInput();
        /// <summary>
        /// Use this method to trigger dialogue from an external source<br/>
        /// Calls TriggerDialogue internally only if dialogue is not already playing
        /// </summary>
        public void OnTriggerDialogueInput()
        {
            if (!DialogueManager.Instance.IsDialoguePlaying)
                TriggerDialogue();
        }
        private void TriggerDialogue()
        {
            // reset the story state, otherwise it will only play once
            // DialogueManager will handle persisting states
            story.ResetState();
            DialogueManager.Instance.BeginDialogue(story, speakerProfiles, this);
        }
        public virtual void BindFunctions(Story story)
        {
            story.BindExternalFunction<string>("TestDebug", (string s) => Debug.Log(s));
        }
        public virtual void UnbindFunctions(Story story)
        {
            story.UnbindExternalFunction("TestDebug");
        }
    }
}
