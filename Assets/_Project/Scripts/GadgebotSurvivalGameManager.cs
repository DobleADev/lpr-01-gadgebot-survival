using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalGameManager : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGameLoader _loader;
	[SerializeField] NavigationManager _navManager;
	private GadgebotSurvivalLevelManager _level;
	public bool gameRunning { get; private set; }
	public UnityEvent onWin;
	public UnityEvent onLose;

	void Awake()
	{
		if (_loader != null) _loader.OnSetupLoaded(this);
	}

	void OnDestroy()
	{
		if (_loader != null)
		{
			if (_loader.onLoading) _loader.Reset();
			else
			{
				_level.goal.onCountUpdated.RemoveListener(CheckWin);
				_level.goal.onCountUpdated.RemoveListener((count) => CheckLose());
				_level.spawner.onSpawn.RemoveListener(OnGadgebotSpawned);
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
		if (!gameRunning || _navManager.selectables.Count != 0 || _level.spawner.count != 0) return;
		gameRunning = false;
		onLose?.Invoke();
	}

	public void InitGame(GadgebotSurvivalLevelManager levelManager)
    {
		_level = levelManager;
    }

	public void StartGame()
	{
		if (gameRunning) return;
		gameRunning = true;
		_level.goal.onCountUpdated.AddListener(CheckWin);
		_level.goal.onCountUpdated.AddListener((count) => CheckLose());
		_level.spawner.onSpawn.AddListener(OnGadgebotSpawned);
		StartCoroutine(_level.SpawnLoop());
	}

	public void QuitGame()
	{
		_loader.UnloadGame();
	}
}
