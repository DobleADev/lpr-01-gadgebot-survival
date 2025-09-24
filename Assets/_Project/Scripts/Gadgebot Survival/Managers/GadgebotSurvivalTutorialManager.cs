using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalTutorialManager : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameServices _gameServices;
    [SerializeField] GadgebotSurvivalSaveDataRepository _repository;
    [SerializeField] GadgebotSurvivalLevelManager _levelManager;
    [SerializeField] GadgebotSurvivalSpawner _spawner;
    [SerializeField] Transform _tutorialHintCenter;
    [SerializeField] GameObject _tutorialGoalPopup;
    [SerializeField] UnityEvent _onTutorialInit;
    [SerializeField] UnityEvent _onTutorialStart;
    [SerializeField] UnityEvent _onTutorialGadgebotDetonateCommand;
    [SerializeField] UnityEvent _onTutorialGadgebotExplodes;
    [SerializeField] UnityEvent _onTutorialEnd;
    GadgebotSurvivalGadgebotController _firstGadgebot;
    Camera _mainCamera;
    bool _tutorialRunning;
    bool _gadgebotExploded;
    void Awake()
    {
        if (_repository.IsGameTutorialFinished())
        {
            gameObject.SetActive(false);
            return;
        }
        _mainCamera = Camera.main;
        _spawner.onSpawn.AddListener(SetFirstGadgebot);
        _tutorialHintCenter.gameObject.SetActive(false);
        _tutorialGoalPopup.SetActive(false);
        StartCoroutine(HintFollowCoroutine());
        _onTutorialInit.Invoke();
    }

    void SetFirstGadgebot(GadgebotSurvivalGadgebotController gadgebot)
    {
        _firstGadgebot = gadgebot;
        _levelManager.PauseSpawn();
        _firstGadgebot.onCommandChanged += EvaluateFirstGadgebotCommand;
        _firstGadgebot.onDeath += OnGadgebotExplodes;
        _spawner.onSpawn.RemoveListener(SetFirstGadgebot);
    }

    IEnumerator HintFollowCoroutine()
    {
        while (true)
        {
            if (_firstGadgebot != null)
            {
                _tutorialHintCenter.position = _mainCamera.WorldToScreenPoint(_firstGadgebot.transform.position);
            }
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (_firstGadgebot == null) _spawner.onSpawn.RemoveListener(SetFirstGadgebot);
        if (_tutorialRunning)
        {
            _firstGadgebot.onCommandChanged -= EvaluateFirstGadgebotCommand;
            if (!_gadgebotExploded) _firstGadgebot.onDeath -= OnGadgebotExplodes;
        }
    }

    public void StartTutorial()
    {
        _tutorialHintCenter.gameObject.SetActive(true);
        _tutorialRunning = true;
        _onTutorialStart?.Invoke();
    }

    public void FreezeGame()
    {
        _gameServices.SetGameSpeed(0);
    }

    public void EvaluateFirstGadgebotCommand(GadgebotState gadgebotState)
    {
        if (!(gadgebotState is GadgebotDetonateCommand)) return;
        _gameServices.SetGameSpeed(1);
        _tutorialHintCenter.gameObject.SetActive(false);
        _onTutorialGadgebotDetonateCommand?.Invoke();
        _firstGadgebot.onCommandChanged -= EvaluateFirstGadgebotCommand;
    }

    public void OnGadgebotExplodes()
    {
        _gadgebotExploded = true;
        _tutorialGoalPopup.SetActive(true);
        _onTutorialGadgebotExplodes?.Invoke();
        _firstGadgebot.onDeath -= OnGadgebotExplodes;
    }

    public void EndTutorial()
    {
        _repository.FinishTutorial();
        gameObject.SetActive(false);
        _levelManager.ResumeSpawn();
        _tutorialRunning = false;

        _onTutorialEnd?.Invoke();
    }
}
