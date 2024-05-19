using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen
{
    public class DialogueComponent : MonoBehaviour
    {
        [SerializeField] private TextAsset inkJson;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.Instance.BeginDialogue(inkJson);
            }
        }
    }
}
