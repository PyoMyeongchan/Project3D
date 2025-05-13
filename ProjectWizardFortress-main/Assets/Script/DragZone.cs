using UnityEngine;
using UnityEngine.EventSystems;

public class DragZone: MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<DraggableUI>();
        if (dragged != null && dragged.ReplacementPrefab != null)
        {
            Instantiate(dragged.ReplacementPrefab, transform.position, Quaternion.identity, transform.parent);
            
        }
    }
}
