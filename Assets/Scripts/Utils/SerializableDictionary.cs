using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.Utils
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Header("When ticked, List will not update from game")]
        [SerializeField] private bool modify = false;
        [Header("Dictionary Data")]
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            if (!modify)
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<TKey, TValue> pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                this[keys[i]] = values[i];
            }
        }
    }
}
