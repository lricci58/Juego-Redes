using UnityEngine;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectPanel;
    [SerializeField] private Canvas canvas;

    public void OnDrag(PointerEventData eventData) => dragRectPanel.anchoredPosition += eventData.delta / canvas.scaleFactor;

    public void OnPointerDown(PointerEventData eventData) => dragRectPanel.SetAsLastSibling();
}
