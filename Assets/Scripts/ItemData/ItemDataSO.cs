using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemDataSO : ScriptableObject
{
    public string displayName;
    public Sprite sprite;
    public int id;
    public int maxStackSize;
}
