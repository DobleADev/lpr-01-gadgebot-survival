using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalGame : MonoBehaviour 
{
	public GadgebotSpawner spawner;
	public GadgebotGoal goal;
	public NavigationManager navManager;
	public GameObject winPopup; 
	public bool startGameOnStart;
	public float startSpawnInterval = 2;
	public float spawnInterval = 3;

	void Awake()
	{
		goal.onCountUpdated.AddListener(CheckWin);
		spawner.onSpawn.AddListener(navManager.AddSelectable);
	}

	void OnDestroy()
	{
		goal.onCountUpdated.RemoveListener(CheckWin);
		spawner.onSpawn.RemoveListener(navManager.AddSelectable);
	}

    void Start()
    {
        if (startGameOnStart) StartGame();
    }

	IEnumerator SpawnLoop()
	{
		bool firstSpawned = false;
		float internalSpawnInterval = startSpawnInterval;
		while (true)
		{
			yield return new WaitForSeconds(internalSpawnInterval);
			spawner.Spawn();
			if (!firstSpawned)
			{
				internalSpawnInterval = spawnInterval;
				firstSpawned = true;
			}
		}
	}

    public void StartGame()
	{
		StartCoroutine(SpawnLoop());
	}

	public void CheckWin(int count)
	{
		if (count != 0) return;
		winPopup.SetActive(true);
	}

	
}
