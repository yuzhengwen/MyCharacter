using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuzuValen.Utils;

namespace YuzuValen
{
    public class GameSaveManager : MonoBehaviourSingletonPersistent<GameSaveManager>
    {
        private IDataPersistService saveService;
        private ISerializer serializer;

        [SerializeField] private string fileName = "save";
        [SerializeField] private bool overwrite = false;

        [ReadOnlyInspector][SerializeField] private ISaveable[] saveables;

        [ReadOnlyInspector][SerializeField] private SaveData saveData;

        private void Start()
        {
            serializer = new JsonSerializer();
            saveService = new LocalSaveService(serializer);
            saveables = GetSaveables();
        }

        [ContextMenu("New Game")]
        public void NewGame()
        {
            saveData = new();
        }
        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            foreach (var saveable in saveables)
                saveable.Save(saveData);
            saveService.Save(fileName, saveData, overwrite);
        }
        [ContextMenu("Load Game")]
        public void LoadGame()
        {
            saveData = saveService.Load(fileName);
            foreach (var saveable in saveables)
                saveable.Load(saveData);
        }
        [ContextMenu("Delete All Saves")]
        public void DeleteAllSaves() => saveService.DeleteAll();
        [ContextMenu("Print Saves")]
        public void PrintSaves() => Debug.Log(string.Join("\n", ListSaves));
        [ContextMenu("Copy Save Path")]
        public void CopySavePath() => GUIUtility.systemCopyBuffer = ((LocalSaveService)saveService).saveDir;

        public string[] ListSaves => saveService.ListSaves().ToArray();
        private ISaveable[] GetSaveables() => FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();
    }
}