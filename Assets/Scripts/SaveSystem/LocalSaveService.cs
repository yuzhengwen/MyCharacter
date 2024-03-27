using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace YuzuValen
{
    public class LocalSaveService : IDataPersistService
    {
        public readonly string saveDir = Application.persistentDataPath + "/saves/";
        public readonly string fileExtension = ".json";

        private readonly ISerializer serializer;

        public LocalSaveService(ISerializer serializer)
        {
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);
            this.serializer = serializer;
        }

        /// <summary>
        /// Returns true if the file saved successfully
        /// </summary>
        public bool Save(string fileName, SaveData data, bool overwrite = false)
        {
            string filePath = GetFilePath(fileName);
            if (!overwrite)
            {
                if (File.Exists(filePath))
                {
                    GetModifiedFilePath(ref fileName, ref filePath);
                }
            }

            string json = serializer.Serialize(data);
            File.WriteAllText(filePath, json);

            GUIUtility.systemCopyBuffer = saveDir; // Copy save path to clipboard
            return true;
        }

        /// <summary>
        /// Returns the SaveData object from the file <br/>
        /// </summary>
        /// <returns>SaveData object, null if unable to find save with filename specified</returns>
        public SaveData Load(string fileName)
        {
            string filePath = GetFilePath(fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return serializer.Deserialize(json);
            }
            Debug.LogError("Save file not found");
            return null;
        }
        public void Delete(string fileName)
        {
            string path = saveDir + fileName + ".json";
            if (File.Exists(path))
                File.Delete(path);
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(saveDir))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(saveDir))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
        #region Path Helper Methods
        private string GetFilePath(string fileName)
        {
            return saveDir + fileName + fileExtension;
        }

        private void GetModifiedFilePath(ref string fileName, ref string filePath)
        {
            while (File.Exists(filePath))
            {
                if (fileName.EndsWith(")"))
                {
                    int openBracketIndex = fileName.LastIndexOf('(');
                    int closeBracketIndex = fileName.LastIndexOf(')');
                    string number = fileName.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
                    fileName = fileName.Remove(openBracketIndex, closeBracketIndex - openBracketIndex + 1);
                    fileName = fileName + "(" + (int.Parse(number) + 1).ToString() + ")";
                }
                else
                {
                    fileName += "(1)";
                }
                filePath = GetFilePath(fileName);
            }
        }
        #endregion
    }

    public interface IDataPersistService
    {
        bool Save(string fileName, SaveData data, bool overwrite = false);
        SaveData Load(string fileName);
        void Delete(string fileName);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}