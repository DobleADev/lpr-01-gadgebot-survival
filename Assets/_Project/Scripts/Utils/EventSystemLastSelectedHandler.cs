using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemLastSelectedHandler : MonoBehaviour
{
    [SerializeField] EventSystem _eventSystem;
    [SerializeField] StandaloneInputModule _inputModule;
    public Selectable _lastSelectable { get; private set; }
    void Start()
    {
        
    }
    
    void OnDestroy()
    {
        
    }
}
