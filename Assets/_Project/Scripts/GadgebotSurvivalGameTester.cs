using UnityEngine;

public class GadgebotSurvivalGameTester : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader loader;
    [SerializeField] GadgebotSurvivalLevelData level;

    void Start()
    {
        loader.Play(level);
    }

    void OnDestroy()
    {
        loader.Reset();
    }

}
