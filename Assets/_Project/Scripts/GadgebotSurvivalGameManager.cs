using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalGameManager : MonoBehaviour
{
	public GadgebotSurvivalGameLoader loader;
	public GadgebotSpawner spawner;
	public GadgebotGoal goal;
	public NavigationManager navManager;
	public bool startGameOnStart;
	public bool gameRunning { get; private set; }
	public float startSpawnInterval = 2;
	public float spawnInterval = 3;
	public UnityEvent onWin;
	public UnityEvent onLose;

	void Awake()
	{
		if (loader != null) loader.OnSetupLoaded(this);
	}

	void Start()
	{
		if (startGameOnStart) StartGame();
	}

	void OnDestroy()
	{
		if (loader != null && gameRunning) loader.Reset();
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
		goal.onCountUpdated.AddListener(CheckWin);
		goal.onCountUpdated.AddListener((count) => CheckLose());
		spawner.onSpawn.AddListener(OnGadgebotSpawned);
		StartCoroutine(SpawnLoop());
	}

	void CheckWin(int count)
	{
		if (!gameRunning || count != 0) return;
		gameRunning = false;
		onWin?.Invoke();
		// OnGameEnd();
		// loader.OnGameExit();
	}

	void CheckLose()
	{
		if (!gameRunning || navManager.selectables.Count != 0) return;
		gameRunning = false;
		onLose?.Invoke();
		// OnGameEnd();
		// loader.OnGameExit();
	}
	
}
