using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixScrollRect: MonoBehaviour, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField] private ScrollRect _mainScroll;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _mainScroll.OnBeginDrag(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        _mainScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _mainScroll.OnEndDrag(eventData);
    }


    public void OnScroll(PointerEventData data)
    {
        _mainScroll.OnScroll(data);
    }


}