using UnityEngine;
using UnityEngine.EventSystems;

public class MobileAimTrackpad : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Vector2 AimDelta { get; private set; }

    Vector2 lastPos;
    bool dragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        AimDelta = eventData.position - lastPos;
        lastPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        AimDelta = Vector2.zero;
    }
}