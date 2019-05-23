﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointerHoverAndLeave : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent PointerEnter;
    public UnityEvent PointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit?.Invoke();
    }
}