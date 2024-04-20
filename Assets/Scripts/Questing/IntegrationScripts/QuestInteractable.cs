using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInteractable : MonoBehaviour
{
    [SerializeField] private QuestJournal questJournal;
    [SerializeField] private string interactableName;

    public void Interact(GameObject player)
    {
        var playerJournal = player.GetComponent<QuestJournal>();
        if (playerJournal != null)
            questJournal = playerJournal;

        if (questJournal == null)
        {
            Debug.LogError("QuestJournal not found");
            return;
        }

        questJournal.TriggerEvent(interactableName + "Interact");
    }
}
