using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GadgebotSurvivalGoal : MonoBehaviour
{
    [SerializeField, Min(0)] private int _count = 3;
    public int count
    {
        get
        {
            return _count;
        }

        set
        {
            _count = value;
            onCountUpdated?.Invoke(_count);
        }
    }
    public GadgebotUnityEvent onReachedGoal;
    public IntUnityEvent onCountUpdated;

    public void OnInteract(GadgebotSurvivalGadgebotController gadgebot)
    {
        onReachedGoal?.Invoke(gadgebot);
        Destroy(gadgebot.gameObject);
        if (count > 0) count--;
    }

    void OnValidate()
    {
        onCountUpdated?.Invoke(_count);
    }
}
