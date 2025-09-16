using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GadgebotSurvivalLevelOption : MonoBehaviour
{
    [SerializeField] Button _levelOptionButton;
    [SerializeField] TMP_Text _levelNameLabel;
    [SerializeField] Image _levelCompletedIndicator;
    int _id;
    GadgebotSurvivalLevelSelector _levelSelector;
    public void Init(GadgebotSurvivalLevelSelector levelSelector, GadgebotSurvivalLevelData levelData, GadgebotSurvivalSaveData.LevelProgressData levelProgress, int id)
    {
        _levelNameLabel.text = levelData.values.name;
        _levelCompletedIndicator.enabled = levelProgress == null ? false : levelProgress.timesCompleted > 0;
        _levelSelector = levelSelector;
        _id = id;

        if (levelData.values.locked)
        {
            _levelOptionButton.interactable = false;
            _levelNameLabel.color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void FocusSelection()
    {
        _levelOptionButton.Select();
    }

    public void SelectLevel()
    {
        _levelSelector.ShowSelectedLevel(_id);
    }

    public void ClickLevel()
    {
        _levelSelector.PlayLevel(_id);
    }
}
