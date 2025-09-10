using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgebotSurvivalSwingTranslationProgressTest : MonoBehaviour
{
    [SerializeField] float _autoPlaySpeed = 1;
    [SerializeField] float _endStartOffset = 0.5f;
    [SerializeField] float _endDistance = 2;
    [SerializeField] float _laps = 1;
    [SerializeField] Transform _swingerJointCenter;
    [SerializeField] Transform _swingerJointEnd;
    [SerializeField] Transform _gadgebotTransform;
    [SerializeField] Vector3 _gadgebotStartPos;
    [SerializeField] Vector3 _gadgebotEndPos;
    [SerializeField] float _gadgebotSwingPositionOffset = 0.35f;
    [SerializeField] float _gadgebotStartJumpMaxApex = 1;
    [SerializeField] float _gadgebotStartJumpOffset = 0;
    [SerializeField] float _gadgebotStartJumpDuration = 0.25f;
    [SerializeField] float _gadgebotEndJumpMaxApex = 1;
    [SerializeField] float _gadgebotEndJumpOffset = 0;
    [SerializeField] float _gadgebotEndJumpDuration = 0.25f;
    public void UpdateProgress(float value)
    {
        // float angle = 2 * value * _laps * Mathf.PI;
        // float angle = 2 * Mathf.Sin(Mathf.PI * value) * _laps * Mathf.PI;
        float lapCohefficent = 2 * _laps * Mathf.PI;

        // Swinger
        float angle = 2 * Mathf.Sin((lapCohefficent * value) - (Mathf.PI * _endStartOffset)) * 0.25f * Mathf.PI;
        Vector3 endPosition = _endDistance * new Vector3(Mathf.Sin(angle), -Mathf.Abs(Mathf.Cos(angle)), 0);
        _swingerJointEnd.position = _swingerJointCenter.position + endPosition;

        // Jump Start
        float startJumpDeltaDuration = 1 / _gadgebotStartJumpDuration;
        float gadebotStartJumpProgress = startJumpDeltaDuration * value;
        float gadebotStartJumpPredictAngle = 2 * Mathf.Sin((lapCohefficent * _gadgebotStartJumpDuration) - (Mathf.PI * _endStartOffset)) * 0.25f * Mathf.PI;
        Vector3 gadgebotStartJumpEndPosition = _swingerJointCenter.position + _gadgebotSwingPositionOffset * Vector3.left + (_endDistance * new Vector3(Mathf.Sin(gadebotStartJumpPredictAngle), -Mathf.Abs(Mathf.Cos(gadebotStartJumpPredictAngle)), 0));

        // Jump End
        float endJumpDeltaDuration = 1 / _gadgebotEndJumpDuration;
        // float gadebotEndJumpProgress = Mathf.Clamp(gadebotStartJumpProgress - (startJumpDeltaDuration * (1 - _gadgebotEndJumpDuration)), -1, 1);
        float gadebotEndJumpProgress = Mathf.Clamp((endJumpDeltaDuration * value) - (endJumpDeltaDuration * (1 - _gadgebotEndJumpDuration)), -1, 1);
        // float gadebotEndJumpProgress = value - (1 - _gadgebotEndJumpDuration);
        float gadebotEndJumpPredictAngle = 2 * Mathf.Sin(lapCohefficent - (lapCohefficent * _gadgebotEndJumpDuration) - (Mathf.PI * _endStartOffset)) * 0.25f * Mathf.PI;
        Vector3 gadgebotEndJumpEndPosition = _swingerJointCenter.position + _gadgebotSwingPositionOffset * Vector3.left + (_endDistance * new Vector3(Mathf.Sin(gadebotEndJumpPredictAngle), -Mathf.Abs(Mathf.Cos(gadebotEndJumpPredictAngle)), 0));

        if (gadebotStartJumpProgress < 1)
        {
            _gadgebotTransform.position = ParabolicLerp(_gadgebotStartPos, gadgebotStartJumpEndPosition, gadebotStartJumpProgress, _gadgebotStartJumpMaxApex);
        }
        else if (gadebotEndJumpProgress >= 0)
        {
            _gadgebotTransform.position = ParabolicLerp(gadgebotEndJumpEndPosition, _gadgebotEndPos, gadebotEndJumpProgress, _gadgebotEndJumpMaxApex);
        }
        else
        {
            _gadgebotTransform.position = _swingerJointEnd.position + _gadgebotSwingPositionOffset * Vector3.left;
        }

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
            progress += _autoPlaySpeed * Time.deltaTime;
            if (progress >= 1) progress -= 1;
            UpdateProgress(progress);
            yield return null;
        }
    }

    /// <summary>
    /// Calculates a parabolic position between two points.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="endPos">The target position.</param>
    /// <param name="t">The time parameter, typically between 0 and 1.</param>
    /// <param name="jumpHeight">The maximum height of the jump arc.</param>
    /// <returns>The calculated position along the parabolic path.</returns>
    public static Vector3 ParabolicLerp(Vector3 startPos, Vector3 endPos, float t, float jumpHeight)
    {
        // Straight-line interpolation
        Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

        // Parabolic Y offset
        float yOffset = jumpHeight * 4 * t * (1 - t);
        currentPos.y += yOffset;

        return currentPos;
    }
}
