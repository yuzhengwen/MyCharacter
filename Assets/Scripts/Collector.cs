using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    public void OnEnable()
    {
        Coin.OnCoinCollected += AddCollectibleToInventory;
        Crystal.OnCrystalCollected += AddCollectibleToInventory;
    }

    public void OnDisable()
    {
        Coin.OnCoinCollected -= AddCollectibleToInventory;
        Crystal.OnCrystalCollected -= AddCollectibleToInventory;
    }
    private void AddCollectibleToInventory(ItemData data)
    {
        inventory.AddItem(data, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();
        collectible?.Collect();
    }
}
