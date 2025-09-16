using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GadgebotSurvivalLevelSelector : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader _loader;
    [SerializeField] GadgebotSurvivalSaveDataRepository _saveDataRepository;
    [SerializeField] TMP_Text _levelMetaDataLabel;
    [SerializeField] TMP_Text _levelDescriptionLabel;
    [SerializeField] GameObject _levelSelectorPanel;
    [SerializeField] Transform _levelOptionParent;
    [SerializeField] GadgebotSurvivalLevelOption _levelOptionTemplate;
    [SerializeField] GadgebotSurvivalLevelData[] _levels;
    int lastPlayedLevel = -1;
    List<GadgebotSurvivalLevelOption> _levelOptions = new List<GadgebotSurvivalLevelOption>();

    void OnEnable()
    {
        FetchLevels();
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
            var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
            newLevelOption.Init(
                this,
                _levels[i],
                _saveDataRepository.GetLevelProgressByLevelData(_levels[i]),
                i);
            _levelOptions.Add(newLevelOption);
        }
        _levelOptionTemplate.gameObject.SetActive(false);

        AutoSelectLevel();
    }

    public void AutoSelectLevel()
    {
        if (_levelOptions.Count > 0)
        {
            if (lastPlayedLevel == -1)
                _levelOptions[0].FocusSelection();
            else
                _levelOptions[lastPlayedLevel].FocusSelection();
        }
    }

    public void ShowSelectedLevel(int index)
    {
        GadgebotSurvivalLevelData level = _levels[index];
        GadgebotSurvivalSaveData.LevelProgressData levelProgress = _saveDataRepository.GetLevelProgressByLevelData(level);
        
        _levelMetaDataLabel.text =
        "Gadgebot Survival\n"
        // + level.values.name
        + "\nTimes Completed:  " + (levelProgress == null ? "0" : levelProgress.timesCompleted.ToString()) ;
        _levelDescriptionLabel.text = level.values.description;
    }

    public void PlayLevel(int index)
    {
        _loader.Play(_levels[index]);
        lastPlayedLevel = index;
        _levelSelectorPanel.SetActive(false);
        _loader.onGameUnloaded += OnLevelEnd;
    }

    public void OnLevelEnd()
    { 
        _levelSelectorPanel.SetActive(true);
        _loader.onGameUnloaded -= OnLevelEnd;
    }
}
