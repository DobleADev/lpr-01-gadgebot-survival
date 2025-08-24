using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationSelectable : Selectable
{
    public Gadgebot gadgebot;
    public UnityEvent onSelected;
    public UnityEvent onExited;
    bool isSelected = false;

    public void KeepInGadgebot()
    {
        Vector2 gadgebotScreenPosition = Camera.main.WorldToScreenPoint(gadgebot.transform.position);
        transform.position = gadgebotScreenPosition;
    }

    public void WhenSelected()
    {
        if (EventSystem.current.currentSelectedGameObject != null
        && EventSystem.current.currentSelectedGameObject != gameObject
        && !isSelected) return;
        isSelected = true;
        onSelected?.Invoke();
    }

    public void WhenExited()
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject) return;

        EventSystem.current.SetSelectedGameObject(null);
        onExited?.Invoke();
    }
}
