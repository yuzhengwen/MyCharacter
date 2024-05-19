using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuzuValen.Utils;

namespace YuzuValen
{
    [System.Serializable]
    public class SaveData
    {
        public InventoryData inventoryData = new();
        public PlayerData playerData = new();
        public QuestData questData = new();
        public Dictionary<string, SceneSaveData> sceneDatas = new();
    }
    public interface ISaveable
    {
        void Save(SaveData data);
        void Load(SaveData data);
    }
    [System.Serializable]
    public class SceneSaveData
    {
        public Dictionary<string, object> saveData = new();
    }
}
