using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalGame : MonoBehaviour
{
	public GadgebotSpawner spawner;
	public GadgebotGoal goal;
	public NavigationManager navManager;
	public UnityEvent onWin;
	public UnityEvent onLose;
	public bool startGameOnStart;
	public bool gameRunning;
	public float startSpawnInterval = 2;
	public float spawnInterval = 3;

	void Awake()
	{
		goal.onCountUpdated.AddListener(CheckWin);
		goal.onCountUpdated.AddListener((count) => CheckLose());
		spawner.onSpawn.AddListener(OnGadgebotSpawned);
	}

	void OnDestroy()
	{
		goal.onCountUpdated.RemoveListener(CheckWin);
		goal.onCountUpdated.RemoveListener((count) => CheckLose());
		spawner.onSpawn.RemoveListener(OnGadgebotSpawned);
	}

	void OnGadgebotSpawned(Gadgebot gadgebot)
	{
		gadgebot.onDestroy.AddListener(OnGadgebotDestroy);
		navManager.AddSelectable(gadgebot);
	}

	void OnGadgebotDestroy(Gadgebot gadgebot)
	{
		gadgebot.onDestroy.RemoveListener(OnGadgebotDestroy);
		navManager.RemoveSelectable(gadgebot);
		CheckLose();
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
		if (gameRunning) return;
		gameRunning = true;
		StartCoroutine(SpawnLoop());
	}

	void CheckWin(int count)
	{
		if (!gameRunning || count != 0) return;
		gameRunning = false;
		onWin?.Invoke();
	}

	void CheckLose()
	{
		if (!gameRunning || navManager.selectables.Count != 0) return;
		gameRunning = false;
		onLose?.Invoke();
	}
	
}
