using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // ordered list of items
    private readonly List<InventoryItem> items= new();
    // unordered record of what items are already in the inventory (for quick lookup)
    private readonly HashSet<ItemData> itemHashSet = new();

    // events
    public event Action<InventoryItem> OnNewItemAdded;

    [SerializeField] StartingInventory startingInventory;

    private void Start()
    {
        AddStartingInventory();
    }

    private void AddStartingInventory()
    {
        if (startingInventory)
        {
            for (int i = 0; i < startingInventory.items.Count; i++)
            {
                AddItem(startingInventory.items[i], startingInventory.amounts[i]);
            }
        }
    }

    public void AddItem(ItemData itemData, int amount)
    {
        // find ALL inventoryitems with this itemdata and add to stack if possible
        if (itemHashSet.Contains(itemData))
        {
            foreach (InventoryItem item in items)
            {
                if (item.itemData == itemData && !item.IsMaxStack())
                {
                    amount = item.AddToStack(amount);
                    if (amount == 0) {
                        PrintInventory();
                        return; 
                    }
                }
            }
        }
        // if we get here, we need to add new inventoryitem
        InventoryItem newItem;
        itemHashSet.Add(itemData);
        // add as many maxstacksize items as needed
        while (amount/itemData.maxStackSize > 0)
        {
            newItem = new(itemData, itemData.maxStackSize);
            items.Add(newItem);
            OnNewItemAdded?.Invoke(newItem);
            amount -= itemData.maxStackSize;
        }
        if (amount > 0) {
            // add remainder
            newItem = new(itemData, amount);
            items.Add(newItem);
            OnNewItemAdded?.Invoke(newItem);
        }
        PrintInventory();
    }
    public void RemoveItem(ItemData itemData, int amount)
    {
        if (itemHashSet.Contains(itemData))
        {
            Stack<InventoryItem> fullStacks = new(), nonFullStacks = new();
            foreach (InventoryItem item in items)
            {
                if (item.itemData == itemData)
                    if (item.IsMaxStack())
                        fullStacks.Push(item);
                    else
                        nonFullStacks.Push(item);
            }
            InventoryItem itemToRemoveFrom;
            while (nonFullStacks.Count > 0 && amount != 0)
            {
                itemToRemoveFrom = nonFullStacks.Pop();
                amount = itemToRemoveFrom.RemoveFromStack(amount);
                if (itemToRemoveFrom.stackSize == 0)
                    items.Remove(itemToRemoveFrom);
            }
            while (fullStacks.Count > 0 && amount != 0)
            {
                itemToRemoveFrom = fullStacks.Pop();
                amount = itemToRemoveFrom.RemoveFromStack(amount);
                if (itemToRemoveFrom.stackSize == 0)
                    items.Remove(itemToRemoveFrom);
            }

            if (amount == 0)
            {
                if (nonFullStacks.Count == 0 && fullStacks.Count == 0)
                    itemHashSet.Remove(itemData);
            }
            else
                Debug.LogError($"{amount} of {itemData.displayName} couldn't be removed");
        }
        else
        {
            Debug.LogError("Item not found in inventory!");
        }
        PrintInventory();
    }
    public void PrintInventory() {        
        foreach (InventoryItem item in items)
        {
            Debug.Log($"{item.itemData.displayName}: {item.stackSize}");
        }
    }
    public List<InventoryItem> GetItems()
    {
        return items;
    }
}
