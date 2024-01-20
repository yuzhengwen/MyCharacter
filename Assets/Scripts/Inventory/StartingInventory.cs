using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StartingInventory", menuName = "ScriptableObjects/StartingInventory", order = 1)]
public class StartingInventory : ScriptableObject
{
    public List<ItemData> items = new();
    public List<int> amounts = new();
}
