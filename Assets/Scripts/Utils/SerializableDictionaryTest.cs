using System.Linq;
using UnityEngine;

namespace YuzuValen.Utils
{
    public class SerializableDictionaryTest : MonoBehaviour
    {
        public SerializableDictionary<string, string> dict = new();

        [ContextMenu("Test")]
        public void Test()
        {
            var keys = dict.Keys;
            string keyString = "";
            foreach (var key in keys)
            {
                keyString+=key.ToString();
            }
            var values = dict.Values;
            string valueString = "";
            foreach (var val in values)
            {
                valueString+=val.ToString();
            }
            Debug.Log($"{keys.Count} Keys: {keyString}, {values.Count} Values: {valueString}");
        }
        private void Start()
        {
            dict.Remove( "test" );
        }
    }
}
