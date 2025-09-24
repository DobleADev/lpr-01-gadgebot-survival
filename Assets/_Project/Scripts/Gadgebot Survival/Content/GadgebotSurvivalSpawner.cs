using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class GadgebotSurvivalSpawner : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGameServices _gameServices;
	public GadgebotSurvivalGadgebotController prefab;
	public Vector3 spawnOrigin;
	public float spawnDelay = 1;
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
	public UnityEvent onSpawnRequested;
	public GadgebotUnityEvent onSpawn;
	public IntUnityEvent onCountUpdated;

	[ContextMenu("Spawn")]
	public void Spawn()
	{
		if (count <= 0) return;
		StartCoroutine(SpawnInternal());
	}

	IEnumerator SpawnInternal()
	{
		onSpawnRequested?.Invoke();
		float t = 0;
		yield return new WaitUntil(() =>
			{
				if (t >= spawnDelay)
				{
					t = 0;
					return true;
				}
				t += Time.deltaTime * _gameServices.gameSpeed;
				return false;
			});
		var newGadgebot = Instantiate(prefab, transform.TransformPoint(spawnOrigin), Quaternion.identity);
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newGadgebot.gameObject, gameObject.scene);
		onSpawn?.Invoke(newGadgebot);
		count--;
	}

	void OnValidate()
	{
		onCountUpdated?.Invoke(_count);
	}

    void OnDrawGizmos()
    {
		Gizmos.DrawWireCube(transform.TransformPoint(spawnOrigin), 0.1f * Vector2.one);
    }
}
