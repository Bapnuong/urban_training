using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragableItem dragableItem = dropped.GetComponent<DragableItem>();
        dragableItem.partentAfterDrag = transform;
    }
}
