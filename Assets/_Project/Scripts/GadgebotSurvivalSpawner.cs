using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GadgebotSurvivalSpawner : MonoBehaviour
{
	[SerializeField, Min(0)] private int _count = 3;
	public Vector3 spawnOrigin;
	public Gadgebot prefab;
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
	public GadgebotUnityEvent onSpawn;
	public IntUnityEvent onCountUpdated;

	[ContextMenu("Spawn")]
	public void Spawn()
	{
		if (count <= 0) return;
		var newGadgebot = Instantiate(prefab, transform.TransformPoint(spawnOrigin), Quaternion.identity);
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
