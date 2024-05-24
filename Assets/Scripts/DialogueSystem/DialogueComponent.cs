using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.DialogueSystem
{
    public class DialogueComponent : MonoBehaviour
    {
        [SerializeField] private TextAsset inkJson;
        [SerializeField] private SpeakerProfile[] speakerProfiles;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !DialogueManager.Instance.IsDialoguePlaying)
            {
                TriggerDialogue();
            }
        }
        public void TriggerDialogue()
        {
            DialogueManager.Instance.BeginDialogue(inkJson, speakerProfiles);
        }
    }
}
