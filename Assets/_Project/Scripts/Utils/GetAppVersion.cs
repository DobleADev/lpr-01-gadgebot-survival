using UnityEngine;
using UnityEngine.Events;

public class GetAppVersion : MonoBehaviour
{
    [SerializeField] string _versionLabel = "Version ";
    [SerializeField] StringUnityEvent _onAwakeGetVersion;
    private void Awake()
    {
        _onAwakeGetVersion?.Invoke(_versionLabel + Application.version);
    }
}

[System.Serializable]
public class StringUnityEvent : UnityEvent<string> { }