using UnityEngine;
using UnityEngine.EventSystems;

public class UI_InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform slotParentTransform;
    private UI_InventorySlot slotParent;
    [SerializeField] private Transform canvas;

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
            InventorySlot item1 = slotParent.GetItem(); // item being dragged
            InventorySlot item2 = newSlot.GetItem(); // item being dragged onto

            // if item is of the same type, stack them if possible
            if (item2.IsOccupied() && item1 == item2)
            {
                if (item2.stackSize + item1.stackSize <= item2.itemData.maxStackSize)
                {
                    item2.AddToStack(slotParent.GetItem().stackSize);
                    item1.ClearSlot();
                }
                else
                {
                    int amountToMove = item2.itemData.maxStackSize - item2.stackSize;
                    item2.AddToStack(amountToMove);
                    item1.RemoveFromStack(amountToMove);
                }
            }
            else
                item1.Swap(item2);
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
