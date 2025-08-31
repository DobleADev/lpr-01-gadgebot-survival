using System.Collections;
using UnityEngine;

public class GadgebotSurvivalLevelManager : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader _loader;
    [SerializeField] GadgebotSurvivalSpawner _spawner;
    [SerializeField] GadgebotSurvivalGoal _goal;
    [SerializeField] float _startSpawnInterval = 2;
    private GadgebotSurvivalLevelData _levelData;
    public GadgebotSurvivalSpawner spawner { get { return _spawner; } }
    public GadgebotSurvivalGoal goal { get { return _goal; } }

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

    public void InitLevel(GadgebotSurvivalLevelData levelData)
    {
        _levelData = levelData;
        _spawner.count = levelData.values.spawnCount;
        _goal.count = levelData.values.goalCount;
    }
    
    public IEnumerator SpawnLoop()
	{
		bool firstSpawned = false;
		float internalSpawnInterval = _startSpawnInterval;
		while (true)
		{
			yield return new WaitForSeconds(internalSpawnInterval);
			spawner.Spawn();
			if (!firstSpawned)
			{
				internalSpawnInterval = _levelData.values.spawnInterval;
				firstSpawned = true;
			}
		}
	}
}
