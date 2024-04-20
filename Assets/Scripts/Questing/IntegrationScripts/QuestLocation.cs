using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLocation : MonoBehaviour
{
    [SerializeField] private QuestJournal questJournal;
    [SerializeField] private string locationName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerJournal = collision.GetComponent<QuestJournal>();
            if (playerJournal != null)
                questJournal = playerJournal;

            if (questJournal == null)
            {
                Debug.LogError("QuestJournal not found");
                return;
            }

            questJournal.TriggerEvent(locationName + "Entered");
        }
    }
}
