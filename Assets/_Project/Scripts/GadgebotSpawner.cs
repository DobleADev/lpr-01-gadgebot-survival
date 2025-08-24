using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GadgebotSpawner : MonoBehaviour
{
	[SerializeField, Min(0)] private int _count = 3;
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
		var newGadgebot = Instantiate(prefab, transform.position, Quaternion.identity);
		onSpawn?.Invoke(newGadgebot);
		count--;
	}

	void OnValidate()
	{
		onCountUpdated?.Invoke(_count);
	}
}
