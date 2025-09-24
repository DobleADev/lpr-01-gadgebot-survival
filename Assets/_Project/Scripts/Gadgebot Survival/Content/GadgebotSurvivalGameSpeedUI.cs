using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GadgebotSurvivalGameSpeedUI : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameServices _gameServices;
    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text _label;
    [SerializeField] float _maxSpeed = 2;
    [SerializeField] float _steps = 20;

    void Awake()
    {
        ResetGameSpeed();
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            _slider.value += Mathf.CeilToInt(scroll);
            SetGameSpeed(_slider.value);
        }
    }

    private void OnValidate()
    {
        _slider.maxValue = _steps;
        SetGameSpeed(_slider.value);
    }

    public void SetGameSpeed(float speed)
    {
        float newSpeed = speed * _maxSpeed / _steps;
        _gameServices.SetGameSpeed(newSpeed);
        _label.text = newSpeed.ToString("n1") + "x";
    }
    
    public void ResetGameSpeed()
    {
        float defaultSliderValue = _steps / _maxSpeed;
        SetGameSpeed(_slider.value = defaultSliderValue);
    }
}
