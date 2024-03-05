using InventorySystem;
using UnityEngine;

namespace YuzuValen {
    public class GameSaveManager : MonoBehaviour
    {
        public SaveData saveData = new();
        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            saveData.inventorySlots = GetComponent<Inventory>().GetItems();
            SaveLoad.Save("save", saveData);
        }
        [ContextMenu("Load Game")]
        public void LoadGame()
        {
            saveData = SaveLoad.Load("save");
        }
    }
}