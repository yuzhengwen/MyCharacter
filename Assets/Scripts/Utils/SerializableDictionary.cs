using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.Utils
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
            {
                throw new System.Exception("The number of keys and values does not match!");
            }

            for (int i = 0; i < keys.Count; i++)
            {
                this[keys[i]] = values[i];
            }
        }
    }
}
