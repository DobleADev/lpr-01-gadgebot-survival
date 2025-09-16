using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GadgebotSurvivalLevelOption : MonoBehaviour
{
    [SerializeField] Button _levelOptionButton;
    public Button button => _levelOptionButton;
    [SerializeField] TMP_Text _levelNameLabel;
    [SerializeField] Image _levelCompletedIndicator;
    [SerializeField] Image _levelOptionTag;
    [SerializeField] Color _levelOptionTagDefaultColor = Color.cyan;
    [SerializeField] Color _levelOptionTagUnofficialColor = Color.red;
    int _id;
    GadgebotSurvivalLevelSelector _levelSelector;
    public void Init(GadgebotSurvivalLevelSelector levelSelector, GadgebotSurvivalLevelData levelData, GadgebotSurvivalSaveData.LevelProgressData levelProgress, int id)
    {
        _levelNameLabel.text = levelData.values.name;
        _levelCompletedIndicator.enabled = levelProgress == null ? false : levelProgress.timesCompleted > 0;
        _levelSelector = levelSelector;
        _id = id;
        Color levelOptionTagColor = levelData.values.official ? _levelOptionTagDefaultColor : _levelOptionTagUnofficialColor;

        if (levelData.values.locked)
        {
            _levelOptionButton.interactable = false;
            _levelNameLabel.color = new Color(1, 1, 1, 0.5f);
        }
        _levelOptionTag.color = levelOptionTagColor;
    }

    // public void FocusSelection()
    // {
    //     // _levelOptionButton.Select();
    //     deselectionGroup.SetNewSelected(_levelOptionButton);
    // }

    public void SelectLevel()
    {
        _levelSelector.ShowSelectedLevel(_id);
    }

    public void ClickLevel()
    {
        _levelSelector.PlayLevel(_id);
    }
}
