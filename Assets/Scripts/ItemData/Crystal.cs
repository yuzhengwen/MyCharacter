using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour, ICollectible
{

    public static event Action<ItemDataSO> OnCrystalCollected;

    public ItemDataSO itemData;

    public void Collect()
    {
        OnCrystalCollected?.Invoke(itemData);
        Destroy(gameObject);
    }
}
