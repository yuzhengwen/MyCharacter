using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace YuzuValen.DialogueSystem
{
    public class Dialogue_UIController : MonoBehaviour, IDialogueUIController
    {
        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private SpeakerProfileDisplay[] speakerProfileDisplays;

        [Header("Choice UI")]
        [SerializeField] private GameObject choicePanel;
        private Button[] choiceButtons;
        private TextMeshProUGUI[] choiceTexts;

        [Header("Typing Parameters")]
        [SerializeField] private float typingSpeed = 0.05f;
        private Coroutine TypingCoVar;
        public bool IsTyping => TypingCoVar != null;

        private void Awake()
        {
            choiceButtons = choicePanel.GetComponentsInChildren<Button>();
            choiceTexts = new TextMeshProUGUI[choiceButtons.Length];
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int index = i; // to avoid referencing the same variable in the lambda
                choiceButtons[i].onClick.AddListener(() => OnClickChoice(index));
                choiceTexts[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }
        private void Start()
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(false);
        }
        public void ShowPanel(bool enable)
        {
            dialoguePanel.SetActive(enable);
        }
        public void SetText(string dialogue)
        {
            dialogueText.text = dialogue;
            TypingCoVar = StartCoroutine(TypingCo(dialogue));
        }

        private IEnumerator TypingCo(string line)
        {
            dialogueText.maxVisibleCharacters = 0;
            int totalVisibleCharacters = 0;
            int charIndex = 0;

            while (charIndex < line.Length)
            {
                // If the character is a tag, skip to the end of the tag (without pause)
                // TMP visible characters are not the same as string length when tags are present
                if (line[charIndex] == '<') 
                    while (line[charIndex] != '>')
                    {
                        charIndex++;
                        if (charIndex >= line.Length)
                            break;
                    }
                else
                    totalVisibleCharacters++;

                charIndex++;
                dialogueText.maxVisibleCharacters = totalVisibleCharacters;
                yield return new WaitForSeconds(typingSpeed);
            }

            TypingCoVar = null;
        }
        public void SkipTyping()
        {
            if (IsTyping)
            {
                StopCoroutine(TypingCoVar);
                TypingCoVar = null;
            }
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        }
        public virtual void SetCurrentSpeaker(SpeakerProfile speakerProfile){
            speakerProfileDisplays[0].SetSpeaker(speakerProfile);
        }
        public virtual void SetSpeaker(int index, SpeakerProfile speakerProfile)
        {
            if (index >= speakerProfileDisplays.Length)
            {
                Debug.LogError($"Trying to set speaker at index {index}, but UI only supports {speakerProfileDisplays.Length} speakers");
                return;
            }
            speakerProfileDisplays[index].SetSpeaker(speakerProfile);
        }
        private Action<int> makeChoice;
        public void SetChoices(string[] choices, Action<int> makeChoice)
        {
            if (choices.Length >= choiceTexts.Length)
            {
                Debug.LogError($"Trying to set {choices.Length} choice, but UI only supports {choiceTexts.Length} choices");
                return;
            }
            this.makeChoice = makeChoice;

            // if typing, wait for it to finish before displaying choices
            if (IsTyping) StartCoroutine(WaitForTypingToFinish(choices));
            else DisplayChoices(choices);
        }

        private IEnumerator WaitForTypingToFinish(string[] choices)
        {
            while (IsTyping) yield return null;
            DisplayChoices(choices);
        }

        private void DisplayChoices(string[] choices)
        {
            choicePanel.SetActive(true);
            // hide all choice buttons
            Array.ForEach(choiceButtons, x => x.gameObject.SetActive(false));
            // only set and show the choices that are available
            for (int i = 0; i < choices.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];
            }
        }
        private void OnClickChoice(int index)
        {
            makeChoice?.Invoke(index);
            choicePanel.SetActive(false);
        }
    }
    [System.Serializable]
    public class SpeakerProfileDisplay
    {
        public TMPro.TextMeshProUGUI speakerText;
        public UnityEngine.UI.Image speakerPortrait;
        public void SetSpeaker(SpeakerProfile speakerProfile)
        {
            speakerPortrait.sprite = speakerProfile.portrait;
            speakerText.text = speakerProfile.name;
        }
    }
}