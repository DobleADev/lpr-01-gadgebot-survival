using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalSwinger : MonoBehaviour
{
    [Header("Swinger Properties")]
    [SerializeField] float _autoPlaySpeed = 1;
    [SerializeField] float _swingSpeed = 1;
    [SerializeField] float _swingDistance = 2;
    [SerializeField] float _swingStartOffset = 0f;
    [SerializeField] private float _swingEdgeThreshold = 0.999f;
    [SerializeField] private float _jointEndJumpStartPredictionDebug;
    [Header("References")]
    [SerializeField] Transform _swingerJointCenter;
    [SerializeField] Transform _swingerJointEnd;
    [SerializeField] GadgebotSurvivalGadgebotController _gadgebotSwinging;
    [Header("Gadgebot Positioning")]
    [SerializeField] Vector3 _gadgebotStartPos;
    [SerializeField] Vector3 _gadgebotEndPos;
    [SerializeField] float _gadgebotSwingPositionOffset = 0.35f;
    [Header("Gadgebot Start Jump Values")]
    [SerializeField] float _gadgebotStartJumpMaxApex = 1;
    [SerializeField] float _gadgebotStartJumpOffset = 0;
    [SerializeField] float _gadgebotStartJumpDuration = 0.25f;
    [Header("Gadgebot End Jump Values")]
    [SerializeField] float _gadgebotEndJumpMaxApex = 1;
    [SerializeField] float _gadgebotEndJumpOffset = 0;
    [SerializeField] float _gadgebotEndJumpDuration = 0.25f;
    public float swingSpeed { get { return _swingSpeed; } set { _swingSpeed = value; } }
    // public float jointProgress { get; private set; }
    public float jointProgress { get; private set; }
    public bool onSwinging { get; private set; }
    private bool _isSwingingLeft = true;
    private int _swingPhase = -1;
    float _gadgebotProgress = 0;

    private void Start()
    {
        StartCoroutine(SwingProcess());
    }

    public bool RequestSwing(GadgebotSurvivalGadgebotController gadgebot)
    {
        if (_gadgebotSwinging != null
        // || gadgebot.direction * jointProgress < 0
        || gadgebot.direction * (_isSwingingLeft ? 1 : -1) < 0)
        {
            return false;
        }
        // if (gadgebot.direction * jointProgress < 0) return false;

        // _jointProgressOffset = jointProgress;
        _gadgebotStartPos = gadgebot.transform.position;
        _gadgebotSwinging = gadgebot;
        onSwinging = true;
        _gadgebotProgress = 0;
        _swingPhase = -1;
        return true;
    }

    IEnumerator SwingProcess()
    {
        float timeElapsed = 0;
        while (true)
        {
            timeElapsed += _autoPlaySpeed * Time.deltaTime;
            if (timeElapsed >= 1) timeElapsed -= 1;
            float lapCohefficent = 2 * _swingSpeed * Mathf.PI;

            // Swinger
            jointProgress = 2 * Mathf.Sin((lapCohefficent * timeElapsed) - (Mathf.PI * _swingStartOffset)) * 0.25f * Mathf.PI;
            Vector3 endPosition = _swingDistance * new Vector3(Mathf.Sin(jointProgress), -Mathf.Abs(Mathf.Cos(jointProgress)), 0);
            _swingerJointCenter.rotation = Quaternion.AngleAxis(jointProgress * -Mathf.Rad2Deg, Vector3.back);
            _swingerJointEnd.position = _swingerJointCenter.position + endPosition;

            float swingEdgeBound = _swingEdgeThreshold - Time.deltaTime;

            if (_isSwingingLeft)
            {
                if (jointProgress <= -swingEdgeBound)
                {
                    _isSwingingLeft = false;
                }
            }
            else
            {
                if (jointProgress >= swingEdgeBound)
                {
                    _isSwingingLeft = true;
                }
            }
            if (_gadgebotSwinging == null) onSwinging = false;

            // Swing process 
            if (onSwinging)
            {
                float gadgebotStartSwingOffset = 0;
                float gadgebotEndSwingOffset = 0;

                //Phase -1 - Wait
                if (_swingPhase == -1)
                {
                    if (_gadgebotSwinging.direction * jointProgress <= 0)
                    {
                        gadgebotStartSwingOffset = -_gadgebotSwinging.direction * jointProgress;
                        _gadgebotProgress = 0;
                        _swingPhase++;
                    }
                }

                //Phase 0 - Start
                // Take Gadgebot wait position (predict swing joint angle)
                // Lerp Gadgebot from wait to swinger end point position
                else if (_swingPhase == 0)
                {
                    // Jump Start
                    float startJumpDeltaDuration = 1 / _gadgebotStartJumpDuration;
                    float gadebotStartJumpProgress = startJumpDeltaDuration * _gadgebotProgress;
                    float gadebotStartJumpPredictAngle = 2 * Mathf.Sin((lapCohefficent * _gadgebotStartJumpOffset) - (Mathf.PI * (_swingStartOffset + gadgebotStartSwingOffset))) * 0.25f * Mathf.PI;
                    Vector3 gadgebotStartJumpEndPosition = _swingerJointCenter.position + _gadgebotSwingPositionOffset * Vector3.left + (_swingDistance * new Vector3(Mathf.Sin(gadebotStartJumpPredictAngle), -Mathf.Abs(Mathf.Cos(gadebotStartJumpPredictAngle)), 0));

                    _gadgebotSwinging.transform.position = ParabolicLerp(_gadgebotStartPos, gadgebotStartJumpEndPosition, gadebotStartJumpProgress, _gadgebotStartJumpMaxApex);

                    if (_gadgebotProgress >= _gadgebotStartJumpDuration)
                    {
                        _gadgebotProgress = 0;
                        _swingPhase++;
                    }
                }

                //Phase 1 - Swing
                // Translate Gadgebot within swinger joint end
                else if (_swingPhase == 1)
                {
                    _gadgebotSwinging.transform.position = _swingerJointEnd.position + _gadgebotSwingPositionOffset * Vector3.left;
                    // if (jointProgress >= swingEdgeBound - _gadgebotEndJumpDuration) // TEMP - Take direction into account
                    if (_gadgebotSwinging.direction * jointProgress >= 1 - (_gadgebotEndJumpDuration - _gadgebotEndJumpOffset)) // TEMP - Take direction into account
                    {
                        gadgebotEndSwingOffset = _gadgebotSwinging.direction * jointProgress;
                        _gadgebotProgress = 0;
                        _swingPhase++;
                    }
                }

                //Phase 2 - End
                // Take Gadgebot last position
                // Lerp Gadgebot from last position to the next ground
                else if (_swingPhase == 2)
                {
                    // Jump End
                    float endJumpDeltaDuration = 1 / _gadgebotEndJumpDuration;
                    float gadebotEndJumpProgress = endJumpDeltaDuration * _gadgebotProgress;
                    // float gadebotEndJumpPredictAngle = 2 * Mathf.Sin((lapCohefficent * (1f - (_gadgebotEndJumpDuration - _gadgebotEndJumpOffset))) - (Mathf.PI * (_swingStartOffset))) * 0.25f * Mathf.PI;
                    float gadebotEndJumpPredictAngle = 2 * Mathf.Sin((lapCohefficent * (_jointEndJumpStartPredictionDebug)) - (Mathf.PI * (_swingStartOffset + gadgebotEndSwingOffset - _gadgebotEndJumpOffset))) * 0.25f * Mathf.PI;
                    Vector3 gadgebotEndJumpEndPosition = _swingerJointCenter.position + _gadgebotSwingPositionOffset * Vector3.left + (_swingDistance * new Vector3(Mathf.Sin(gadebotEndJumpPredictAngle), -Mathf.Abs(Mathf.Cos(gadebotEndJumpPredictAngle)), 0));

                    _gadgebotSwinging.transform.position = ParabolicLerp(gadgebotEndJumpEndPosition, transform.TransformPoint(_gadgebotEndPos), gadebotEndJumpProgress, _gadgebotEndJumpMaxApex);

                    if (_gadgebotProgress >= _gadgebotEndJumpDuration)
                    {
                        _gadgebotSwinging.transform.position = transform.TransformPoint(_gadgebotEndPos);
                        _gadgebotSwinging.EndSwing();
                        onSwinging = false;
                        _gadgebotSwinging = null;
                    }
                }

                _gadgebotProgress += Time.deltaTime;
            }
            yield return null;
        }
    }

    // void Update()
    // {
    //     jointProgress = Mathf.Sin(_swingSpeed * Time.time);
    //     _jointCenterTransform.rotation = Quaternion.AngleAxis(_maxSwingAngle * jointProgress, Vector3.back);
    // }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.TransformPoint(_gadgebotEndPos), 0.2f * Vector2.one);
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

// public class GadgebotSurvivalSwinger : MonoBehaviour
// {
//     [SerializeField] Transform _jointCenterTransform;
//     [SerializeField] Transform _jointEndTransform;
//     [SerializeField] float _swingSpeed = 1;
//     [SerializeField] float _maxSwingAngle = 90;
//     [SerializeField] private float _threshold = 0.99f;
//     GadgebotSurvivalGadgebotController _gadgebotSwinging;
//     public float swingSpeed { get { return _swingSpeed; } set { _swingSpeed = value; } }
//     public float jointProgress { get; private set; }
//     public bool onSwinging { get; private set; }
//     private bool _isSwingingLeft = true;
//     [SerializeField] private float _jointProgressOffset;
//     private Vector3 _gadgebotWaitPosition;

//     private void Start()
//     {
//         StartCoroutine(SwingProcess());
//     }

//     public bool RequestSwing(GadgebotSurvivalGadgebotController gadgebot)
//     {
//         if (_gadgebotSwinging != null
//         || gadgebot.direction * jointProgress < 0
//         || gadgebot.direction * (_isSwingingLeft ? 1 : -1) < 0)
//         {
//             return false;
//         }
//         // if (gadgebot.direction * jointProgress < 0) return false;

//         // _jointProgressOffset = jointProgress;
//         _gadgebotWaitPosition = gadgebot.transform.position;
//         _gadgebotSwinging = gadgebot;
//         onSwinging = true;
//         return true;
//     }

//     IEnumerator SwingProcess()
//     {
//         float t = 0;
//         while (true)
//         {
//             t += Time.deltaTime;
//             float maxProgress = Mathf.PI * Mathf.Rad2Deg;
//             if (t >= maxProgress) t -= maxProgress;
//             jointProgress = Mathf.Sin(_swingSpeed * t);
//             _jointCenterTransform.rotation = Quaternion.AngleAxis(_maxSwingAngle * jointProgress, Vector3.back);

//             if (_isSwingingLeft)
//             {
//                 // ...y ha alcanzado el final de su recorrido derecho
//                 if (jointProgress >= _threshold)
//                 {
//                     // Debug.Log("Pendulo ha llegado al lado DERECHO. ¡Puedes soltar o entregar el objeto!");
//                     if (onSwinging)
//                     {
//                         if (_gadgebotSwinging.direction == -1)
//                         {
//                             onSwinging = false;
//                             _gadgebotSwinging = null;
//                         }
//                     }
//                     _isSwingingLeft = false;
//                     // Aquí va la lógica para soltar el objeto, activar un evento, etc.
//                 }
//             }
//             // Si el péndulo se está moviendo a la izquierda...
//             else
//             {
//                 // ...y ha alcanzado el final de su recorrido izquierdo
//                 if (jointProgress <= -_threshold)
//                 {
//                     // Debug.Log("Pendulo ha llegado al lado IZQUIERDO. ¡Puedes tomar o entregar el objeto!");
//                     if (onSwinging)
//                     {
//                         if (_gadgebotSwinging.direction == 1)
//                         {
//                             onSwinging = false;
//                             _gadgebotSwinging = null;
//                         }
//                     }
//                     _isSwingingLeft = true;
//                     // Aquí va la lógica para soltar el objeto, activar un evento, etc.
//                 }
//             }
//             if (_gadgebotSwinging == null) onSwinging = false;
//             if (onSwinging)
//             {
//                 // Vector3 jointEndPosition = _jointCenterTransform.TransformPoint(_jointEndTransform.localPosition);
//                 Vector3 jointEndPosition = _jointEndTransform.position + new Vector3(_gadgebotSwinging.direction * -0.4f, 0, 0);
//                 if (_gadgebotSwinging.direction * (_isSwingingLeft ? -1 : 1) < 0)
//                 {
//                     // jointEndPosition.x = _gadgebotSwinging.transform.position.x;
//                     // jointEndPosition.z = _gadgebotSwinging.transform.position.z;
//                     // _gadgebotSwinging.transform.position = Vector3.Lerp(_gadgebotWaitPosition, jointEndPosition, Mathf.Abs(jointProgress) + Mathf.Abs(_jointProgressOffset));
//                 }
//                 else
//                 {
//                     _gadgebotSwinging.transform.position = jointEndPosition;
//                 }
//             }
//             yield return null;
//         }
//     }

//     // void Update()
//     // {
//     //     jointProgress = Mathf.Sin(_swingSpeed * Time.time);
//     //     _jointCenterTransform.rotation = Quaternion.AngleAxis(_maxSwingAngle * jointProgress, Vector3.back);
//     // }
// }
