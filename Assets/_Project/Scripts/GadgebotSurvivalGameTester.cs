using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalGameTester : MonoBehaviour
{
    [SerializeField] GameObject[] _testerObjects;

    private void OnEnable()
    {
        SetActiveObjects(true);
    }

    void OnDisable()
    {
        SetActiveObjects(false);
    }

    void SetActiveObjects(bool value)
    {
        foreach (var gameObject in _testerObjects)
        {
            if (gameObject != null) gameObject.SetActive(value);
        }
    }
}
