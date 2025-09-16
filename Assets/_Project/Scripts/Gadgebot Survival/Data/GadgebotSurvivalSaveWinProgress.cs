using UnityEngine;

public class GadgebotSurvivalSaveWinProgress : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameManager _gameManager;
    [SerializeField] GadgebotSurvivalSaveDataRepository _repository;

    private void Awake()
    {
        _gameManager.onWin.AddListener(SaveWinProgress);
    }

    void OnDestroy()
    {
        _gameManager.onWin.RemoveListener(SaveWinProgress);
    }

    void SaveWinProgress()
    {
        _repository.AddWinLevelProgressByLevelData(_gameManager.level.levelData);
        _repository.SaveGame();
    }
}
