using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventorySlot slot;

    private GameObject uiInventoryItem;
    private Image icon;
    private TextMeshProUGUI stackSizeDisplay;
    private TextMeshProUGUI labelDisplay;

    private Image slotImage;
    private Color defaultColor;

    private void Awake()
    {
        uiInventoryItem = transform.GetChild(0).gameObject;
        ClearUISlot();

        slotImage = GetComponent<Image>();
        defaultColor = slotImage.color;

        icon = uiInventoryItem.transform.Find("Icon").GetComponent<Image>();
        stackSizeDisplay = uiInventoryItem.transform.Find("StackSize").GetComponent<TextMeshProUGUI>();
        labelDisplay = uiInventoryItem.transform.Find("Label").GetComponent<TextMeshProUGUI>();
    }
    /// <summary>
    /// Assigns the inventory slot to be tracked by this UI slot
    /// </summary>
    /// <param name="slot"></param>
    public void AssignSlot(InventorySlot slot)
    {
        this.slot = slot;
        slot.OnStackChanged += UpdateStackSize;
        slot.OnItemChanged += UpdateItem;
        slot.OnSlotCleared += ClearUISlot;
    }
    private void OnDisable()
    {
        if (slot != null)
        {
            slot.OnStackChanged -= UpdateStackSize;
            slot.OnItemChanged -= UpdateItem;
            slot.OnSlotCleared -= ClearUISlot;
        }
    }
    // automatically called when inventory slot being tracked becomes empty
    private void ClearUISlot()
    {
        uiInventoryItem.SetActive(false);
    }

    // automatically called when inventory slot being tracked changes item
    private void UpdateItem(InventorySlot slot)
    {
        if (!slot.IsOccupied())
        {
            ClearUISlot();
            return;
        }

        icon.sprite = slot.itemData.sprite;
        stackSizeDisplay.text = slot.stackSize.ToString();
        labelDisplay.text = slot.itemData.displayName;

        if (!uiInventoryItem.activeSelf) uiInventoryItem.SetActive(true);
    }
    // automatically called when inventory slot being tracked changes stack size
    private void UpdateStackSize(int stackSize, int change)
    {
        if (stackSize == 0)
        {
            ClearUISlot();
            return;
        }
        stackSizeDisplay.text = stackSize.ToString();
    }

    /// <summary>
    /// Returns the inventory slot being tracked
    /// </summary>
    /// <returns></returns>
    public InventorySlot GetItem()
    {
        return slot;
    }

    #region Mouse Hover effect
    public void OnPointerEnter(PointerEventData eventData)
    {
        slotImage.color = new Color(255, 255, 255, 0.8f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.color = defaultColor;
    }
    #endregion
}
