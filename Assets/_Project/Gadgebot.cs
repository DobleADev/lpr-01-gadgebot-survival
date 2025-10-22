using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgebot : MonoBehaviour 
{
	public float speed = 1;
	public Rigidbody2D physics;
	public GadgebotCommandState currentCommand;

	void Start () 
	{
		
	}
	
	void FixedUpdate () 
	{
		physics.velocity = Vector3.right * speed;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent(out GadgebotGoal goal))
		{
			goal.OnInteract(this);
		}
	}

	public void ChangeCommandState(GadgebotCommandState commandState)
	{

	}
}

public abstract class GadgebotCommandState
{
	public abstract void OnCommandStateEnter(Collider other);
	public abstract void OnCommandStateExit(Collider other);
	public abstract void OnTriggerEnter(Collider other);
	public abstract void TrySetCommand(GadgebotCommandState command);
}