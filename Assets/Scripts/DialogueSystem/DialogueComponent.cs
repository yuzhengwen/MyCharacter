using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.DialogueSystem
{
    public class DialogueComponent : MonoBehaviour
    {
        [SerializeField] private TextAsset inkJson;
        [SerializeField] private SpeakerProfile speakerProfile;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !DialogueManager.Instance.IsPlaying)
            {
                TriggerDialogue();
            }
        }
        public void TriggerDialogue()
        {
            DialogueManager.Instance.BeginDialogue(inkJson, speakerProfile);
        }
    }
}
