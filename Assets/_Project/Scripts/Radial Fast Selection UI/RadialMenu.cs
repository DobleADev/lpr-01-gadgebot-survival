using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text selectedLabel;
    public bool isOpened;
    public float selectionTimer = 2;
    public GadgebotCommandOption[] options;
    public RadialMenuItem[] uiOptions;
    public GadgebotCommandOptionUnityEvent onSelect;
    public UnityEvent onMenuOpen;
    public UnityEvent onMenuClose;
    Vector2 selectionVector;
    RadialMenuItem itemSelected;
    float selectionTime = 0;

    void Start()
    {
        canvasGroup.alpha = 0;
        for (int i = 0; i < uiOptions.Length; i++)
        {
            uiOptions[i].Init(options[i]);
        }
        selectedLabel.text = "";
    }

    void Update()
    {
        bool commandMenuInputHolded = Input.GetKey(KeyCode.Tab);
        if (!isOpened)
        {

            if (commandMenuInputHolded) Open();
        }
        else
        {
            selectionVector = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);

            HandleItemSelection();

            if (!commandMenuInputHolded) Close();
        }
    }

    void Open()
    {
        isOpened = true;
        canvasGroup.alpha = 1;
        Time.timeScale = 0;
        selectionVector = Vector3.zero;
        // EventSystem.current.sendNavigationEvents = false;
        ChangeSelection(null);
        onMenuOpen?.Invoke();
    }

    void Close()
    {
        isOpened = false;
        canvasGroup.alpha = 0;
        Time.timeScale = 1;
        SelectCommand();
        // EventSystem.current.sendNavigationEvents = true;
        onMenuClose?.Invoke();
    }

    void HandleItemSelection()
    {
        // Add a small "dead zone" to prevent accidental selection when input is near zero.
        if (selectionVector.magnitude < 0.4f)
        {
            if (selectionTime > 0) selectionTime -= Time.unscaledDeltaTime;
            else ChangeSelection(null);
            return;
        }

        float angle = Mathf.Atan2(selectionVector.y, selectionVector.x);
        angle *= Mathf.Rad2Deg;

        // Shift the angle so that it's in a 0-360 degree range.
        // Also, rotate the coordinate system so that "up" (0 degrees)
        // corresponds to our first option.
        // 90 is subtracted because Atan2 starts from the positive X-axis (right),
        // but we want our radial selector to start from the positive Y-axis (up).
        angle = 90f - angle;

        if (angle < 0)
        {
            angle += 360f;
        }

        float sectorSize = 360f / uiOptions.Length;
        int index = Mathf.FloorToInt(angle / sectorSize);

        ChangeSelection(uiOptions[index]);
    }

    void ChangeSelection(RadialMenuItem newSelection)
    {
        if (itemSelected == newSelection) return;
        if (itemSelected != null)
        {
            itemSelected.OnDeselect();
        }
        itemSelected = newSelection;
        if (newSelection != null)
        {
            selectionTime = selectionTimer; 
            itemSelected.OnSelect();
            selectedLabel.text = options[System.Array.IndexOf(uiOptions, itemSelected)].command.ToString();
        }
        else
        {
            selectedLabel.text = "";
        }
    }

    void SelectCommand()
    {
        int selectedIndex = System.Array.IndexOf(uiOptions, itemSelected);
        if (selectedIndex == -1) return;
        onSelect?.Invoke(options[selectedIndex]);
    }
}
