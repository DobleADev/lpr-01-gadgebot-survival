using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuItem : MonoBehaviour
{
    public float normalScale = 1f;
    public float selectedScale = 1.25f;
    public Color normalFrameColor = Color.yellow;
    public Color selectedFrameColor = Color.white;
    public Image frame;
    public Image icon;
    public void Init(GadgebotCommandOption option)
    {
        icon.sprite = option.sprite;
    }

    public void OnSelect()
    {
        frame.color = selectedFrameColor;
        transform.localScale = selectedScale * Vector3.one;
    }

    public void OnDeselect()
    {
        frame.color = normalFrameColor;
        transform.localScale = normalScale * Vector3.one;
    }
}
