using System.Collections;
using TMPro;
using UnityEngine;

public class TextTickVisualIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] float _tickSpeed = 1;
    [SerializeField] float _tickExposure = 1;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(LabelCoroutine());
    }
    
    IEnumerator LabelCoroutine()
    {
        while (true)
        {
            Color finalColor = _label.color;
            finalColor.a = Mathf.Clamp01(Mathf.Abs(Mathf.Sin(Time.time * _tickSpeed)) * _tickExposure);
            _label.color = finalColor;
            yield return null;
        }
    }
    
    // void UpdateLabel(float time)
    // {
    //     // Color finalColor = _mpb.GetColor("_FaceColor");
    //     Color finalColor = _label.color;
    //     finalColor.a = Mathf.Clamp01(Mathf.Abs(Mathf.Sin(time * _tickSpeed)) * _tickExposure);

    //     _label.color = finalColor;
    //     // _mpb.SetColor("_FaceColor", finalColor);
    //     // _labelRenderer.SetPropertyBlock(_mpb);
    // }
}
