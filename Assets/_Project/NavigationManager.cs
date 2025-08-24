using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NavigationManager : MonoBehaviour
{
    public NavigationSelectable prefab;
    public List<NavigationSelectable> selectables = new List<NavigationSelectable>();
    public NavigationSelectable currentSelectable;
    public NavigationCursor reticle;

    void Start()
    {
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked; 
    }

    public void AddSelectable(Gadgebot gadgebot)
    {
        var newSelectable = Instantiate(prefab, transform);
        gadgebot.onDestroy.AddListener(RemoveSelectable);
        newSelectable.onSelected.AddListener(() => UpdateCurrentSelectable(newSelectable));
        newSelectable.onSelected.AddListener(() => reticle.selectableSelected = newSelectable);
        newSelectable.onExited.AddListener(() => UpdateCurrentSelectable(null));
        newSelectable.onExited.AddListener(() => reticle.selectableSelected = null);
        newSelectable.gadgebot = gadgebot;
        newSelectable.KeepInGadgebot();
        selectables.Add(newSelectable);

    }

    public void RemoveSelectable(Gadgebot gadgebot)
    {
        gadgebot.onDestroy.RemoveListener(RemoveSelectable);
        var selectableToRemove = selectables.Where(s => s.gadgebot == gadgebot).FirstOrDefault();
        selectableToRemove.onSelected.RemoveListener(() => UpdateCurrentSelectable(selectableToRemove));
        selectableToRemove.onSelected.RemoveListener(() => reticle.selectableSelected = selectableToRemove);
        selectableToRemove.onExited.RemoveListener(() => UpdateCurrentSelectable(null));
        selectableToRemove.onExited.RemoveListener(() => reticle.selectableSelected = null);
        selectables.Remove(selectableToRemove);
        Destroy(selectableToRemove.gameObject);
    }

    public void UpdateCurrentSelectable(NavigationSelectable selectable)
    {
        currentSelectable = selectable;
    }

    void Update()
    {
        foreach (var selectable in selectables)
        {
            selectable.KeepInGadgebot();
        }

    }
}
