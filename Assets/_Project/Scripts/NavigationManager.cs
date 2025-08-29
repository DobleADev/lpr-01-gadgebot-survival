using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class NavigationManager : MonoBehaviour
{
    public Camera gameCamera;
    public NavigationCursor reticle;
    public RadialMenu commandMenu;
    public NavigationSelectable prefab;
    public NavigationSelectable defaultSelectable;
    public NavigationSelectable currentSelectable;
    public float distanceToDeselect = 1;
    public float onDeselectCursorOffset = 1f;
    public List<NavigationSelectable> selectables = new List<NavigationSelectable>();
    public NavigationSelectable GetSelectableByGadgebot(Gadgebot gadgebot) => selectables.Where(s => s.gadgebot == gadgebot).FirstOrDefault();

    void Awake()
    {
        commandMenu.onSelect.AddListener(RequestCommandToCurrentSelectable);
        commandMenu.onMenuOpen.AddListener(OnCommandMenuOpen);
        commandMenu.onMenuClose.AddListener(OnCommandMenuClose);
        if (gameCamera == null) gameCamera = Camera.main;
    }

    void OnDestroy()
    {
        commandMenu.onSelect.RemoveListener(RequestCommandToCurrentSelectable);
        commandMenu.onMenuOpen.RemoveListener(OnCommandMenuOpen);
        commandMenu.onMenuClose.RemoveListener(OnCommandMenuClose);
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // EventSystem.current.sendNavigationEvents = false;
    }

    public void AddSelectable(Gadgebot gadgebot)
    {
        var newSelectable = Instantiate(prefab, transform);
        // gadgebot.onDestroy.AddListener(RemoveSelectable);
        newSelectable.onSelected.AddListener(() => UpdateCurrentSelectable(newSelectable));
        // newSelectable.onExited.AddListener(() => UpdateCurrentSelectable(null));
        newSelectable.gadgebot = gadgebot;
        if (gameCamera != null) newSelectable.KeepInGadgebot(gameCamera);
        selectables.Add(newSelectable);

    }

    public void RemoveSelectable(Gadgebot gadgebot)
    {
        // gadgebot.onDestroy.RemoveListener(RemoveSelectable);
        var selectableToRemove = GetSelectableByGadgebot(gadgebot);
        selectableToRemove.onSelected.RemoveListener(() => UpdateCurrentSelectable(selectableToRemove));
        // selectableToRemove.onExited.RemoveListener(() => UpdateCurrentSelectable(null));
        selectables.Remove(selectableToRemove);
        if (selectableToRemove != null) Destroy(selectableToRemove.gameObject);
    }

    void RequestCommandToCurrentSelectable(GadgebotCommandOption gadgebotCommand)
    {
        if (currentSelectable == null || currentSelectable.gadgebot == null) return;

        currentSelectable.gadgebot.RequestCommandChange(gadgebotCommand);
    }

    void UpdateCurrentSelectable(NavigationSelectable selectable)
    {
        if (commandMenu.isOpened) return;
        if (selectable == null)
        {
            currentSelectable = defaultSelectable;
            SetEventSystemSelectable(defaultSelectable);
            defaultSelectable.gameObject.SetActive(true);
            reticle.selectableSelected = null;
            // Debug.Log("Trying...");
            return;
        }
        currentSelectable = reticle.selectableSelected = selectable;
        SetEventSystemSelectable(currentSelectable);
        defaultSelectable.gameObject.SetActive(false);
        // Debug.Log("SELECTED!");
    }

    void SetEventSystemSelectable(NavigationSelectable selectable)
    {
        if (EventSystem.current == null) return;
        if (EventSystem.current.currentSelectedGameObject == selectable.gameObject) return;
        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }

    void OnCommandMenuOpen()
    {
        reticle.timeScale = 0;

    }

    void OnCommandMenuClose()
    {
        reticle.timeScale = 1;
        SetEventSystemSelectable(currentSelectable);
    }

    void LateUpdate()
    {
        if (gameCamera == null) return;
        foreach (var selectable in selectables)
        {
            selectable.KeepInGadgebot(gameCamera);
        }

        reticle.UpdatePosition();
        defaultSelectable.transform.position = reticle.GetScreenPosition();

        if (reticle.selectableSelected == null)
        {
            UpdateCurrentSelectable(GetSelectableByGadgebot(reticle.DoGadgebotRaycast()));
        }
        else
        {
            if (!reticle.selectableSelected.gadgebot.canSelect)
            {
                UpdateCurrentSelectable(null);
            }
            else if (reticle.cursorVectorOnSelected.magnitude > distanceToDeselect)
            {
                Vector3 direction = reticle.cursorVectorOnSelected.normalized;
                reticle.position += onDeselectCursorOffset * direction;
                UpdateCurrentSelectable(null);
            }
        }
    }
    
}
