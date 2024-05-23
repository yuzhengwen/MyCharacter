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
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private Image speakerPortrait;

        [Header("Choice UI")]
        [SerializeField] private GameObject choicePanel;
        private Button[] choiceButtons;
        private TextMeshProUGUI[] choiceTexts;

        private void Awake()
        {
            choiceButtons = choicePanel.GetComponentsInChildren<Button>();
            choiceTexts = new TextMeshProUGUI[choiceButtons.Length];
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int index = i; // to avoid referencing the same variable in the lambda
                choiceButtons[i].onClick.AddListener(() => OnClickChoice(index));
                Debug.Log($"Added listener to {i}");
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
        }
        public void SetSpeaker(SpeakerProfile speakerProfile) => SetSpeaker(speakerProfile.name, speakerProfile.portrait);
        public void SetSpeaker(string speaker, Sprite portrait)
        {
            speakerText.text = speaker;
            speakerPortrait.sprite = portrait;
        }
        private Action<int> makeChoice;
        public void SetChoices(string[] choices, Action<int> makeChoice)
        {
            if (choices.Length >= choiceTexts.Length)
            {
                Debug.LogError($"Trying to set {choices.Length} choice, but UI only supports {choiceTexts.Length} choices");
                return;
            }
            choicePanel.SetActive(true);
            this.makeChoice = makeChoice;
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
}