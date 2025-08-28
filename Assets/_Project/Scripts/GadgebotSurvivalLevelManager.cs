using UnityEngine;

public class GadgebotSurvivalLevelManager : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameLoader loader;
    [SerializeField] GadgebotSurvivalSpawner GadgebotSurvivalSpawner;
    [SerializeField] GadgebotSurvivalGoal GadgebotSurvivalGoal;
    [SerializeField] int spawnCount = 0;
    [SerializeField] int goalCount = 0;

    void Awake()
    {
        GadgebotSurvivalSpawner.count = spawnCount;
        GadgebotSurvivalGoal.count = goalCount;
        if (loader != null) loader.OnLevelLoaded(new GadgebotSurvivalGameLoader.LevelDependencies(GadgebotSurvivalSpawner, GadgebotSurvivalGoal));
    }

}
