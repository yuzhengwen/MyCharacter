using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform slotParentTransform;
    private UI_InventorySlot slotParent;
    [SerializeField]
    private Transform canvas;

    // Start is called before the first frame update
    void Start()
    {
        slotParentTransform = transform.parent;
        slotParent = slotParentTransform.GetComponent<UI_InventorySlot>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(slotParentTransform, true);

        UI_InventorySlot newSlot = CheckForValidSlot();
        if (newSlot)
        {
            InventoryItem itemMoved = slotParent.GetItem();
            // if item is of the same type, stack them if possible
            if (newSlot.IsOccupied() && newSlot.GetItem() == slotParent.GetItem())
            {
                if (newSlot.GetItem().stackSize + slotParent.GetItem().stackSize <= newSlot.GetItem().itemData.maxStackSize)
                {
                    newSlot.GetItem().AddToStack(slotParent.GetItem().stackSize);
                    slotParent.ClearSlot();
                }
                else
                {
                    int amountToMove = newSlot.GetItem().itemData.maxStackSize - newSlot.GetItem().stackSize;
                    newSlot.GetItem().AddToStack(amountToMove);
                    slotParent.GetItem().RemoveFromStack(amountToMove);
                }
            }
            else
            {
                // swap items between the prev and new slot (if any)
                slotParent.ClearSlot();
                if (newSlot.IsOccupied())
                    slotParent.SetItem(newSlot.GetItem());
                newSlot.SetItem(itemMoved);
            }
        }  
    }
    private UI_InventorySlot CheckForValidSlot()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position, transform.forward, 100.0F);

        foreach (RaycastHit2D hit in hits)
        {
            UI_InventorySlot newSlot = hit.collider.gameObject.GetComponent<UI_InventorySlot>();
            if (newSlot != null && newSlot != slotParent)
                return newSlot;
        }
        return null;
    }

    // TODO add cursor icon change
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
