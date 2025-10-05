using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixScrollRect : MonoBehaviour, ISelectHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField] private ScrollRect _mainScroll;
    [Tooltip("Velocidad de interpolación para el desplazamiento suave.")]
    public float ScrollSpeed = 10f;

    private Coroutine _scrollToCenterCoroutine;

    public void OnSelect(BaseEventData eventData)
    {
        Canvas.ForceUpdateCanvases(); 
        RectTransform selectedRect = (RectTransform)eventData.selectedObject.transform;
        RectTransform viewportRect = _mainScroll.viewport != null ? _mainScroll.viewport : (RectTransform)_mainScroll.transform;

        if (selectedRect == null || viewportRect == null) return;
        Vector3 selectedPositionInViewport = viewportRect.InverseTransformPoint(selectedRect.position);
        Vector2 targetAnchoredPosition = _mainScroll.content.anchoredPosition;

        if (_mainScroll.vertical)
        {
            float yOffset = selectedPositionInViewport.y;
            float itemHeight = selectedRect.rect.height;
            float pivotYOffset = selectedRect.pivot.y - 2.75f;
            float pivotCorrection = itemHeight * pivotYOffset;
            targetAnchoredPosition.y -= yOffset;
            targetAnchoredPosition.y += pivotCorrection;
        }

        if (_mainScroll.horizontal)
        {
            float xOffset = selectedPositionInViewport.x;

            float itemWidth = selectedRect.rect.width;
            float pivotXOffset = selectedRect.pivot.x - 0.5f;
            float pivotCorrection = itemWidth * pivotXOffset;
            targetAnchoredPosition.x -= xOffset;
            targetAnchoredPosition.x -= pivotCorrection;
        }

        _mainScroll.content.anchoredPosition = targetAnchoredPosition;
        // if (_scrollToCenterCoroutine != null)
        // {
        //     StopCoroutine(_scrollToCenterCoroutine);
        // }
        // _scrollToCenterCoroutine = StartCoroutine(SmoothScroll(targetAnchoredPosition));
    }

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

    /// <summary>
    /// Corrutina para mover el Content a la posición objetivo de forma gradual.
    /// </summary>
    private IEnumerator SmoothScroll(Vector2 targetPosition)
    {
        // Importante: Forzar la actualización inmediata para que las posiciones sean exactas antes de empezar
        Canvas.ForceUpdateCanvases(); 
        Vector2 initialPosition = _mainScroll.content.anchoredPosition;
        
        // Usar un pequeño margen para finalizar el loop
        while (Vector2.Distance(initialPosition, targetPosition) > 0)
        {
            _mainScroll.content.anchoredPosition = Vector2.MoveTowards(
                initialPosition,
                targetPosition,
                Time.deltaTime * ScrollSpeed
            );
            yield return null;
        }

        // Asegurar que el scroll termina exactamente en la posición objetivo
        _mainScroll.content.anchoredPosition = targetPosition;
        _scrollToCenterCoroutine = null;
    }

}