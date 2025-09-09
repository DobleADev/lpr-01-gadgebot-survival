using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgebotSurvivalSwingTranslationProgressTest : MonoBehaviour
{
    [SerializeField] float _endDistance = 2;
    [SerializeField] float _laps = 1;
    [SerializeField] Transform _swingerJointCenter;
    [SerializeField] Transform _swingerJointEnd;
    [SerializeField] Transform _gadgebot;
    public void UpdateProgress(float value)
    {
        // float angle = 2 * value * _laps * Mathf.PI;
        // float angle = 2 * Mathf.Sin(Mathf.PI * value) * _laps * Mathf.PI;
        float angle = 2 * Mathf.Sin((2 * _laps * Mathf.PI * value) - (Mathf.PI * 0.5f)) * 0.25f * Mathf.PI;
        Vector3 endPosition = _endDistance * new Vector3(Mathf.Sin(angle), -Mathf.Abs(Mathf.Cos(angle)), 0);
        _swingerJointEnd.position = _swingerJointCenter.position + endPosition;
    }

    public void StopAutoProgress()
    {
        StopAllCoroutines();
    }

    public void PlayAutoProgress()
    {
        StopAllCoroutines();
        StartCoroutine(ProgressCoroutine());
    }

    IEnumerator ProgressCoroutine()
    {
        float progress = 0;
        while (true)
        {
            progress += Time.deltaTime;
            if (progress >= 1) progress -= 1;
            UpdateProgress(progress);
            yield return null;
        }
    }
}
