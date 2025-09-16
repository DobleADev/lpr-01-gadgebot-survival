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
    private GadgebotSurvivalGadgebotController _gadgebotTeletransporting;
    public bool canBeWaitedFor { get; private set; } = true;

    public bool RequestTeleport(GadgebotSurvivalGadgebotController gadgebot)
    {
        if (onTeletransportation || gadgebot == null || _gadgebotTeletransporting != null) return false;
        _gadgebotTeletransporting = gadgebot;
        onTeleportRequested?.Invoke(_gadgebotTeletransporting);
        process = StartCoroutine(TeletransportationProcess());
        return true;
    }

    IEnumerator TeletransportationProcess()
    {
        onTeletransportation = true;
        canBeWaitedFor = false;
        // Debug.Log("TP WAIT");
        yield return new WaitForSeconds(teletransportationDuration);
        // Debug.Log("TP START");

        _gadgebotTeletransporting.gameObject.SetActive(false);
        onTeleportStart?.Invoke();
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);

        onTeleportEnd?.Invoke();
        canBeWaitedFor = true;
		yield return new WaitForSeconds(teleportTransitionDuration * 0.5f);
        // Debug.Log("TP END");

        _gadgebotTeletransporting.gameObject.SetActive(true);
		_gadgebotTeletransporting.transform.position = endPoint.TransformPoint(endPointOffset);
        onTeletransportation = false; 
        _gadgebotTeletransporting = null;
    }

    void OnDrawGizmos()
    {
        if (endPoint == null) return;
        Gizmos.DrawWireCube(endPoint.TransformPoint(endPointOffset), 0.1f * Vector2.one);
    }
}
