using UnityEngine;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform reserveUnitsPanel;

    public void OnDrag(PointerEventData eventData) => reserveUnitsPanel.anchoredPosition += eventData.delta;
}
