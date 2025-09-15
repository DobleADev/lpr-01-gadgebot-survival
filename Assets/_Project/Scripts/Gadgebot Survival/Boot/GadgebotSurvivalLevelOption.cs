using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GadgebotSurvivalLevelOption : MonoBehaviour
{
    [SerializeField] TMP_Text _levelNameLabel;
    int _id;
    GadgebotSurvivalLevelSelector _levelSelector;
    public void Init(GadgebotSurvivalLevelSelector levelSelector, GadgebotSurvivalLevelData levelData, int id)
    {
        _levelNameLabel.text = levelData.values.name;
        _levelSelector = levelSelector;
        _id = id;
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
