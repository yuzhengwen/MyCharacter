using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class UI_Inventory : MonoBehaviour
    {
        /// <summary>
        /// Inventory object containing data to display
        /// </summary>
        [SerializeField] private Inventory inventory;
        [SerializeField] private GameObject inventorySlotPrefab;

        // ordered list of slots
        private readonly List<UI_InventorySlot> inventorySlots = new();

        /// <summary>
        /// Assigns the inventory to be displayed
        /// </summary>
        /// <param name="inventory"></param>
        public void AssignInventory(Inventory inventory)
        {
            this.inventory = inventory;
            AddSlots();
        }
        private void AddSlots()
        {
            InventorySlot[] items = inventory.GetItems().ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, transform);
                slot.SetActive(true);
                slot.transform.SetParent(transform, false);
                UI_InventorySlot uiSlot = slot.GetComponent<UI_InventorySlot>();
                inventorySlots.Add(uiSlot);
                uiSlot.AssignSlot(items[i]);
            }
        }
    }
}