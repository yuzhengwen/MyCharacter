using InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen
{
    public class PlayerSaver : MonoBehaviour, ISaveable
    {
        public void Load(SaveData data)
        {
            transform.position = data.playerData.position;
        }

        public void Save(SaveData data)
        {
            data.playerData.position = transform.position;
        }
    }
    [Serializable]
    public class PlayerData
    {
        public PlayerStats stats;
        public Vector3 position;
    }
}
