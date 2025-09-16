using UnityEngine;

[CreateAssetMenu(fileName = "NewGadgebotSurvivalLevel", menuName = "Scriptable Object/Gadgebot Survival Level")]
public class GadgebotSurvivalLevelData : ScriptableObject
{
    [System.Serializable]
    public struct LevelDataValues
    {
        [Header("Level Meta data")]
        public string name;
        public string sceneName;
        // public int winPrize;
        [TextArea] public string description;
        [Header("Level Properties")]
        public int spawnCount;
        public int goalCount;
        public float spawnInterval;
        public bool locked;
    }

    [SerializeField] LevelDataValues _values;
    public LevelDataValues values { get { return _values; } }
}
