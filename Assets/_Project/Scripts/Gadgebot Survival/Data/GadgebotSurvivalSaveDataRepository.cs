using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Gadgebot Survival Data Repository", menuName = "Scriptable Object/Gadgebot Survival Save Data Repository")]
public class GadgebotSurvivalSaveDataRepository : ScriptableObject
{
    [SerializeField] private string _webSaveDataKey = "dobleadev_gadgebot_survival_savedata";
    [SerializeField] private GadgebotSurvivalSaveData _currentData = new GadgebotSurvivalSaveData();
    private GenericSaveLoadManager _saveManager = new GenericSaveLoadManager();
    // public GadgebotSurvivalSaveData currentData { get { return _currentData; } }
    
    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        _currentData = new GadgebotSurvivalSaveData();
    }

    public void SaveGame()
    {
        _saveManager.SaveGameData(_webSaveDataKey, _currentData);
    }

    public void LoadGame()
    {
        var loadedGameData = _saveManager.LoadGameData(_webSaveDataKey);
        if (loadedGameData == null) return;
        _currentData = loadedGameData;
    }

    public GadgebotSurvivalSaveData.LevelProgressData GetLevelProgressByLevelData(GadgebotSurvivalLevelData levelData)
    {
        // Debug.Log(_currentData == null ? "_currentData doesn't exists" : "_currentData does exists");
        // Debug.Log(_currentData.levelProgress == null ? "levelProgress doesn't exists" : "levelProgress does exists");
        return _currentData.levelProgress.FirstOrDefault(level => level.id == levelData.GetInstanceID());
    }

    public void AddWinLevelProgressByLevelData(GadgebotSurvivalLevelData levelData)
    {
        GadgebotSurvivalSaveData.LevelProgressData levelProgress = GetLevelProgressByLevelData(levelData);
        if (levelProgress == null)
        {
            levelProgress = new GadgebotSurvivalSaveData.LevelProgressData(levelData.GetInstanceID());
            _currentData.levelProgress.Add(levelProgress);
        }
        levelProgress.timesCompleted++;
    } 
    // public bool TryGetLevelProgress(GadgebotSurvivalLevelData levelData, out GadgebotSurvivalSaveData.LevelProgressData levelProgress)
    // {
    //     levelProgress = _currentData.levelProgress.FirstOrDefault(level => level.id == levelData.GetInstanceID());
    //     return _currentData.levelProgress.Contains(levelProgress);
    // } 

    // public void LoadData(GadgebotSurvivalSaveData saveData)
    // {
    //     _currentData = saveData;
    // }
}

[System.Serializable]
public class GadgebotSurvivalSaveData
{
    // [SerializeField] private Level[] _levelProgress;
    // public Level[] levelProgress { get { return _levelProgress; } set { _levelProgress = value; } }
    public List<LevelProgressData> levelProgress = new List<LevelProgressData>();

    [System.Serializable]
    public class LevelProgressData
    {
        public int id;
        public int timesCompleted;
        public LevelProgressData(int id)
        {
            this.id = id;
        }
    }
}