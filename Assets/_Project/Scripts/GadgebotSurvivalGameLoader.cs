using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Gadgebot Survival Loader", menuName = "Scriptable Object/Gadgebot Survival Loader")]
public class GadgebotSurvivalGameLoader : ScriptableObject
{
    [SerializeField] string setupSceneName = "gadgebot-survival-setup";
    private GadgebotSurvivalGameManager gameManager;
    private LevelDependencies levelDependencies;
    private List<string> loadedGameScenes = new List<string>();
    public bool onLoading { get; private set; }
    public event Action onGameLoadStart;
    public event Action onGameLoadEnd;

    [Serializable]
    public struct LevelDependencies
    {
        public GadgebotSurvivalSpawner GadgebotSurvivalSpawner { get; private set; }
        public GadgebotSurvivalGoal GadgebotSurvivalGoal { get; private set; }
        public LevelDependencies(GadgebotSurvivalSpawner spawner, GadgebotSurvivalGoal goal)
        {
            GadgebotSurvivalSpawner = spawner;
            GadgebotSurvivalGoal = goal;
        }

        public void CleanDependencies()
        {
            GadgebotSurvivalSpawner = null;
            GadgebotSurvivalGoal = null;
        }
    }

    [ContextMenu("Reset Properties")]
    public void Reset()
    {
        onLoading = false;
        loadedGameScenes.Clear();
    }

    bool IsGameRunning()
    {
        if (gameManager == null) return false;
        return gameManager.gameRunning;
    }

    void CancelLoading()
    {
        Reset();
    }

    public void Play(string levelName)
    {
        if (IsGameRunning() || onLoading) return;
        onLoading = true;
        onGameLoadStart?.Invoke();

        SceneManager.LoadScene(setupSceneName, LoadSceneMode.Additive);
        loadedGameScenes.Add(setupSceneName);
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        loadedGameScenes.Add(levelName);
        
    }

    public void OnSetupLoaded(GadgebotSurvivalGameManager gameManager)
    {
        if (!onLoading) return;
        this.gameManager = gameManager;
        ValidateGameStartup();
    }

    public void OnLevelLoaded(LevelDependencies levelDependencies)
    {
        if (!onLoading) return;
        this.levelDependencies = levelDependencies;
        ValidateGameStartup();
    }

    void ValidateGameStartup()
    {
        if (
            gameManager == null
            ||
            levelDependencies.GadgebotSurvivalSpawner == null
            ||
            levelDependencies.GadgebotSurvivalGoal == null
        )
        {
            return;
        }
        gameManager.spawner = levelDependencies.GadgebotSurvivalSpawner;
        gameManager.goal = levelDependencies.GadgebotSurvivalGoal;
        gameManager.StartGame();
        onLoading = false;
        onGameLoadEnd?.Invoke();
        
    }

    public void OnGameExit()
    {
        gameManager.StartCoroutine(UnloadGameScenes());
    }

    IEnumerator UnloadGameScenes()
    {
        for (int i = loadedGameScenes.Count - 1; i >= 0; i--)
        {
            string sceneToUnload = loadedGameScenes[i];
            if (SceneManager.GetSceneByName(sceneToUnload).isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToUnload);

                asyncUnload.completed += (AsyncOperation operation) =>
                {
                    Debug.Log(sceneToUnload + " has been unloaded.");
                };
            }
            else
            {
                Debug.Log(sceneToUnload + " is not currently loaded.");
            }
            loadedGameScenes.RemoveAt(i);
            yield return null;
        }
        CleanDependencies();
        
    }

    void CleanDependencies()
    {
        gameManager = null;
        levelDependencies.CleanDependencies();
    }
}
