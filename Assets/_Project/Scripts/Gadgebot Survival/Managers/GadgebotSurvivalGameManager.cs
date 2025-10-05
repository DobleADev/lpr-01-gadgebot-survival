using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GadgebotSurvivalGameManager : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGameLoader _loader;
	[SerializeField] NavigationManager _navManager;
	public GadgebotSurvivalLevelManager level { get; private set; }
	public bool gameRunning { get; private set; }
	public UnityEvent onStart;
	public UnityEvent onQuit;
	public UnityEvent onRestartProcessStart;
	public UnityEvent onRestartProcessEnd;
	public UnityEvent onWin;
	public UnityEvent onLose;
	Coroutine _spawnLoopCoroutine;

	void Awake()
	{
		if (_loader != null) _loader.OnSetupLoaded(this);
	}

	void OnDestroy()
	{
		if (_loader != null)
		{
			if (_loader.onLoading) _loader.Reset();
			else if (gameRunning)
			{
				level.goal.onCountUpdated.RemoveListener(CheckWin);
				level.goal.onCountUpdated.RemoveListener((count) => CheckLose());
				level.spawner.onSpawn.RemoveListener(OnGadgebotSpawned);
			}
		}

	}

	void OnGadgebotSpawned(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.onDestroy.AddListener(OnGadgebotDestroy);
		_navManager.AddSelectable(gadgebot);
	}

	void OnGadgebotDestroy(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.onDestroy.RemoveListener(OnGadgebotDestroy);
		_navManager.RemoveSelectable(gadgebot);
		CheckLose();
	}

	void CheckWin(int count)
	{
		if (!gameRunning || count != 0) return;
		gameRunning = false;
		onWin?.Invoke();
	}

	void CheckLose()
	{
		if (!gameRunning || _navManager.selectables.Count != 0 || level.spawner.count != 0) return;
		gameRunning = false;
		onLose?.Invoke();
	}

	public void InitGame(GadgebotSurvivalLevelManager levelManager)
	{
		level = levelManager;
	}

	public void StartGame()
	{
		if (gameRunning)
		{
			return;
		}
		gameRunning = true;
		level.goal.onCountUpdated.AddListener(CheckWin);
		level.goal.onCountUpdated.AddListener((count) => CheckLose());
		level.spawner.onSpawn.AddListener(OnGadgebotSpawned);
		onStart?.Invoke();
		_spawnLoopCoroutine = StartCoroutine(level.SpawnLoop());
	}

	public void RestartLevel()
	{
		StartCoroutine(RestartLevelCoroutine());
	}

	private IEnumerator RestartLevelCoroutine()
	{
		GadgebotSurvivalLevelData levelData = level.levelData;
		if (_spawnLoopCoroutine != null) StopCoroutine(_spawnLoopCoroutine);
		// level.goal.onCountUpdated.RemoveListener(CheckWin);
		// level.goal.onCountUpdated.RemoveListener((count) => CheckLose());
		// level.spawner.onSpawn.RemoveListener(OnGadgebotSpawned);
		// level.StopAllCoroutines();
		gameRunning = false;

		onRestartProcessStart?.Invoke();
		AsyncOperation operacionDescarga = SceneManager.UnloadSceneAsync(levelData.values.sceneName);
		while (!operacionDescarga.isDone)
		{
			yield return null;
		}
		
		AsyncOperation operacionCarga = SceneManager.LoadSceneAsync(levelData.values.sceneName, LoadSceneMode.Additive);
		float minimumLoadWait = 0.5f;
		float t = 0;
		bool enoughWait = false;
		while (!operacionCarga.isDone || !enoughWait)
		{
			yield return new WaitUntil(() =>
			{
				if (t >= minimumLoadWait)
				{
					enoughWait = true;
					return true;
				}
				t += Time.unscaledDeltaTime;
				return false;
			});
			yield return null;
		}
		StartGame();
		onRestartProcessEnd?.Invoke();
	}

	public void QuitGame()
	{
		gameRunning = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		onQuit?.Invoke();
		_loader.UnloadGame();
	}
}
