using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent MouseEnter;
    public UnityEvent MouseExit;
    public UnityEvent MouseOver;

    bool hover = false;

    public void OnPointerEnter(PointerEventData data)
    {
        MouseEnter.Invoke();
        hover = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        MouseExit.Invoke();
        hover = false;
    }

    private void Update()
    {
        if (hover)
            MouseOver.Invoke();
    }
}
