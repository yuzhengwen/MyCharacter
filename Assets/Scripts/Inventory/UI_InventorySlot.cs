using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    private InventoryItem item;
    private ItemData itemData;

    [SerializeField]
    private GameObject uiInventoryItem;
    private Image icon;
    private TextMeshProUGUI stackSizeDisplay;
    private TextMeshProUGUI labelDisplay;

    public void Start()
    {
        icon = uiInventoryItem.transform.Find("Icon").GetComponent<Image>();
        stackSizeDisplay = uiInventoryItem.transform.Find("StackSize").GetComponent<TextMeshProUGUI>();
        labelDisplay = uiInventoryItem.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        ClearSlot();
    }
    public void ClearSlot()
    {
        uiInventoryItem.SetActive(false);
    }
    public void SetItem(InventoryItem item)
    {
        // update listeners to listen to events from new item
        if (this.item != null)
            this.item.OnStackChanged -= UpdateStackSize;
        item.OnStackChanged += UpdateStackSize;

        this.item = item;
        itemData = item.itemData;

        icon.sprite = itemData.sprite;
        stackSizeDisplay.text = item.stackSize.ToString();
        labelDisplay.text = itemData.displayName;

        uiInventoryItem.SetActive(true);
    }
    private void OnDisable()
    {
        if (item != null)
        {
            item.OnStackChanged -= UpdateStackSize;
        }
    }

    private void UpdateStackSize(InventoryItem newItem)
    {
        if (newItem.stackSize == 0)
        {
            ClearSlot();
            return;
        }
        this.item = newItem;
        stackSizeDisplay.text = item.stackSize.ToString();
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    public bool IsOccupied()
    {
        return uiInventoryItem.activeSelf;
    }
}
