using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GadgebotSurvivalTeletransporter : MonoBehaviour
{
    [SerializeField] GadgebotSurvivalGameServices _gameServices;
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
        float t = 0;
		yield return new WaitUntil(() =>
			{
				if (t >= teletransportationDuration)
				{
					t = 0;
					return true;
				}
				t += Time.deltaTime * _gameServices.gameSpeed;
				return false;
			});
        // Debug.Log("TP START");

        _gadgebotTeletransporting.gameObject.SetActive(false);
        onTeleportStart?.Invoke();
        t = 0;
		yield return new WaitUntil(() =>
			{
				if (t >= teleportTransitionDuration * 0.5f)
				{
					t = 0;
					return true;
				}
				t += Time.deltaTime * _gameServices.gameSpeed;
				return false;
			});

        onTeleportEnd?.Invoke();
        canBeWaitedFor = true;
        t = 0;
		yield return new WaitUntil(() =>
			{
				if (t >= teleportTransitionDuration * 0.5f)
				{
					t = 0;
					return true;
				}
				t += Time.deltaTime * _gameServices.gameSpeed;
				return false;
			});
        // Debug.Log("TP END");

        _gadgebotTeletransporting.gameObject.SetActive(true);
		_gadgebotTeletransporting.transform.position = endPoint.TransformPoint(endPointOffset);
        _gadgebotTeletransporting.EndTeletransportation();
        _gadgebotTeletransporting = null;
        onTeletransportation = false;
    }

    void OnDrawGizmos()
    {
        if (endPoint == null) return;
        Gizmos.DrawWireCube(endPoint.TransformPoint(endPointOffset), 0.1f * Vector2.one);
    }
}
