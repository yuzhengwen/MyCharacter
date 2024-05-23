using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.InputSystem;

namespace YuzuValen.DialogueSystem
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        private Story story;
        public bool IsPlaying { get; private set; } = false;

        public event Action OnDialogueBegin;
        public event Action OnDialogueExit;

        [SerializeField] public Dialogue_UIController uiController;

        private const string SPEAKER_TAG = "speaker";
        private readonly List<SpeakerProfile> speakerProfiles = new();

        [SerializeField] private SpeakerProfile[] persistentProfiles;

        [SerializeField] private InputAction continueAction;

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
            IsPlaying = false;
        }
        /// <summary>
        /// Player input should trigger this method<br/>
        /// Will finish typing if the UI is still typing<br/>
        /// If there are choices, will not continue until a choice is made</br>
        /// Otherwise, will continue the story
        /// </summary>
        public void OnContinueInput()
        {
            if (IsPlaying)
            {
                // if the UI is still typing, instantly finish typing
                if (uiController.IsTyping) { SkipTyping(); return; }
                // if there are choices, don't continue until a choice is made
                if (story.currentChoices.Count > 0) return;
                // otherwise, proceed with the story
                ContinueStory();
            }
        }
        /// <summary>
        /// Begins a dialogue with the given inkJson file and speaker profiles<br/>
        /// </summary>
        /// <param name="inkJson"></param>
        /// <param name="speakerProfiles"></param>
        public void BeginDialogue(TextAsset inkJson, params SpeakerProfile[] speakerProfiles)
        {
            // load all the speakers in this dialogue
            this.speakerProfiles.Clear();
            this.speakerProfiles.AddRange(persistentProfiles);
            this.speakerProfiles.AddRange(speakerProfiles);

            OnDialogueBegin?.Invoke();
            story = new Story(inkJson.text);
            IsPlaying = true;

            uiController.ShowPanel(true);
            ContinueStory();
        }

        /// <summary>
        /// Exits the dialogue, hiding the UI and invoking the OnDialogueExit event
        /// This is called by default when the story ends
        /// </summary>
        public void ExitDialogue()
        {
            IsPlaying = false;
            uiController.ShowPanel(false);
            OnDialogueExit?.Invoke();
        }
        private void ContinueStory()
        {
            if (story.canContinue)
            {
                uiController.SetText(story.Continue()); // dequeues and returns the next line of text
                CheckChoices();
                HandleTags(story.currentTags);
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
                    SpeakerProfile speakerProfile = speakerProfiles.Find(sp => sp.name == speaker);
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
            if (story.currentChoices.Count <= 0) return;

            string[] choices = new string[story.currentChoices.Count];
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                choices[i] = story.currentChoices[i].text;
            }
            uiController.SetChoices(choices, MakeChoice);

        }
        private void MakeChoice(int index)
        {
            story.ChooseChoiceIndex(index);
            ContinueStory();
        }
        private void SkipTyping() { if (uiController.IsTyping) uiController.SkipTyping(); }
    }
    public interface IDialogueUIController
    {
        void ShowPanel(bool enable);
        void SetText(string dialogue);
        void SetCurrentSpeaker(SpeakerProfile speakerProfile);
        void SetChoices(string[] choices, Action<int> makeChoice);
        void SkipTyping();
    }
    public interface IVisualNovelUIController : IDialogueUIController
    {
        void SetSpeaker(SpeakerProfile speaker1, SpeakerProfile speaker2);
        void SetSpeaker1(SpeakerProfile speaker1);
        void SetSpeaker2(SpeakerProfile speaker2);
    }
    [Serializable]
    public struct SpeakerProfile
    {
        [Tooltip("Should match the speaker tag value in Ink")]
        public string name;
        public Sprite portrait;
    }
}
