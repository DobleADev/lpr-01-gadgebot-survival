using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalSwinger : MonoBehaviour
{
    [SerializeField] Transform _jointCenterTransform;
    [SerializeField] Transform _jointEndTransform;
    [SerializeField] float _swingSpeed = 1;
    [SerializeField] float _maxSwingAngle = 90;
    [SerializeField] private float _threshold = 0.99f;
    GadgebotSurvivalGadgebotController _gadgebotSwinging;
    public float swingSpeed { get { return _swingSpeed; } set { _swingSpeed = value; } }
    public float jointProgress { get; private set; }
    public bool onSwinging { get; private set; }
    private bool _isSwingingLeft = true;
    private float _jointProgressOffset;
    private Vector3 _gadgebotWaitPosition;

    private void Start()
    {
        StartCoroutine(SwingProcess());
    }

    public bool RequestSwing(GadgebotSurvivalGadgebotController gadgebot)
    {
        if (_gadgebotSwinging != null
        || gadgebot.direction * jointProgress < 0
        || gadgebot.direction * (_isSwingingLeft ? 1 : -1) < 0)
        {
            return false;
        }
        // if (gadgebot.direction * jointProgress < 0) return false;

        _jointProgressOffset = jointProgress;
        _gadgebotWaitPosition = gadgebot.transform.position;
        _gadgebotSwinging = gadgebot;
        onSwinging = true;
        return true;
    }

    IEnumerator SwingProcess()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            float maxProgress = Mathf.PI * Mathf.Rad2Deg;
            if (t >= maxProgress) t -= maxProgress;
            jointProgress = Mathf.Sin(_swingSpeed * t);
            _jointCenterTransform.rotation = Quaternion.AngleAxis(_maxSwingAngle * jointProgress, Vector3.back);

            if (_isSwingingLeft)
            {
                // ...y ha alcanzado el final de su recorrido derecho
                if (jointProgress >= _threshold)
                {
                    // Debug.Log("Pendulo ha llegado al lado DERECHO. ¡Puedes soltar o entregar el objeto!");
                    if (onSwinging)
                    {
                        if (_gadgebotSwinging.direction == -1)
                        {
                            onSwinging = false;
                            _gadgebotSwinging = null;
                        }
                    }
                    _isSwingingLeft = false;
                    // Aquí va la lógica para soltar el objeto, activar un evento, etc.
                }
            }
            // Si el péndulo se está moviendo a la izquierda...
            else
            {
                // ...y ha alcanzado el final de su recorrido izquierdo
                if (jointProgress <= -_threshold)
                {
                    // Debug.Log("Pendulo ha llegado al lado IZQUIERDO. ¡Puedes tomar o entregar el objeto!");
                    if (onSwinging)
                    {
                        if (_gadgebotSwinging.direction == 1)
                        {
                            onSwinging = false;
                            _gadgebotSwinging = null;
                        }
                    }
                    _isSwingingLeft = true;
                    // Aquí va la lógica para soltar el objeto, activar un evento, etc.
                }
            }
            if (_gadgebotSwinging == null) onSwinging = false;
            if (onSwinging)
            {
                // Vector3 jointEndPosition = _jointCenterTransform.TransformPoint(_jointEndTransform.localPosition);
                Vector3 jointEndPosition = _jointEndTransform.position + new Vector3(_gadgebotSwinging.direction * -0.4f, 0, 0);
                // if (_gadgebotSwinging.direction * (_isSwingingLeft ? -1 : 1) < 0)
                // {
                //     jointEndPosition.x = _gadgebotSwinging.transform.position.x;
                //     jointEndPosition.z = _gadgebotSwinging.transform.position.z;
                //     _gadgebotSwinging.transform.position = Vector3.Lerp(_gadgebotWaitPosition, jointEndPosition, Mathf.Abs(jointProgress) - Mathf.Abs(_jointProgressOffset));
                // }
                // else
                {
                    _gadgebotSwinging.transform.position = jointEndPosition;
                }
            }
            yield return null;
        }
    }

    // void Update()
    // {
    //     jointProgress = Mathf.Sin(_swingSpeed * Time.time);
    //     _jointCenterTransform.rotation = Quaternion.AngleAxis(_maxSwingAngle * jointProgress, Vector3.back);
    // }
}
