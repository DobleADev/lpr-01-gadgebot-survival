using UnityEngine;

public class GadgebotSurvivalFunctionabilityTester : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalLevelData _levelData;
    [SerializeField] GadgebotSurvivalLevelManager _levelManager;
    [SerializeField] GadgebotSurvivalGameManager _gameManager;
    void Start()
    {
        _levelManager.InitLevel(_levelData);
        _gameManager.InitGame(_levelManager);
        _gameManager.StartGame();
    }
}
