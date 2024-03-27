using UnityEngine;

namespace YuzuValen
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(SaveData data)
        {
            return JsonUtility.ToJson(data, true);
        }
        public SaveData Deserialize(string json)
        {
            return JsonUtility.FromJson<SaveData>(json);
        }
    }
    public interface ISerializer
    {
        string Serialize(SaveData data);
        SaveData Deserialize(string json);
    }
}