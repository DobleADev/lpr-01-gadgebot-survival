using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgebot : MonoBehaviour 
{
	public float speed = 1;
	public float gravityScale = 1;
	public float maxFallSpeed = 2;
	public Rigidbody2D physics;
	public GadgebotCommandState currentCommand;
	Vector2 fallVelocity;
	bool isGrounded;

	void Start () 
	{
		
	}

	void FixedUpdate()
	{
		isGrounded = Physics2D.BoxCastNonAlloc(physics.position + new Vector2(0, -0.5f), new Vector2(1, 0.1f), 0, Vector2.down, new RaycastHit2D[1], 0.01f) > 0;
		Vector2 movement;
		
		if (isGrounded)
		{
			movement = Vector2.right * speed;
			fallVelocity = Vector2.zero;
		}
		else
		{
			movement = fallVelocity;
			fallVelocity += Time.deltaTime * Physics2D.gravity * gravityScale;
			fallVelocity = Vector2.ClampMagnitude(fallVelocity, maxFallSpeed);
		}
		physics.Walk(Time.deltaTime * movement, 45, 0.08f, 2, 1);
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