using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Collector : MonoBehaviour
{
    private Inventory inventory;
    private void Start()
    {
        inventory = GetComponent<Inventory>();
        FindObjectOfType<UI_Inventory>().AssignInventory(inventory);

    }
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
    private void AddCollectibleToInventory(ItemDataSO data)
    {
        inventory.AddItem(data, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();
        collectible?.Collect();
    }
}
