using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalSwinger : MonoBehaviour
{
    [SerializeField] Transform _jointCenter;
    [SerializeField] float _swingSpeed = 1;
    [SerializeField] float _maxSwingAngle = 90;
    GadgebotSurvivalGadgebotController _gadgebotSwinging;
    
    void Update()
    {
        _jointCenter.rotation = Quaternion.AngleAxis(_maxSwingAngle * Mathf.Sin(_swingSpeed * Time.time), Vector3.back);
    }
}
