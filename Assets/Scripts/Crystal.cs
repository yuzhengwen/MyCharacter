using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour, ICollectible
{

    public static event Action<ItemData> OnCrystalCollected;

    public ItemData itemData;

    public void Collect()
    {
        OnCrystalCollected?.Invoke(itemData);
        Destroy(gameObject);
    }
}
