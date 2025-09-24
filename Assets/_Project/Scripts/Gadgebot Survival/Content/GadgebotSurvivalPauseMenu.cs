using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalPauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pausePanel;
    [SerializeField] GameObject[] _gameObjectsToPause;
    [SerializeField] MonoBehaviour[] _monoBehavioursToPause;
    [SerializeField] UnityEvent _onPause;
    [SerializeField] UnityEvent _onResume;
    bool[] _beforePauseGameObjectStates;
    bool[] _beforePauseMonobehaviourStates;

    void Awake()
    {
        _beforePauseGameObjectStates = new bool[_gameObjectsToPause.Length];
        _beforePauseMonobehaviourStates = new bool[_monoBehavioursToPause.Length];
    }

    void OnDestroy()
    {
        if (_pausePanel.activeSelf) // is paused
        {
            Time.timeScale = 1;
        }
    }

    public void TogglePause()
    {
        bool isPaused = !_pausePanel.activeSelf;
        _pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            for (int i = 0; i < _gameObjectsToPause.Length; i++)
            {
                GameObject gameObjectItem = _gameObjectsToPause[i];
                _beforePauseGameObjectStates[i] = gameObjectItem.activeSelf;
                gameObjectItem.SetActive(false);
            }
            for (int i = 0; i < _monoBehavioursToPause.Length; i++)
            {
                MonoBehaviour monoBehaviour = _monoBehavioursToPause[i];
                _beforePauseMonobehaviourStates[i] = monoBehaviour.enabled;
                monoBehaviour.enabled = false;
            }
            _onPause?.Invoke();
        }
        else
        {
            for (int i = 0; i < _beforePauseGameObjectStates.Length; i++)
            {
                _gameObjectsToPause[i].SetActive(_beforePauseGameObjectStates[i]);
            }
            for (int i = 0; i < _monoBehavioursToPause.Length; i++)
            {
                _monoBehavioursToPause[i].enabled = _beforePauseMonobehaviourStates[i];
            }
            _onResume?.Invoke();
        }
    }
}
