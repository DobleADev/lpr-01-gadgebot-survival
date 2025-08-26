using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgebot : MonoBehaviour
{
	public Rigidbody2D physics;
	public BoxCollider2D boxCollider;
	public MeshRenderer lightRenderer;
	public GadgebotFollowCommand followCommand = new GadgebotFollowCommand();
	public GadgebotSwingCommand swingCommand = new GadgebotSwingCommand();
	public GadgebotElectrifyCommand electrifyCommand = new GadgebotElectrifyCommand();
	public GadgebotBridgeCommand bridgeCommand = new GadgebotBridgeCommand();
	public GadgebotDetonateCommand detonateCommand = new GadgebotDetonateCommand();
	public GadgebotState currentCommand;
	public float speed = 1;
	public float gravityScale = 1;
	public float maxFallSpeed = 2;
	public Vector2 fallVelocity;
	public bool isGrounded;
	public Collider2D ground;
	public int direction = 1;
	public bool walk = true;
	public bool physicsEnabled = true;
	public LayerMask groundLayer = 1;
	public GadgebotUnityEvent onDestroy;
	MaterialPropertyBlock lightProperty;

	void Awake()
	{
		lightProperty = new MaterialPropertyBlock();
	}

	void Start()
	{
		ChangeCommandState(followCommand);
	}

	void FixedUpdate()
	{
		HandleMovement();
		currentCommand.FixedUpdate(this);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent(out GadgebotGoal goal))
		{
			goal.OnInteract(this);
		}

		if (other.TryGetComponent(out DeathZone deathzone))
		{
			Destroy(gameObject);
		}

		currentCommand.OnTriggerEnter2D(this, other);
	}

	void OnDestroy()
	{
		onDestroy?.Invoke(this);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 1, 1, 0.4f);
		Gizmos.DrawWireCube(physics.position + new Vector2(0, -0.5f), new Vector2(boxCollider.size.x, 0.1f));
		if (currentCommand != null) currentCommand.OnDrawGizmos(this);
	}

	void HandleMovement()
	{
		if (!physicsEnabled)
		{
			// fallVelocity = Vector2.zero;
			return;
		}
		ground = Physics2D.OverlapBox(physics.position + new Vector2(0, -0.5f), new Vector2(boxCollider.size.x, 0.1f), 0, groundLayer);
		isGrounded = ground != null;
		Vector2 movement = Vector2.zero;

		if (isGrounded)
		{
			if (walk) movement = direction * Vector2.right * speed;
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

	void ChangeLightColor(Color color)
	{
		lightProperty.SetColor("_Color", color);
		lightRenderer.SetPropertyBlock(lightProperty);
	}

	bool IsLike(string a, string b) { return a.Equals(b, StringComparison.OrdinalIgnoreCase); }

	void ChangeCommandState(GadgebotState commandState)
	{
		if (commandState == null || currentCommand == commandState) return;
		if (currentCommand != null)
		{
			if (!currentCommand.CanChangeCommand(this)) return;
			currentCommand.OnCommandExit(this);
		}
		(currentCommand = commandState).OnCommandEnter(this);
		ChangeLightColor(currentCommand.lightColor);
	}

	public void TryDestroyGameObject(GameObject gameObject)
	{
		if (gameObject == null) return;
		Destroy(gameObject);
	}

	public void TryDestroyGameObject(Component component)
	{
		if (component == null) return;
		Destroy(component.gameObject);
	}

	public GameObject TryInstantiate(GameObject gameObject, Transform parent = null)
	{
		if (gameObject == null) return null;
		return Instantiate(gameObject, parent);
	}

	public GameObject TryInstantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
	{
		if (gameObject == null) return null;
		return Instantiate(gameObject, position, rotation);
	}

	public Component TryInstantiate(Component component, Transform parent = null)
	{
		if (component == null) return null;
		return Instantiate(component, parent);
	}


	public Component TryInstantiate(Component component, Vector3 position, Quaternion rotation)
	{
		if (component == null) return null;
		return Instantiate(component, position, rotation);
	}

	public void RequestCommandChange(GadgebotCommandOption commandOption)
	{
		switch (commandOption.command)
		{
			case string str when IsLike(str, "swing"): ChangeCommandState(swingCommand); break;
			case string str when IsLike(str, "electrify"): ChangeCommandState(electrifyCommand); break;
			case string str when IsLike(str, "bridge"): ChangeCommandState(bridgeCommand); break;
			case string str when IsLike(str, "detonate"): ChangeCommandState(detonateCommand); break;
			default: ChangeCommandState(followCommand); break;
		}
	}
}

[System.Serializable]
public class GadgebotFollowCommand : GadgebotState
{
	// public override void OnCommandEnter(Gadgebot gadgebot) {}
	// public override void OnCommandExit(Gadgebot gadgebot) {}
	// public override void OnTriggerEnter2D(Gadgebot gadgebot, Collider2D other) {}
	// public override void OnDrawGizmos(Gadgebot gadgebot) {}
	// public override bool CanChangeCommand(Gadgebot gadgebot) { return true; }
}
[System.Serializable]
public class GadgebotSwingCommand : GadgebotState
{
	// public override void OnCommandEnter(Gadgebot gadgebot) {}
	// public override void OnCommandExit(Gadgebot gadgebot) {}
	// public override void FixedUpdate(Gadgebot gadgebot) {}
	// public override void OnTriggerEnter2D(Gadgebot gadgebot, Collider2D other) {}
	// public override void OnDrawGizmos(Gadgebot gadgebot) {}
	// public override bool CanChangeCommand(Gadgebot gadgebot) { return true; }
}
[System.Serializable]
public class GadgebotElectrifyCommand : GadgebotState
{
	// public override void OnCommandEnter(Gadgebot gadgebot) {}
	// public override void OnCommandExit(Gadgebot gadgebot) {}
	// public override void FixedUpdate(Gadgebot gadgebot) {}
	// public override void OnTriggerEnter2D(Gadgebot gadgebot, Collider2D other) {}
	// public override void OnDrawGizmos(Gadgebot gadgebot) {}
	// public override bool CanChangeCommand(Gadgebot gadgebot) { return true; }
}
[System.Serializable]
public class GadgebotBridgeCommand : GadgebotState
{
	// public override void OnCommandEnter(Gadgebot gadgebot) {}
	// public override void OnCommandExit(Gadgebot gadgebot) {}
	public GameObject bridgeBlockPrefab;
	public GameObject bridgeBlock;
	public float jumpMaxApex = 1;
	public float jumpDuration = 1;
	public bool triggered;
	public override void FixedUpdate(Gadgebot gadgebot)
	{
		if (triggered) return;

		if (gadgebot.isGrounded && !Physics2D.OverlapPoint((Vector2)gadgebot.transform.position + new Vector2(0, -0.6f), gadgebot.groundLayer))
		{
			triggered = true;
			gadgebot.StartCoroutine(GoBridge(gadgebot));
		}
	}

	IEnumerator GoBridge(Gadgebot gadgebot)
	{
		gadgebot.physicsEnabled = false;
		Collider2D lastGround = gadgebot.ground;
		Transform gadgebotTransform = gadgebot.transform;
		Vector2 initialPosition = gadgebotTransform.position;
		Vector2 endPosition = (Vector2)lastGround.transform.position + new Vector2(gadgebot.direction * 1f, 0);
		float t = 0;
		float deltaDuration = 1 / jumpDuration;
		while (t < 1)
		{
			float jump = Mathf.Lerp(0, jumpMaxApex, Mathf.Sin(t * Mathf.PI));
			Vector3 traslation = Vector3.Lerp(initialPosition, endPosition, t);
			gadgebotTransform.position = traslation + jump * Vector3.up;
			t += deltaDuration * Time.deltaTime;
			yield return null;
		}
		gadgebotTransform.position = endPosition;
		// gadgebot.boxCollider.enabled = false;
		bridgeBlock = gadgebot.TryInstantiate(bridgeBlockPrefab, endPosition, Quaternion.identity);
		gadgebot.TryDestroyGameObject(gadgebot);
	}

	public override void OnDrawGizmos(Gadgebot gadgebot)
	{
		if (triggered || !gadgebot.isGrounded) return;
		Gizmos.color = lightColor;
		Gizmos.DrawWireCube((Vector2)gadgebot.transform.position + new Vector2(0, -0.6f), 0.1f * Vector2.one);
	}
	public override bool CanChangeCommand(Gadgebot gadgebot) { return !triggered; }
}
[System.Serializable]
public class GadgebotDetonateCommand : GadgebotState
{
	public float detonationBuildUpSeconds = 3;
	private Coroutine detonateCoroutine;

	IEnumerator ExplosionProcess(Gadgebot gadgebot)
	{
		yield return new WaitForSeconds(detonationBuildUpSeconds);
		Detonate(gadgebot);
	}

	void Detonate(Gadgebot gadgebot)
	{
		Vector2 gadgebotPosition = gadgebot.transform.position;
		gadgebot.TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(0, -0.6f), gadgebot.groundLayer)); // ground
		gadgebot.TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(-0.5f, 0), 1 << gadgebot.gameObject.layer)); // left gadgebot
		gadgebot.TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(0.5f, 0), 1 << gadgebot.gameObject.layer)); // right gadgebot
		gadgebot.TryDestroyGameObject(gadgebot);
	}

	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		gadgebot.walk = false;
		detonateCoroutine = gadgebot.StartCoroutine(ExplosionProcess(gadgebot));
	}

	public override void OnCommandExit(Gadgebot gadgebot)
	{
		if (detonateCoroutine != null) gadgebot.StopCoroutine(detonateCoroutine);
		gadgebot.walk = true;
	}

	public override void OnDrawGizmos(Gadgebot gadgebot)
	{
		Vector2 gadgebotPosition = gadgebot.transform.position;
		Gizmos.color = lightColor;
		Gizmos.DrawWireCube(gadgebotPosition + new Vector2(0, -0.6f), 0.1f * Vector2.one);
		Gizmos.DrawWireCube(gadgebotPosition + new Vector2(-0.5f, 0), 0.1f * Vector2.one);
		Gizmos.DrawWireCube(gadgebotPosition + new Vector2(0.5f, 0), 0.1f * Vector2.one);
	}
}
public abstract class GadgebotState
{
	[SerializeField] Color _lightColor = new Color(1, 1, 1, 1);
	public Color lightColor { get { return _lightColor; } }
	public virtual void OnCommandEnter(Gadgebot gadgebot) { }
	public virtual void OnCommandExit(Gadgebot gadgebot) { }
	public virtual void FixedUpdate(Gadgebot gadgebot) { }
	public virtual void OnTriggerEnter2D(Gadgebot gadgebot, Collider2D other) { }
	public virtual void OnDrawGizmos(Gadgebot gadgebot) { }
	public virtual bool CanChangeCommand(Gadgebot gadgebot) { return true; }
}