using UnityEngine;

[CreateAssetMenu(fileName = "NewGadgebotSurvivalLevel", menuName = "Scriptable Object/Gadgebot Survival Level")]
public class GadgebotSurvivalLevelData : ScriptableObject
{
    [System.Serializable]
    public struct LevelDataValues
    {
        [Header("Level Meta data")]
        public string name;
        public string description;
        public int winPrize;
        public string sceneName;
        [Header("Level Properties")]
        public int spawnCount;
        public int goalCount;
        public float spawnInterval;
    }

    [SerializeField] LevelDataValues _values;
    public LevelDataValues values { get { return _values; } }
}
