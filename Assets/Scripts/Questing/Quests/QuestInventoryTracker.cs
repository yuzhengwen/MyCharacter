using InventorySystem;
using System;
using UnityEngine;

public class QuestInventoryTracker : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private QuestJournal journal;
    void Start()
    {
        inventory.OnItemAdded += SendQuestMessage;
    }

    private void SendQuestMessage(ItemDataSO data, int no)
    {
        Debug.Log("Trigger Event: " + data.displayName + " Collected");
        journal.TriggerEvent(data.displayName + "Collected", new IntEventArgs(no));
        /* same as above
        if (data.displayName == "Coin")
        {
            journal.TriggerEvent("CoinCollected", new IntEventArgs(no));
        }
        if (data.displayName == "Crystal")
        {
            journal.TriggerEvent("CrystalCollected", new IntEventArgs(no));
        }
        */
    }
    public class IntEventArgs : EventArgs
    {
        public int value;
        public IntEventArgs(int value)
        {
            this.value = value;
        }
    }
}
