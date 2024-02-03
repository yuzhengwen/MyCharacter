using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int slotsCount = 10;
    // ordered list of items
    [SerializeField] private List<InventorySlot> items = new();
    // unordered record of what items are already in the inventory (for quick lookup)
    private readonly HashSet<ItemDataSO> itemHashSet = new();

    // events
    public event Action<ItemDataSO, int> OnItemAdded;
    public event Action<ItemDataSO, int> OnItemRemoved;

    [SerializeField] StartingInventory startingInventory;

    private void Awake()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            items.Add(new InventorySlot());
        }
    }
    private void Start()
    {
        AddStartingInventory();
    }

    private void AddStartingInventory()
    {
        if (startingInventory)
            foreach (InventorySlot inventoryItem in startingInventory.startingItems)
                AddItem(inventoryItem);
    }

    public void AddItem(InventorySlot inventoryItem)
    {
        AddItem(inventoryItem.itemData, inventoryItem.stackSize);
    }
    public void AddItem(ItemDataSO itemData, int amount)
    {
        if (amount <= 0 || itemData == null)
        {
            Debug.LogError("Invalid amount or itemData");
            return;
        }
        OnItemAdded?.Invoke(itemData, amount);

        // find ALL inventoryitems with this itemdata and add to stack if possible
        if (itemHashSet.Contains(itemData))
        {
            foreach (InventorySlot item in items)
                if (item.itemData == itemData && !item.IsMaxStack())
                {
                    amount = item.AddToStack(amount);
                    if (amount == 0)
                        return;
                }
        }
        else
            itemHashSet.Add(itemData); 
        // if we get here, we need to add new inventoryitem stacks
        // add as many maxstacksize items as needed
        while (amount / itemData.maxStackSize > 0)
        {
            AddNewItemInternal(itemData, itemData.maxStackSize);
            amount -= itemData.maxStackSize;
        }
        if (amount > 0)
        {
            // add remainder
            AddNewItemInternal(itemData, amount);
        }
    }
    private InventorySlot AddNewItemInternal(ItemDataSO itemData, int amount)
    {
        return GetNextEmptySlot().SetItem(itemData, amount);
    }
    public void RemoveItem(ItemDataSO itemData, int amount)
    {
        if (amount <= 0 || itemData == null)
        {
            Debug.LogError("Invalid amount or itemData");
            return;
        }
        if (!itemHashSet.Contains(itemData))
        {
            Debug.LogError("Item not found in inventory!");
            return;
        }
        OnItemRemoved?.Invoke(itemData, amount);

        // Split into full and non-full stacks
        Stack<InventorySlot> fullStacks = new(), nonFullStacks = new();
        foreach (InventorySlot item in items)
        {
            if (item.itemData == itemData)
                if (item.IsMaxStack())
                    fullStacks.Push(item);
                else
                    nonFullStacks.Push(item);
        }

        // Remove from non-full stacks first
        InventorySlot itemToRemoveFrom;
        while (nonFullStacks.Count > 0 && amount != 0)
        {
            itemToRemoveFrom = nonFullStacks.Pop();
            amount = itemToRemoveFrom.RemoveFromStack(amount);
        }
        while (fullStacks.Count > 0 && amount != 0)
        {
            itemToRemoveFrom = fullStacks.Pop();
            amount = itemToRemoveFrom.RemoveFromStack(amount);
        }

        if (amount == 0)
        {
            // Remove item from hashset if no more of it is in inventory
            if (nonFullStacks.Count == 0 && fullStacks.Count == 0)
                itemHashSet.Remove(itemData);
        }
        else
            Debug.LogError($"{amount} of {itemData.displayName} couldn't be removed");

    }
    public List<InventorySlot> GetItems()
    {
        return items;
    }
    private InventorySlot GetNextEmptySlot()
    {
        return items.FirstOrDefault(item => item.itemData == null);
    }
    /// <summary>
    /// Closes up gaps in the inventory
    /// </summary>
    public void FillSpace()
    {
        InventorySlot[] filled = items.Where(item => item.itemData != null && item.stackSize > 0).ToArray();
        filled = Array.ConvertAll(filled, i => i.Copy()); // creates copy of each item (not reference)
        ClearInventory();
        for (int i = 0; i < filled.Length; i++)
        {
            items[i].SetItem(filled[i]);
        }
    }
    /// <summary>
    /// CLEARS INVENTORY
    /// </summary>
    public void ClearInventory()
    {
        foreach (InventorySlot item in items)
        {
            item.ClearSlot();
        }
    }
    public void PrintInventory()
    {
        foreach (InventorySlot item in items)
        {
            Debug.Log($"{item.itemData.displayName}: {item.stackSize}");
        }
    }
}
