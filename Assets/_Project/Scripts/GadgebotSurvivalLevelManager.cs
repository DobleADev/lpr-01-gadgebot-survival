using UnityEngine;

public class GadgebotSurvivalLevelManager : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader loader;
    [SerializeField] GadgebotSpawner gadgebotSpawner;
    [SerializeField] GadgebotGoal gadgebotGoal;

    void Awake()
    {
        loader.OnLevelLoaded(new GadgebotSurvivalGameLoader.LevelDependencies(gadgebotSpawner, gadgebotGoal));
    }

}
