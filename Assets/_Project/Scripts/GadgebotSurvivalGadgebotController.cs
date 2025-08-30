using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgebotSurvivalGadgebotController : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGadgebotData _properties;
	public GadgebotSurvivalGadgebotData properties { get { return _properties; } }
	[SerializeField] Rigidbody2D physics;
	[SerializeField] BoxCollider2D boxCollider;
	[SerializeField] MeshRenderer lightRenderer;
	// public GadgebotFollowCommand followCommand = new GadgebotFollowCommand();
	// public GadgebotSwingCommand swingCommand = new GadgebotSwingCommand();
	// public GadgebotElectrifyCommand electrifyCommand = new GadgebotElectrifyCommand();
	// public GadgebotBridgeCommand bridgeCommand = new GadgebotBridgeCommand();
	// public GadgebotDetonateCommand detonateCommand = new GadgebotDetonateCommand();
	GadgebotState currentCommand;
	// public float speed = 1;
	// public float gravityScale = 1;
	// public float maxFallSpeed = 12;
	// public float fallSpeedDeathThreshold = 8;
	public Vector2 fallVelocity { get; private set;  }
	public bool isGrounded { get; private set;  }
	public Collider2D ground { get; private set;  }
	public bool onTeletransportation { get; set; }
	public bool bridgeActivated { get; private set; }
	// public float walkDelayAfterGround = 0.3f;
	public int direction = 1;
	public bool walk = true;
	public bool physicsEnabled = true;
	public bool canSelect = true;
	// public LayerMask groundLayer = 1;
	public GadgebotUnityEvent onDestroy;
	MaterialPropertyBlock lightProperty;
	public Coroutine detonateCoroutine { get; private set; }
	float timeAfterGrounded;

	void Awake()
	{
		lightProperty = new MaterialPropertyBlock();
		timeAfterGrounded = properties.walkDelayAfterGround;
	}

	void Start()
	{
		ChangeCommandState(properties.followCommand);
	}

	void FixedUpdate()
	{
		currentCommand.FixedUpdate(this);
		HandleMovement();
		
	}

	void OnPhysicsCollision2D(Collider2D collider)
	{
		if (collider.TryGetComponent(out GadgebotSurvivalDirectionBlock directionBlock))
		{
			direction = directionBlock.direction;
		}

		if (collider.TryGetComponent(out GadgebotSurvivalGadgebotController gadgebot))
		{
			if (Vector2.Angle((gadgebot.transform.position - transform.position).normalized, Vector2.down) < 30)
			{
				fallVelocity = Vector2.zero;
				Destroy(gadgebot.gameObject);
			}
		}
    }

    void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent(out GadgebotSurvivalGoal goal))
		{
			goal.OnInteract(this);
		}

		if (other.TryGetComponent(out GadgebotSurvivalDeathZone GadgebotSurvivalDeathZone))
		{
			Destroy(gameObject);
		}

		currentCommand.OnTriggerEnter2D(this, other);
	}

	void OnTriggerStay2D(Collider2D other)
	{
		currentCommand.OnTriggerStay2D(this, other);
	}

	void OnDestroy()
	{
		onDestroy?.Invoke(this);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 1, 1, 0.4f);
		Gizmos.DrawWireCube(physics.position + new Vector2(0, -0.48f), new Vector2(boxCollider.size.x, 0.01f));
		if (currentCommand != null) currentCommand.OnDrawGizmos(this);
	}

	void HandleMovement()
	{
		if (!physicsEnabled)
		{
			// fallVelocity = Vector2.zero;
			return;
		}
		// ground = Physics2D.OverlapBox(physics.position + new Vector2(0, -0.45f), new Vector2(boxCollider.size.x, 0.1f), 0, groundLayer);
		// isGrounded = ground != null;
		Vector2 movement = Vector2.zero;
		
		Collider2D groundCollider = Physics2D.OverlapBox(physics.position + new Vector2(0, -0.48f), new Vector2(boxCollider.size.x, 0.01f), 0);
		if (isGrounded = groundCollider != null)
		{
			ground = (1 << groundCollider.gameObject.layer) == properties.groundLayer ? groundCollider : null;
		}

		if (isGrounded)
		{
			// if (ground.TryGetComponent(out GadgebotSurvivalDirectionBlock directionBlock))
			// {
			// 	direction = directionBlock.direction;
			// }
			if (timeAfterGrounded >= properties.walkDelayAfterGround)
			{
				if (walk) movement = direction * Vector2.right * properties.speed;
			}
			else timeAfterGrounded += Time.deltaTime;

			if (fallVelocity.y < -properties.fallSpeedDeathThreshold) Destroy(gameObject);
			fallVelocity = Vector2.zero;
		}
		else
		{
			timeAfterGrounded = 0;
			movement = fallVelocity;
			fallVelocity += Time.deltaTime * Physics2D.gravity * properties.gravityScale;
			fallVelocity = Vector2.ClampMagnitude(fallVelocity, properties.maxFallSpeed);
		}

		// physics.Slide(Time.deltaTime * movement, 0.08f, 2);
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

	public IEnumerator GoBridge(GadgebotBridgeCommand bridgeCommand)
	{
		bridgeActivated = true;
		physicsEnabled = false;
		canSelect = false;
		Collider2D lastGround = ground;
		Transform gadgebotTransform = transform;
		Vector2 initialPosition = gadgebotTransform.position;
		Vector2 endPosition = (Vector2)lastGround.transform.position + new Vector2(direction * 1f, 0);
		float t = 0;
		float deltaDuration = 1 / bridgeCommand.jumpDuration;
		while (t < 1)
		{
			float jump = Mathf.Lerp(0, bridgeCommand.jumpMaxApex, Mathf.Sin(t * Mathf.PI));
			Vector3 traslation = Vector3.Lerp(initialPosition, endPosition, t);
			gadgebotTransform.position = traslation + jump * Vector3.up;
			t += deltaDuration * Time.deltaTime;
			yield return null;
		}
		gadgebotTransform.position = endPosition;
		var block = Instantiate(bridgeCommand.bridgeBlockPrefab, endPosition, Quaternion.identity);
		UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(block, gameObject.scene);
		Destroy(gameObject);
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

	public void RequestDefaultCommand()
	{
		ChangeCommandState(properties.followCommand);
	}

	public void StartDetonation(GadgebotDetonateCommand detonateCommand)
	{
		detonateCoroutine = StartCoroutine(ExplosionProcess(detonateCommand));
	}

	IEnumerator ExplosionProcess(GadgebotDetonateCommand detonateCommand)
	{
		yield return new WaitForSeconds(detonateCommand.detonationBuildUpSeconds);
		Detonate();
	}

	void Detonate()
	{
		Vector2 gadgebotPosition = transform.position;
		// gadgebot.TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(0, -0.48f), gadgebot.groundLayer)); // ground
		TryDestroyGameObject(ground); // ground
		TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(-0.5f, 0), 1 << gameObject.layer)); // left gadgebot
		TryDestroyGameObject(Physics2D.OverlapPoint(gadgebotPosition + new Vector2(0.5f, 0), 1 << gameObject.layer)); // right gadgebot
		TryDestroyGameObject(this);
	}

	public void RequestCommandChange(GadgebotCommandOption commandOption)
	{
		switch (commandOption.command)
		{
			case GadgebotCommandOption.GadgebotCommands.Swing: ChangeCommandState(properties.swingCommand); break;
			case GadgebotCommandOption.GadgebotCommands.Electrify: ChangeCommandState(properties.electrifyCommand); break;
			case GadgebotCommandOption.GadgebotCommands.Bridge: ChangeCommandState(properties.bridgeCommand); break;
			case GadgebotCommandOption.GadgebotCommands.Detonate: ChangeCommandState(properties.detonateCommand); break;
			// case string str when IsLike(str, "swing"): ChangeCommandState(properties.swingCommand); break;
			// case string str when IsLike(str, "electrify"): ChangeCommandState(properties.electrifyCommand); break;
			// case string str when IsLike(str, "bridge"): ChangeCommandState(properties.bridgeCommand); break;
			// case string str when IsLike(str, "detonate"): ChangeCommandState(properties.detonateCommand); break;
			default: ChangeCommandState(properties.followCommand); break;
		}
	}
}

[System.Serializable]
public class GadgebotFollowCommand : GadgebotState
{
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) {}
	// public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return true; }
}
[System.Serializable]
public class GadgebotSwingCommand : GadgebotState
{
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) {}
	// public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return true; }
}
[System.Serializable]
public class GadgebotElectrifyCommand : GadgebotState
{
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot) {}
	// public bool onTeletransportation { get; set; }
	public override void OnTriggerStay2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		if (other.TryGetComponent(out GadgebotSurvivalTeletransporter teletransporter))
		{
			// RequestTeleport(gadgebot, teletransporter);
			teletransporter.RequestTeleport(gadgebot);
		}
	}
	
	// void RequestTeleport(GadgebotSurvivalGadgebotController gadgebot, GadgebotSurvivalTeletransporter teletransporter)
    // {
    //     if (teletransporter.onTeletransportation) return;
    //     gadgebot.StartCoroutine(TeletransportationProcess(gadgebot, teletransporter));
    // }

	// IEnumerator TeletransportationProcess(GadgebotSurvivalGadgebotController gadgebot, GadgebotSurvivalTeletransporter teletransporter)
	// {
	// 	teletransporter.onTeletransportation = true;
	// 	gadgebot.walk = false;
	// 	onTeletransportation = true;
	// 	yield return new WaitForSeconds(teletransporter.teletransportationDuration);
	// 	gadgebot.transform.position = teletransporter.endPoint.TransformPoint(teletransporter.endPointOffset);
	// 	teletransporter.onTeletransportation = false;
	// 	gadgebot.walk = true;
	// 	onTeletransportation = false;
	// 	gadgebot.RequestDefaultCommand();
    // }
	// public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) {}
	public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return !gadgebot.onTeletransportation; }
}
[System.Serializable]
public class GadgebotBridgeCommand : GadgebotState
{
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) {}
	public GameObject bridgeBlockPrefab;
	public float jumpMaxApex = 1;
	public float jumpDuration = 1;
	
	public override void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot)
	{
		if (gadgebot.bridgeActivated) return;

		if (gadgebot.isGrounded && gadgebot.ground && gadgebot.fallVelocity.y == 0 && !Physics2D.OverlapPoint((Vector2)gadgebot.transform.position + new Vector2(0, -0.6f), gadgebot.properties.groundLayer))
		{
			gadgebot.StartCoroutine(gadgebot.GoBridge(this));
		}
	}

	public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot)
	{
		if (gadgebot.bridgeActivated || !gadgebot.isGrounded) return;
		Gizmos.color = lightColor;
		Gizmos.DrawWireCube((Vector2)gadgebot.transform.position + new Vector2(0, -0.6f), 0.1f * Vector2.one);
	}
	public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return !gadgebot.bridgeActivated; }
}
[System.Serializable]
public class GadgebotDetonateCommand : GadgebotState
{
	public float detonationBuildUpSeconds = 3;

	public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.walk = false;
		gadgebot.StartDetonation(this);
	}

	public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot)
	{
		if (gadgebot.detonateCoroutine != null) gadgebot.StopCoroutine(gadgebot.detonateCoroutine);
		gadgebot.walk = true;
	}

	public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot)
	{
		Vector2 gadgebotPosition = gadgebot.transform.position;
		Gizmos.color = lightColor;
		// Gizmos.DrawWireCube(gadgebotPosition + new Vector2(0, -0.48f), 0.1f * Vector2.one);
		Gizmos.DrawWireCube(gadgebotPosition + new Vector2(-0.5f, 0), 0.1f * Vector2.one);
		Gizmos.DrawWireCube(gadgebotPosition + new Vector2(0.5f, 0), 0.1f * Vector2.one);
	}
}
public abstract class GadgebotState
{
	[SerializeField] Color _lightColor = new Color(1, 1, 1, 1);
	public Color lightColor { get { return _lightColor; } }
	public virtual void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) { }
	public virtual void OnTriggerStay2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) { }
	public virtual void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return true; }
}