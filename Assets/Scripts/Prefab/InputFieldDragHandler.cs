using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect _parentScrollRect;
    private bool forwarding = false;
    private Vector2 dragStart;

    private void Awake()
    {
        _parentScrollRect = GetComponentInParent<ScrollRect>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStart = eventData.position;
        forwarding = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!forwarding)
        {
            float dragDistance = Vector2.Distance(dragStart, eventData.position);
            if (dragDistance > 10f) // 너무 작으면 손가락 떨림에도 스크롤됨
            {
                forwarding = true;
                _parentScrollRect.OnBeginDrag(eventData); // 이 타이밍에 시작!
            }
        }

        if (forwarding)
        {
            _parentScrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (forwarding)
        {
            _parentScrollRect.OnEndDrag(eventData);
            forwarding = false;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        _parentScrollRect.OnScroll(eventData);
    }
}
