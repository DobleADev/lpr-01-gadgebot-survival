using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalTeletransporter : MonoBehaviour
{
    public float teletransportationDuration = 2;
    public Vector3 endPointOffset;
    public Transform endPoint;
    public bool onTeletransportation;
    // public void RequestTeleport(Gadgebot gadgebot)
    // {
    //     if (onTeletransportation) return;
    //     onTeletransportation = true;
    //     StartCoroutine(TeletransportationProcess(gadgebot));
    // }

    // IEnumerator TeletransportationProcess(Gadgebot gadgebot)
    // {
    //     yield return new WaitForSeconds(teletransportationDuration);
    //     gadgebot.transform.position = endPoint.TransformPoint(endPointOffset);
    //     onTeletransportation = false;
    // }

    void OnDrawGizmos()
    {
        if (endPoint == null) return;
        Gizmos.DrawWireCube(endPoint.TransformPoint(endPointOffset), 0.1f * Vector2.one);
    }
}
