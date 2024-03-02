using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectible {

    public static event Action<ItemDataSO> OnCoinCollected;

    public ItemDataSO itemData;

    public void Collect()
    {
        OnCoinCollected?.Invoke(itemData);
        Destroy(gameObject);
    }
}
