using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;

namespace YuzuValen.DialogueSystem
{
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        private Story story;
        public bool IsPlaying { get; private set; } = false;

        public event Action OnDialogueBegin;
        public event Action OnDialogueExit;

        [SerializeField] private Dialogue_UIController uiController;

        private const string SPEAKER_TAG = "speaker";
        private List<SpeakerProfile> speakerProfiles = new();

        [SerializeField] private SpeakerProfile playerProfile;

        private void Start()
        {
            IsPlaying = false;
            speakerProfiles.Add(playerProfile);
        }
        public void BeginDialogue(TextAsset inkJson, params SpeakerProfile[] speakerProfiles)
        {
            this.speakerProfiles.AddRange(speakerProfiles);
            OnDialogueBegin?.Invoke();
            story = new Story(inkJson.text);
            IsPlaying = true;

            uiController.ShowPanel(true);
            ContinueStory();
        }

        private void ExitDialogue()
        {
            IsPlaying = false;
            uiController.ShowPanel(false);
            OnDialogueExit?.Invoke();
        }
        private void Update()
        {
            if (IsPlaying && Input.GetKeyDown(KeyCode.F))
                ContinueStory();
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

        private void HandleTags(List<string> currentTags)
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
                    uiController.SetSpeaker(speakerProfile);
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
        public void MakeChoice(int index)
        {
            story.ChooseChoiceIndex(index);
            ContinueStory();
        }
    }
    public interface IDialogueUIController
    {
        void ShowPanel(bool enable);
        void SetText(string dialogue);
        void SetSpeaker(SpeakerProfile speakerProfile);
        void SetChoices(string[] choices, Action<int> makeChoice);
    }
    [Serializable]
    public struct SpeakerProfile
    {
        public string name;
        public Sprite portrait;
    }
}
