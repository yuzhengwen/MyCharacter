using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private GameObject inventorySlotPrefab;
    [SerializeField]
    private int noOfSlots = 10;

    // ordered list of slots
    private readonly List<UI_InventorySlot> inventorySlots = new();

    public void Awake()
    {
        for (int i = 0; i < noOfSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, transform);
            slot.SetActive(true);
            slot.transform.SetParent(transform, false);
            inventorySlots.Add(slot.GetComponent<UI_InventorySlot>()); 
        }
    }
    public void OnEnable()
    {
        inventory.OnNewItemAdded += AddNewItem;
    }
    public void OnDisable()
    {
        inventory.OnNewItemAdded -= AddNewItem;
    }

    private void AddNewItem(InventoryItem item)
    {
        UI_InventorySlot slot = GetNextEmptySlot();
        slot.SetItem(item);
    }
    public void FillSpace(List<InventoryItem> items)
    {
        ResetInventory();
        foreach (InventoryItem item in items)
        {
            AddNewItem(item);
        }
    }
    public void ResetInventory()
    {
        foreach (UI_InventorySlot slot in inventorySlots)
        {
            slot.ClearSlot();
        }
    }
    private UI_InventorySlot GetNextEmptySlot()
    {
        foreach (UI_InventorySlot slot in inventorySlots)
        {
            if (!slot.IsOccupied())
                return slot;
        }
        Debug.LogError("No empty slots");
        return null;
    }
}