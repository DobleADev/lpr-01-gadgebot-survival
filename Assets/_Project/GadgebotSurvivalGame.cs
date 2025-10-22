using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalGame : MonoBehaviour 
{
	public GadgebotSpawner spawner;
	public GadgebotGoal goal;
	public GameObject winPopup; 

	void Awake ()
	{
		goal.onCountUpdated.AddListener(CheckWin);
	}

	public void StartGame()
	{
		
	}

	public void CheckWin(int count)
	{
		if (count != 0) return;

		winPopup.SetActive(true);
	}

	void OnDestroy ()
	{
		goal.onCountUpdated.RemoveListener(CheckWin);
	}
}
