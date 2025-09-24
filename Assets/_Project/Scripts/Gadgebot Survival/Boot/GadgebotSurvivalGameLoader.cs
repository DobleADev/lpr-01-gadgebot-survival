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
    private GadgebotSurvivalLevelManager levelManager;
    private GadgebotSurvivalLevelData levelData;
    // private LevelDependencies levelDependencies;
    private List<string> loadedGameScenes = new List<string>();
    public bool onLoading { get; private set; }
    public event Action onGameLoadStart;
    public event Action onGameLoadEnd;
    public event Action onGameUnloaded;

    
    bool IsGameRunning()
    {
        if (gameManager == null) return false;
        return gameManager.gameRunning;
    }

    void ValidateGameStartup()
    {
        if (
            gameManager == null
            ||
            levelManager == null
        )
        {
            return;
        }
        levelManager.InitLevel(gameManager, levelData);
        gameManager.InitGame(levelManager);
        gameManager.StartGame();
        onLoading = false;
        onGameLoadEnd?.Invoke();
        
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
                    // Debug.Log(sceneToUnload + " has been unloaded.");
                };
            }
            else
            {
                // Debug.Log(sceneToUnload + " is not currently loaded.");
            }
            loadedGameScenes.RemoveAt(i);
            yield return null;
        }
        CleanDependencies();
        
    }

    void CleanDependencies()
    {
        gameManager = null;
        levelManager = null;
        levelData = null;
    }

    public void Play(GadgebotSurvivalLevelData gadgebotSurvivalLevel)
    {
        if (IsGameRunning() || onLoading) return;
        onLoading = true;
        onGameLoadStart?.Invoke();

        SceneManager.LoadScene(setupSceneName, LoadSceneMode.Additive);
        loadedGameScenes.Add(setupSceneName);
        SceneManager.LoadScene(gadgebotSurvivalLevel.values.sceneName, LoadSceneMode.Additive);
        loadedGameScenes.Add(gadgebotSurvivalLevel.values.sceneName);
        levelData = gadgebotSurvivalLevel;
    }

    public void OnSetupLoaded(GadgebotSurvivalGameManager gameManager)
    {
        if (!onLoading) return;
        this.gameManager = gameManager;
        ValidateGameStartup();
    }

    public void OnLevelLoaded(GadgebotSurvivalLevelManager levelManager)
    {
        if (!onLoading)
        {
            if (gameManager != null) // Restart Level
            {
                this.levelManager = levelManager;
                levelManager.InitLevel(gameManager, levelData);
                gameManager.InitGame(levelManager);
                // gameManager.StartGame();
            }

            return;
        }
        this.levelManager = levelManager;
        ValidateGameStartup();
    }

    public void UnloadGame()
    {
        gameManager.StartCoroutine(UnloadGameScenes());
        onGameUnloaded?.Invoke();
    }

    [ContextMenu("Reset Properties")]
    public void Reset()
    {
        onLoading = false;
        loadedGameScenes.Clear();
    }
}
