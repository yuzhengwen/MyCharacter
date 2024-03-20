using InventorySystem;
using System;
using UnityEngine;

public class QuestCollector : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private QuestJournal journal;
    void Start()
    {
        inventory.OnItemAdded += SendQuestMessage;
    }

    private void SendQuestMessage(ItemDataSO data, int no)
    {
        Debug.Log("Item Collected: " + data.displayName);
        if (data.displayName == "Coin")
        {
            journal.TriggerEvent("CoinCollected");
        }
        if (data.displayName == "Crystal")
        {
            journal.TriggerEvent("CrystalCollected");
        }
    }
}
