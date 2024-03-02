using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "StartingInventory", menuName = "ScriptableObjects/StartingInventory", order = 1)]
    public class StartingInventory : ScriptableObject
    {
        public List<InventorySlot> startingItems;
    }
}
