using UnityEngine;

[CreateAssetMenu(fileName = "NewGadgebotSurvivalLevel", menuName = "Scriptable Object/Gadgebot Survival Level")]
public class GadgebotSurvivalLevelData : ScriptableObject
{
    [SerializeField] string _levelName = "gadgebot-survival-level";
    public string levelName { get { return _levelName; } }
}
