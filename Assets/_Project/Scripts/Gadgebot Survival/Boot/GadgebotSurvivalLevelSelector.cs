using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GadgebotSurvivalLevelSelector : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader _loader;
    [SerializeField] TMP_Text _levelMetaDataLabel;
    [SerializeField] TMP_Text _levelDescriptionLabel;
    [SerializeField] GameObject _levelSelectorPanel;
    [SerializeField] Transform _levelOptionParent;
    [SerializeField] GadgebotSurvivalLevelOption _levelOptionTemplate;
    [SerializeField] GadgebotSurvivalLevelData[] _levels;
    [SerializeField] GameObjectUnityEvent _onFetch;
    List<GadgebotSurvivalLevelOption> _levelOptions = new List<GadgebotSurvivalLevelOption>();

    void Start()
    {
        FetchLevels();
    }

    public void FetchLevels()
    {
        for (int i = _levelOptions.Count - 1; i >= 0; i--)
        {
            Destroy(_levelOptions[i]);
            _levelOptions.RemoveAt(i);
        }
        _levelOptionTemplate.gameObject.SetActive(true);
        for (int i = 0; i < _levels.Length; i++)
        {
            var newLevelOption = Instantiate(_levelOptionTemplate, _levelOptionParent);
            newLevelOption.Init(this, _levels[i], i);
            _levelOptions.Add(newLevelOption);
        }
        _levelOptionTemplate.gameObject.SetActive(false);

        if (_levelOptions.Count > 0)
        {
            _onFetch?.Invoke(_levelOptions[0].gameObject);
        }
    }

    public void ShowSelectedLevel(int index)
    {
        GadgebotSurvivalLevelData level = _levels[index];
        _levelMetaDataLabel.text =
        "Gadgebot Survival\nREWARD: " + level.values.winPrize + ".\nTimes Completed:  0";
        _levelDescriptionLabel.text = level.values.description;
    }

    public void PlayLevel(int index)
    {
        _loader.Play(_levels[index]);
        _levelSelectorPanel.SetActive(false);
        _loader.onGameUnloaded += OnLevelEnd;
    }

    public void OnLevelEnd()
    { 
        _levelSelectorPanel.SetActive(true);
        _loader.onGameUnloaded -= OnLevelEnd;
    }
}
