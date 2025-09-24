using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GadgebotSurvivalLevelSelector : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader _loader;
    [SerializeField] GadgebotSurvivalSaveDataRepository _saveDataRepository;
    [SerializeField] PreventDeselectionGroup _preventDeselection;
    [SerializeField] TMP_Text _levelMetaDataLabel;
    [SerializeField] TMP_Text _levelDescriptionLabel;
    [SerializeField] GameObject _levelSelectorPanel;
    [SerializeField] Transform _levelOptionParent;
    [SerializeField] GadgebotSurvivalLevelOption _levelOptionTemplate;
    [SerializeField] GadgebotSurvivalLevelData[] _levels;
    int lastSelectedLevel = -1;
    List<GadgebotSurvivalLevelOption> _levelOptions = new List<GadgebotSurvivalLevelOption>();
    bool _levelPlaying;

    // void Awake()
    // {
    //     FetchLevels();
    // }

    void OnDestroy()
    {
        if (_levelPlaying)
        {
            _loader.onGameUnloaded -= OnLevelEnd;
        }
    }

    public void FetchLevels()
    {
        for (int i = _levelOptions.Count - 1; i >= 0; i--)
        {
            Destroy(_levelOptions[i].gameObject);
        }
        _levelOptions.Clear();
        _levelOptionTemplate.gameObject.SetActive(true);

        for (int i = 0; i < _levels.Length; i++)
        {
            var levelData = _levels[i];
            if (levelData.values.hided) continue;

            var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
            newLevelOption.gameObject.name = levelData.values.name;

            if (!_saveDataRepository.IsGameTutorialFinished() && i == 0)
            {
                newLevelOption.Init(
                    levelSelector: this,
                    levelData: levelData,
                    levelProgress: _saveDataRepository.GetLevelProgressByLevelData(levelData),
                    id: 0,
                    hasTutorial: true);
                _levelOptions.Add(newLevelOption);
                continue;
            }

            // var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
            // newLevelOption.gameObject.name = levelData.values.name;
            newLevelOption.Init(
                levelSelector: this,
                levelData: levelData,
                levelProgress: _saveDataRepository.GetLevelProgressByLevelData(levelData),
                id: i);
            _levelOptions.Add(newLevelOption);
        }

        // if (_saveDataRepository.IsGameTutorialFinished())
        // {
        //     for (int i = 0; i < _levels.Length; i++)
        //     {
        //         var levelData = _levels[i];
        //         if (levelData.values.hided) continue;

        //         var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
        //         newLevelOption.gameObject.name = levelData.values.name;
        //         newLevelOption.Init(
        //             levelSelector: this,
        //             levelData: levelData,
        //             levelProgress: _saveDataRepository.GetLevelProgressByLevelData(levelData),
        //             id: i);
        //         _levelOptions.Add(newLevelOption);
        //     }
        // }
        // else
        // {
        //     var levelData = _levels[0];
        //     var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
        //     newLevelOption.gameObject.name = levelData.values.name;
        //     newLevelOption.Init(
        //         levelSelector: this,
        //         levelData: levelData,
        //         levelProgress: _saveDataRepository.GetLevelProgressByLevelData(levelData),
        //         id: 0,
        //         hasTutorial: true);
        //     _levelOptions.Add(newLevelOption);
        // }

        _levelOptionTemplate.gameObject.SetActive(false);

        AutoSelectLevel();
    }

    public void AutoSelectLevel()
    {
        if (_levelOptions.Count > 0)
        {
            if (lastSelectedLevel == -1)
            {
                // _levelOptions[0].button.Select();
                _preventDeselection.SetNewSelected(_levelOptions[0].button);
            }
            else
            {
                // _levelOptions[lastPlayedLevel].button.Select();
                _preventDeselection.SetNewSelected(_levelOptions[lastSelectedLevel].button);
            }
        }
    }

    public void ShowSelectedLevel(int index)
    {
        lastSelectedLevel = index;
        GadgebotSurvivalLevelData level = _levels[index];
        GadgebotSurvivalSaveData.LevelProgressData levelProgress = _saveDataRepository.GetLevelProgressByLevelData(level);

        _levelMetaDataLabel.text =
        "Gadgebot Survival\n"
        // + level.values.name
        + (level.values.official ? "Official Level" : "Unofficial Level")
        + "\nTimes Completed:  " + (levelProgress == null ? "0" : levelProgress.timesCompleted.ToString());
        _levelDescriptionLabel.text = level.values.description;
    }

    public void PlayLevel(int index)
    {
        _levelPlaying = true;
        _loader.Play(_levels[index]);
        // lastPlayedLevel = index;
        _levelSelectorPanel.SetActive(false);
        _loader.onGameUnloaded += OnLevelEnd;
    }

    public void OnLevelEnd()
    {
        _levelPlaying = false;
        _levelSelectorPanel.SetActive(true);
        FetchLevels();
        _loader.onGameUnloaded -= OnLevelEnd;
    }
}
