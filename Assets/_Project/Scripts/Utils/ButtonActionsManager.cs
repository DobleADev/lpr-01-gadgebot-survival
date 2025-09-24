using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ButtonActionsManager : MonoBehaviour
{
    [SerializeField] private ButtonActions[] _buttonActions;

    private void OnValidate()
    {
        InjectButtonActions();
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            InjectButtonActions();
        }
    }

    private void InjectButtonActions()
    {
        if (_buttonActions == null) return;
        for (int i = 0; i < _buttonActions.Length; i++)
        {
            var buttonAction = _buttonActions[i];
            if (buttonAction.button == null) 
            {
                buttonAction.Name = "Element " + i;
                continue;
            }
            buttonAction.Name = buttonAction.button.name;
            if (buttonAction.actions != null)
            {
                // Limpiar los listeners existentes antes de agregar los nuevos.
                buttonAction.button.onClick.RemoveAllListeners();
                buttonAction.button.onClick.AddListener(buttonAction.actions.Invoke);
            }
        }
    }
}

[System.Serializable]
public class ButtonActions
{
    [HideInInspector] public string Name;
    public Button button;
    public Button.ButtonClickedEvent actions;
}