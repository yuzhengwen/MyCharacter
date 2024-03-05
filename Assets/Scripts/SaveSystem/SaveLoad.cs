using System;
using System.IO;
using UnityEngine;

namespace YuzuValen
{
    public static class SaveLoad
    {
        private readonly static string savePath = Application.persistentDataPath + "/saves/";

        public static event Action OnSave;
        public static event Action<SaveData> OnLoad;

        /// <summary>
        /// Returns true if the file saved successfully
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Save(string fileName, SaveData data)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            string path = savePath + fileName + ".json";
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            GUIUtility.systemCopyBuffer = savePath;
            OnSave?.Invoke();
            return true;
        }
        /// <summary>
        /// Returns the SaveData object from the file <br/>
        /// If the file does not exist, returns a new (empty) SaveData object
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SaveData Load(string fileName)
        {
            string path = savePath + fileName + ".json";
            SaveData data = new();

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Debug.LogError("Save file not found");
            }
            OnLoad?.Invoke(data);
            return data;
        }
        public static void Delete(string fileName)
        {
            string path = savePath + fileName + ".json";
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}