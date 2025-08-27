using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalDirectionBlock : MonoBehaviour
{
    public int direction { get { return Mathf.RoundToInt(Vector2.Dot(transform.right, Vector2.right)); } }
}
