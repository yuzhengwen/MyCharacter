using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.InputSystem;
using YuzuValen.Utils;

namespace YuzuValen.DialogueSystem
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        public Story CurrentStory { get; protected set; }
        public bool IsDialoguePlaying { get; protected set; } = false;

        public event Action<Story> OnDialogueBegin;
        public event Action<Story> OnDialogueExit;

        [SerializeField] public Dialogue_UIController uiController;
        /// <summary>
        /// object that stores all story variables, allows for setting and getting variables from the story
        /// </summary>
        public readonly DialogueVariables dialogueVariables = new();

        private const string SPEAKER_TAG = "speaker";
        private readonly List<SpeakerProfile> speakerProfiles = new();

        [SerializeField] private SpeakerProfile[] persistentProfiles;

        [SerializeField] private InputAction continueAction;

        private IDialogueFunctionBinder binder;

        private void OnEnable()
        {
            if (continueAction != null)
            {
                continueAction.Enable();
                continueAction.performed += _ => OnContinueInput();
            }
        }
        private void Start()
        {
            IsDialoguePlaying = false;
        }
        /// <summary>
        /// Player input should trigger this method<br/>
        /// Will finish typing if the UI is still typing<br/>
        /// If there are choices, will not continue until a choice is made<br/>
        /// Otherwise, will continue the story
        /// </summary>
        public void OnContinueInput()
        {
            if (IsDialoguePlaying)
            {
                // if the UI is still typing, instantly finish typing
                if (uiController.IsTyping) { SkipTyping(); return; }
                // if there are choices, don't continue until a choice is made
                if (CurrentStory.currentChoices.Count > 0) return;
                // otherwise, proceed with the story
                ContinueStory();
            }
        }
        /// <summary>
        /// Begins a dialogue with the given story (and profiles and external functions)<br/>
        /// </summary>
        /// <param name="story">Ink story object</param>
        /// <param name="speakerProfiles">Adds speaker profiles available to use in ink story</param>
        /// <param name="binder">For binding external functions to ink story</param>
        public void BeginDialogue(Story story, SpeakerProfile[] speakerProfiles = null, IDialogueFunctionBinder binder = null)
        {
            CurrentStory = story;
            OnDialogueBegin?.Invoke(CurrentStory);
            IsDialoguePlaying = true;

            // load all the speakers in this dialogue
            if (speakerProfiles != null)
            {
                AddSpeakerProfiles(speakerProfiles);
            }

            // bind external functions to the story
            this.binder = binder;
            binder?.BindFunctions(CurrentStory);

            // update all variables in the story (important to do this before displaying the first line)
            dialogueVariables.UpdateStoryVariables(CurrentStory);
            // start listening for variable changes
            dialogueVariables.StartListening(CurrentStory);

            uiController.ShowMainPanel(true);
            ContinueStory();
        }
        public void AddSpeakerProfiles(SpeakerProfile[] speakerProfiles)
        {
            this.speakerProfiles.Clear();
            this.speakerProfiles.AddRange(persistentProfiles);
            this.speakerProfiles.AddRange(speakerProfiles);
        }

        /// <summary>
        /// Exits the dialogue, hiding the UI and invoking the OnDialogueExit event <br/>
        /// This is called by default when the story ends
        /// </summary>
        public void ExitDialogue()
        {
            binder?.UnbindFunctions(CurrentStory);
            IsDialoguePlaying = false;
            uiController.ShowMainPanel(false);
            OnDialogueExit?.Invoke(CurrentStory);

            // stop listening for variable changes
            dialogueVariables.StopListening(CurrentStory);
        }
        private void ContinueStory()
        {
            if (CurrentStory.canContinue)
            {
                uiController.SetText(CurrentStory.Continue()); // dequeues and returns the next line of text
                CheckChoices();
                HandleTags(CurrentStory.currentTags);
            }
            else
            {
                ExitDialogue();
            }
        }

        protected virtual void HandleTags(List<string> currentTags)
        {
            foreach (string tag in currentTags)
            {
                // handle speaker tag
                if (tag.StartsWith(SPEAKER_TAG))
                {
                    string speaker = tag.Split(':')[1];
                    // find speaker by matching tag value with speakerId, allows for multiple speaker profiles with same name/portrait (e.g. same character with different expressions)
                    SpeakerProfile speakerProfile = speakerProfiles.Find(sp => sp.speakerId == speaker);
                    if (speakerProfile.name == null)
                    {
                        Debug.LogError($"Speaker {speaker} not found in speaker profiles");
                        return;
                    }
                    uiController.SetCurrentSpeaker(speakerProfile);
                }
            }
        }

        private void CheckChoices()
        {
            if (CurrentStory.currentChoices.Count <= 0) return;

            string[] choices = new string[CurrentStory.currentChoices.Count];
            for (int i = 0; i < CurrentStory.currentChoices.Count; i++)
            {
                choices[i] = CurrentStory.currentChoices[i].text;
            }
            uiController.SetChoices(choices, MakeChoice);

        }
        private void MakeChoice(int index)
        {
            CurrentStory.ChooseChoiceIndex(index);
            ContinueStory();
        }
        private void SkipTyping() { if (uiController.IsTyping) uiController.SkipTyping(); }
    }
    public interface IDialogueUIController
    {
        void ShowMainPanel(bool enable);
        void SetText(string dialogue);
        void SetCurrentSpeaker(SpeakerProfile speakerProfile);
        void SetChoices(string[] choices, Action<int> makeChoice);
        void SkipTyping();
    }
    [Serializable]
    public struct SpeakerProfile
    {
        [Tooltip("Should match the speaker tag value in Ink")]
        public string speakerId;
        public string name;
        public Sprite portrait;
    }
}
