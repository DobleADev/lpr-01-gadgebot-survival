using UnityEngine;

public class GadgebotSurvivalLevelJukebox : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GadgebotSurvivalGameManager _gameManager;

    void Awake()
    {
        _gameManager.onStart.AddListener(PlayOST);
        _gameManager.onQuit.AddListener(StopOST);
    }

    void OnDestroy()
    {
        _gameManager.onStart.AddListener(PlayOST);
        _gameManager.onQuit.AddListener(StopOST);
    }

    void PlayOST()
    {
        _audioSource.clip = _gameManager.level.levelData.values.ost;
        _audioSource.Play();
    }

    void StopOST()
    {
        _audioSource.Stop();
    }
}
