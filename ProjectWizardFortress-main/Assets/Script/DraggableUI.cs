using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("이 UI가 드롭될 때 교체될 프리팹")]
    [SerializeField] private GameObject replacementPrefab;
    public GameObject ReplacementPrefab => replacementPrefab;

    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Image dragPanel;
    private Vector2 originalPosition;

    private void Awake()
    {
        dragPanel = GameObject.Find("DragPanel")?.GetComponent<Image>();
        if (dragPanel == null)
        {
            Debug.LogError("dragPanel를 찾을 수 없습니다. 'dragPanel'라는 이름의 Canvas를 하나 만들어주세요.");
        }

        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
    }
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;
        originalPosition = rect.anchoredPosition;

        transform.SetParent(dragPanel.transform, false);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            dragPanel.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos
        );
        rect.localPosition = localPos;
        
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // DropZone 탐색
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var dropZone = result.gameObject.GetComponent<DragZone>();
            if (dropZone != null)
            {
                dropZone.OnDrop(eventData);
                return; // 드롭 성공 시 종료
            }
        }

        // 실패 시 원위치로 복귀
        transform.SetParent(previousParent, false);
        rect.anchoredPosition = originalPosition;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}
