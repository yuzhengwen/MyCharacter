using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen
{
    public class SaveData
    {
        public List<InventorySlot> inventorySlots = new();
        public PlayerData playerData;
    }
}
