using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen
{
    public class SaveData
    {
        public InventoryData inventoryData = new();
        public PlayerData playerData = new();
    }
    public interface ISaveable
    {
        void Save(SaveData data);
        void Load(SaveData data);
    }
}
