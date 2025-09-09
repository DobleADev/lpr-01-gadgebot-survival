using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalGameTester : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader loader;
    [SerializeField] GadgebotSurvivalLevelData level;
    [SerializeField] bool startOnAwake;
    [SerializeField] UnityEvent onGameLoadStart;
    [SerializeField] UnityEvent onGameLoadEnd;
    [SerializeField] UnityEvent onGameUnload;

    void Awake()
    {
        if (!startOnAwake) return;
        StartTest();
    }

    public void StartTest()
    {
        loader.Play(level);
    }

    public void ResetCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnEnable()
    {
        loader.onGameLoadStart += onGameLoadStart.Invoke;
        loader.onGameLoadEnd += onGameLoadEnd.Invoke;
        loader.onGameUnloaded += onGameUnload.Invoke;
    }

    void OnDisable()
    {
        loader.onGameLoadStart -= onGameLoadStart.Invoke;
        loader.onGameLoadEnd -= onGameLoadEnd.Invoke;
        loader.onGameUnloaded -= onGameUnload.Invoke;
    }
}
