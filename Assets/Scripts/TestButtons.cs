using UnityEngine;

public class TestButtons : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private UI_Inventory uiInventory;
    [SerializeField]
    private ItemDataSO itemData;
    public void Add20Coins()
    {
        inventory.AddItem(itemData, 20);
        PopupManager.Instance.ShowPopup("Added 20 coins");
    }
    public void Add1Coin()
    {
        inventory.AddItem(itemData, 1);
        PopupManager.Instance.ShowPopup("Added 1 coin");
    }
    public void Remove1Coin()
    {
        inventory.RemoveItem(itemData, 1);
        PopupManager.Instance.ShowPopup("Removed 1 coin");
    }
    public void Remove20Coins()
    {
        inventory.RemoveItem(itemData, 20);
        PopupManager.Instance.ShowPopup("Removed 20 coins");
    }
    public void FillSpace()
    {
        inventory.FillSpace();
    }
}
