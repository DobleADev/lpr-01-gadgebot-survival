using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalTeletransporter : MonoBehaviour
{
    public float teletransportationDuration = 2;
    public Vector3 endPointOffset;
    public Transform endPoint;
    public float teleportTransitionDuration = 1.4f;
    public GadgebotUnityEvent onTeleportRequested;
    public UnityEvent onTeleportStart;
    public UnityEvent onTeleportEnd;
    public bool onTeletransportation;
    public void RequestTeleport(GadgebotSurvivalGadgebotController gadgebot)
    {
        if (onTeletransportation) return;
        onTeleportRequested?.Invoke(gadgebot);
        StartCoroutine(TeletransportationProcess(gadgebot));
    }

    IEnumerator TeletransportationProcess(GadgebotSurvivalGadgebotController gadgebot)
    {
        onTeletransportation = true;
		gadgebot.walk = false;
		gadgebot.onTeletransportation = true;

		yield return new WaitForSeconds(teletransportationDuration);

        gadgebot.gameObject.SetActive(false);
        onTeleportStart?.Invoke();
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);

        onTeleportEnd?.Invoke();
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);

        onTeletransportation = false;
        gadgebot.gameObject.SetActive(true);
		gadgebot.transform.position = endPoint.TransformPoint(endPointOffset);
		gadgebot.walk = true;
		gadgebot.onTeletransportation = false;
		gadgebot.RequestDefaultCommand();
    }

    void OnDrawGizmos()
    {
        if (endPoint == null) return;
        Gizmos.DrawWireCube(endPoint.TransformPoint(endPointOffset), 0.1f * Vector2.one);
    }
}
