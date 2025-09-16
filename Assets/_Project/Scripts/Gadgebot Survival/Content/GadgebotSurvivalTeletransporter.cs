using System.Collections;
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
    public Coroutine process;
    public bool onTeletransportation { get; private set; }
    public bool RequestTeleport(GadgebotSurvivalGadgebotController gadgebot)
    {
        if (onTeletransportation || gadgebot == null) return false;
        onTeleportRequested?.Invoke(gadgebot);
        process = StartCoroutine(TeletransportationProcess(gadgebot));
        return true;
    }

    IEnumerator TeletransportationProcess(GadgebotSurvivalGadgebotController gadgebot)
    {
        onTeletransportation = true;
        // Debug.Log("TP WAIT");
		yield return new WaitForSeconds(teletransportationDuration);
        // Debug.Log("TP START");

        gadgebot.gameObject.SetActive(false);
        onTeleportStart?.Invoke();
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);

        onTeleportEnd?.Invoke();
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);
        // Debug.Log("TP END");

        gadgebot.gameObject.SetActive(true);
        onTeletransportation = false;
		gadgebot.transform.position = endPoint.TransformPoint(endPointOffset);
    }

    void OnDrawGizmos()
    {
        if (endPoint == null) return;
        Gizmos.DrawWireCube(endPoint.TransformPoint(endPointOffset), 0.1f * Vector2.one);
    }
}
