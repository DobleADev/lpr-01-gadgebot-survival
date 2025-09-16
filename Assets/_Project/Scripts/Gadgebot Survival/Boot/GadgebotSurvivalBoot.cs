using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalBoot : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalSaveDataRepository _repository;
    void Awake()
    {
        _repository.LoadGame();
    }
}
