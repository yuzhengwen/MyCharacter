using System;

namespace InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        public ItemDataSO itemData;
        public int stackSize;

        public event Action<int, int> OnStackChanged;
        public event Action<InventorySlot> OnItemChanged;
        public event Action OnSlotCleared;

        public void ClearSlot()
        {
            itemData = null;
            stackSize = 0;
            OnSlotCleared?.Invoke();
        }
        public InventorySlot SetItem(InventorySlot slot)
        {
            if (slot == null)
            {
                ClearSlot(); return this;
            }
            return SetItem(slot.itemData, slot.stackSize);
        }
        public InventorySlot SetItem(ItemDataSO itemData, int stackSize)
        {
            if (itemData == null || stackSize <= 0)
                ClearSlot();
            this.itemData = itemData;
            this.stackSize = stackSize;
            OnItemChanged?.Invoke(this);
            return this;
        }
        /// <summary>
        /// overflow items are returned
        /// </summary>
        /// <param name="amount">Amount to add to stack</param>
        /// <returns></returns>
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
                OnStackChanged?.Invoke(stackSize, amount);
                return overflow;
            }
        }
        /// <summary>
        /// amount of items still need to be removed returned
        /// </summary>
        /// <param name="amount">Amount to remove from stack</param>
        /// <returns></returns>
        public int RemoveFromStack(int amount)
        {
            stackSize -= amount;
            int excess = 0;
            if (stackSize < 0)
            {
                excess = -stackSize;
                ClearSlot();
            }
            OnStackChanged?.Invoke(stackSize, -amount);
            return excess;
        }
        public bool IsMaxStack()
        {
            return itemData ? stackSize == itemData.maxStackSize : false;
        }
        public bool IsOccupied()
        {
            return itemData != null;
        }
        /// <summary>
        /// Creates a full copy of the slot (not a reference)
        /// </summary>
        /// <returns></returns>
        public InventorySlot Copy()
        {
            return new InventorySlot { itemData = itemData, stackSize = stackSize };
        }
        /// <summary>
        /// Swaps the data between the two slots
        /// If slot passed is null, the slot is cleared
        /// </summary>
        public void Swap(InventorySlot slot)
        {
            InventorySlot temp = slot.Copy();
            slot.SetItem(this);
            SetItem(temp);
        }
    }
}
