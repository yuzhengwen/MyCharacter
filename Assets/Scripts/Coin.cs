using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectible {

    public static event Action<ItemData> OnCoinCollected;

    public ItemData itemData;

    public void Collect()
    {
        OnCoinCollected?.Invoke(itemData);
        Destroy(gameObject);
    }
}
