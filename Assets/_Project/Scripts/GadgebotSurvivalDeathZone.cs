using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalDeathZone : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;

    void OnDrawGizmos()
    {
        if (!boxCollider) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.TransformPoint(boxCollider.offset), transform.lossyScale * boxCollider.size);
    }
}
