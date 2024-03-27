using InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen
{
    public class InventorySaver : MonoBehaviour, ISaveable
    {
        [SerializeField] private Inventory inventory;
        public void Load(SaveData data)
        {
            inventory.LoadInventoryExact(data.inventoryData.inventorySlots);
        }

        public void Save(SaveData data)
        {
            List<InventorySlot> slots = inventory.GetItems();
            data.inventoryData.inventorySlots = slots;
        }
    }
    [Serializable]
    public class InventoryData
    {
        public List<InventorySlot> inventorySlots = new();
    }
}