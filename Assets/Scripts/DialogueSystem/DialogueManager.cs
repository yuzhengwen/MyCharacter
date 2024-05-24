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
        public bool IsDialoguePlaying { get; protected set; } = false;

        public event Action OnDialogueBegin;
        public event Action OnDialogueExit;

        [SerializeField] public Dialogue_UIController uiController;
        /// <summary>
        /// object that stores all story variables, allows for setting and getting variables from the story
        /// </summary>
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
        /// Begins a dialogue with the given inkJson file and speaker profiles<br/>
        /// </summary>
        /// <param name="inkJson"></param>
        /// <param name="speakerProfiles"></param>
        public void BeginDialogue(TextAsset inkJson, SpeakerProfile[] speakerProfiles = null)
        {
            // load all the speakers in this dialogue
            if (speakerProfiles != null)
            {
                AddSpeakerProfiles(speakerProfiles);
            }

            OnDialogueBegin?.Invoke();
            CurrentStory = new Story(inkJson.text);
            IsDialoguePlaying = true;

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
            IsDialoguePlaying = false;
            uiController.ShowMainPanel(false);
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
    public class DialogueVariables
    {
        /// <summary>
        /// If a variable is changed in the story, it will be updated here <br/>
        /// We can also set variables here, which will be reflected in the story <br/>
        /// Updating variables while story is running may cause unexpected behavior
        /// </summary>
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
