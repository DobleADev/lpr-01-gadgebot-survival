using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PreventDeselectionGroup : MonoBehaviour
{
    EventSystem evt;

    private void Awake()
    {
        evt = EventSystem.current;
    }

    GameObject sel;

    private void Update()
    {
        HandleDeselectionCheck();
    }

    private void HandleDeselectionCheck()
    {
        if (evt.currentSelectedGameObject != null && evt.currentSelectedGameObject != sel)
            sel = evt.currentSelectedGameObject;
        else if (sel != null && evt.currentSelectedGameObject == null)
            evt.SetSelectedGameObject(sel);
    }


    public void SetNewSelected(Selectable selectable)
    {
        if (selectable == null) return;
        selectable.Select();
        sel = selectable.gameObject;
        // evt.SetSelectedGameObject(selectable.gameObject);
        // Debug.Log(selectable.gameObject.name + " is the new selected");
        // evt.SetSelectedGameObject(null);
        // HandleDeselectionCheck();
    }
}