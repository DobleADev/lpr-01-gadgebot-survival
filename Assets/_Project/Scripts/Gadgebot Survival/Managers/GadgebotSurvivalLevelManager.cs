using System.Collections;
using UnityEngine;

public class GadgebotSurvivalLevelManager : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameServices _gameServices;
    [SerializeField] GadgebotSurvivalGameLoader _loader;
    [SerializeField] GadgebotSurvivalSpawner _spawner;
    [SerializeField] GadgebotSurvivalGoal _goal;
    [SerializeField] float _startSpawnInterval = 2;
    public GadgebotSurvivalGameManager gameManager { get; private set; }
    public GadgebotSurvivalLevelData levelData { get; private set; }
    public GadgebotSurvivalSpawner spawner { get { return _spawner; } }
    public GadgebotSurvivalGoal goal { get { return _goal; } }
    public bool canSpawn { get; private set; } = true;

    void Awake()
    {
        if (_loader != null) _loader.OnLevelLoaded(this);
    }

    void OnDestroy()
    {
        if (_loader != null)
        {
            if (_loader.onLoading) _loader.Reset();
        }
    }

    public void InitLevel(GadgebotSurvivalGameManager gameManager, GadgebotSurvivalLevelData levelData)
    {
        this.gameManager = gameManager;
        this.levelData = levelData;
        _spawner.count = levelData.values.spawnCount;
        _goal.count = levelData.values.goalCount;
    }

    public void Restart()
    {
        
    }

    public void PauseSpawn()
    {
        canSpawn = false;
    }

    public void ResumeSpawn()
    {
        canSpawn = true;
    }

    public IEnumerator SpawnLoop()
    {
        bool firstSpawned = false;
        float internalSpawnInterval = _startSpawnInterval;
        float t = 0;
        while (true)
        {
            yield return new WaitUntil(() =>
            {
                if (t >= internalSpawnInterval)
                {
                    t = 0;
                    return true;
                }
                t += Time.deltaTime * _gameServices.gameSpeed;
                return false;
            });
            if (canSpawn)
            {
                spawner.Spawn();
                if (!firstSpawned)
                {
                    internalSpawnInterval = levelData.values.spawnInterval;
                    firstSpawned = true;
                }
            }
            yield return null;
        }
    }
}
