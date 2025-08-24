using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NavigationManager : MonoBehaviour
{
    public NavigationCursor reticle;
    public RadialMenu commandMenu;
    public NavigationSelectable prefab;
    public NavigationSelectable currentSelectable;
    public List<NavigationSelectable> selectables = new List<NavigationSelectable>();

    void Start()
    {
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked; 
    }

    public void AddSelectable(Gadgebot gadgebot)
    {
        var newSelectable = Instantiate(prefab, transform);
        // gadgebot.onDestroy.AddListener(RemoveSelectable);
        newSelectable.onSelected.AddListener(() => UpdateCurrentSelectable(newSelectable));
        newSelectable.onExited.AddListener(() => UpdateCurrentSelectable(null));
        newSelectable.gadgebot = gadgebot;
        newSelectable.KeepInGadgebot();
        selectables.Add(newSelectable);

    }

    public void RemoveSelectable(Gadgebot gadgebot)
    {
        // gadgebot.onDestroy.RemoveListener(RemoveSelectable);
        var selectableToRemove = selectables.Where(s => s.gadgebot == gadgebot).FirstOrDefault();
        selectableToRemove.onSelected.RemoveListener(() => UpdateCurrentSelectable(selectableToRemove));
        selectableToRemove.onExited.RemoveListener(() => UpdateCurrentSelectable(null));
        selectables.Remove(selectableToRemove);
        Destroy(selectableToRemove.gameObject);
    }

    public void UpdateCurrentSelectable(NavigationSelectable selectable)
    {
        currentSelectable = reticle.selectableSelected = selectable;
    }

    void Update()
    {
        foreach (var selectable in selectables)
        {
            selectable.KeepInGadgebot();
        }

    }
}
