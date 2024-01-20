using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryItem
{
    public ItemData itemData;
    public int stackSize;

    public event Action<InventoryItem> OnStackChanged;

    public InventoryItem(ItemData itemData, int stackSize)
    {
        this.itemData = itemData;
        this.stackSize = stackSize;
    }
    // overflow items are returned
    public int AddToStack(int amount)
    {
        if (IsMaxStack())
        {
            return amount;
        }
        else
        {
            stackSize += amount;
            int overflow = 0;
            if (stackSize > itemData.maxStackSize)
            {
                overflow = stackSize - itemData.maxStackSize;
                stackSize = itemData.maxStackSize;
            }
            OnStackChanged?.Invoke(this);
            return overflow;
        }
    }
    // amount of items still need to be removed returned
    public int RemoveFromStack(int amount)
    {
        stackSize -= amount;
        int excess = 0;
        if (stackSize < 0)
        {
            excess = -stackSize;
            stackSize = 0;
        }
        OnStackChanged?.Invoke(this);
        return excess;
    }
    public bool IsMaxStack()
    {
        return itemData?stackSize == itemData.maxStackSize : false;
    }
}
