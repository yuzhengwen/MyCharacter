using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.InputSystem;
using System.Linq;

namespace YuzuValen.DialogueSystem
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        public Story CurrentStory { get; protected set; }
        public bool IsPlaying { get; protected set; } = false;

        public event Action OnDialogueBegin;
        public event Action OnDialogueExit;

        [SerializeField] public Dialogue_UIController uiController;
        public readonly DialogueVariables dialogueVariables = new();

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
                if (CurrentStory.currentChoices.Count > 0) return;
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
            CurrentStory = new Story(inkJson.text);
            IsPlaying = true;

            // start listening for variable changes
            dialogueVariables.StartListening(CurrentStory);
            // update all variables in the story
            dialogueVariables.UpdateStoryVariables(CurrentStory);

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
    public class DialogueVariables
    {
        public readonly Dictionary<string, Ink.Runtime.Object> variables = new();
        public void StartListening(Story story)
        {
            story.variablesState.variableChangedEvent += ReadVariable;
        }
        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= ReadVariable;
        }
        private void ReadVariable(string varName, Ink.Runtime.Object value)
        {
            if (variables.ContainsKey(varName) && variables[varName].GetType() != value.GetType())
            {
                Debug.LogError($"Variable {varName} already exists with type {variables[varName].GetType()}, but tried to set with type {value.GetType()}");
                return;
            }
            variables[varName] = value;
        }
        public void UpdateStoryVariables(Story story)
        {
            List<string> keys = story.variablesState.ToList();
            foreach (var key in keys)
            {
                if (variables.ContainsKey(key))
                {
                    var storedValue = variables[key];
                    story.variablesState.SetGlobal(key, storedValue);
                }
            }
        }
    }
}
