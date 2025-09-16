using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalGameManager : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGameLoader _loader;
	[SerializeField] NavigationManager _navManager;
	[SerializeField] bool _lockCursorOnPlay = true;
	public GadgebotSurvivalLevelManager level { get; private set; }
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
		if (_lockCursorOnPlay)
		{
			Cursor.visible = false;
        	Cursor.lockState = CursorLockMode.Locked;
		}
		
    }

	public void StartGame()
	{
		if (gameRunning) return;
		gameRunning = true;
		level.goal.onCountUpdated.AddListener(CheckWin);
		level.goal.onCountUpdated.AddListener((count) => CheckLose());
		level.spawner.onSpawn.AddListener(OnGadgebotSpawned);
		StartCoroutine(level.SpawnLoop());
	}

	public void QuitGame()
	{
		Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
		_loader.UnloadGame();
	}
}
