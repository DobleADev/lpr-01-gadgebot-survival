using UnityEngine;

public class GadgebotSurvivalGameTester : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader loader;
    [SerializeField] GadgebotSurvivalLevelData level;

    void Start()
    {
        loader.Play(level.levelName);
    }

    // void OnDestroy()
    // {
    //     loader.Reset();
    // }

}
