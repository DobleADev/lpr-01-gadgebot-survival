using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace DobleADev.GadgebotSurvival.Gadgebot
// {
public class GadgebotSurvivalGadgebotController : MonoBehaviour
{
	[SerializeField] GadgebotSurvivalGadgebotData _properties;
	public GadgebotSurvivalGadgebotData properties { get { return _properties; } }
	[SerializeField] Rigidbody2D physics;
	// [SerializeField] BoxCollider2D boxCollider;
	[SerializeField] MeshRenderer lightRenderer;
	float timeAfterGrounded;
	GadgebotState currentCommand;
	MaterialPropertyBlock lightProperty;
	public Vector2 fallVelocity { get; private set; }
	public bool isGrounded { get; private set; }
	public Collider2D ground { get; private set; }
	public bool teletransporting { get; private set; }
	public GadgebotSurvivalSwinger swingToWait { get; private set; }
	public GadgebotSurvivalTeletransporter teletransporterToWait { get; private set; }
	public bool swinging { get; private set; }
	public bool bridgeActivated { get; private set; }
	// public float walkDelayAfterGround = 0.3f;
	public int direction { get; private set; } = 1;
	public bool walk { get; private set; } = true;
	public bool physicsEnabled { get; private set; } = true;
	public bool canSelect { get; private set; } = true;
	// public LayerMask groundLayer = 1;
	[SerializeField] GadgebotUnityEvent _onDestroy;
	public GadgebotUnityEvent onDestroy { get { return _onDestroy; } set { _onDestroy = value; } }
	public Coroutine detonateCoroutine { get; private set; }

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
		// if (collider.TryGetComponent(out GadgebotSurvivalDirectionBlock directionBlock))
		// {
		// 	direction = directionBlock.direction;
		// }

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

		if (other.TryGetComponent(out GadgebotSurvivalGadgebotCommandTrigger gadgebotCommandTrigger))
		{
			gadgebotCommandTrigger.OnInteract(this);
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
		Gizmos.DrawWireCube(physics.position + properties.groundCheckOrigin, properties.groundCheckSize);
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

		Collider2D[] groundCollider = new Collider2D[1];
		ContactFilter2D groundFilter = new ContactFilter2D();
		groundFilter.useTriggers = false;
		isGrounded = Physics2D.OverlapBox(physics.position + properties.groundCheckOrigin, properties.groundCheckSize, 0, groundFilter, groundCollider) > 0;
		if (isGrounded)
		{
			ground = (1 << groundCollider[0].gameObject.layer) == properties.groundLayer ? groundCollider[0] : null;

			if (groundCollider[0].TryGetComponent(out GadgebotSurvivalDirectionBlock directionBlock))
			{
				direction = directionBlock.direction;
			}
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
		if (lightRenderer == null) return;
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

	// public void RequestDefaultCommand()
	// {
	// 	ChangeCommandState(properties.followCommand);
	// }

	public void StartDetonation(GadgebotDetonateCommand detonateCommand)
	{
		detonateCoroutine = StartCoroutine(ExplosionProcess(detonateCommand));
		walk = false;
	}

	public void CancelDetonation()
	{
		if (detonateCoroutine != null) StopCoroutine(detonateCoroutine);
		walk = true;
	}

	public void StartWaitingForSwinger(GadgebotSurvivalSwinger swinger)
	{
		swingToWait = swinger;
		walk = false;
	}

	public void CancelWaitingForSwinger()
	{
		swingToWait = null;
		walk = true;
	}

	public void StartWaitingForTeletransporter(GadgebotSurvivalTeletransporter teletransporter)
	{
		teletransporterToWait = teletransporter;
		walk = false;
	}

	public void CancelWaitingForTeletransporter()
	{
		teletransporterToWait = null;
		walk = true;
	}

	// public IEnumerator SwingProcess(GadgebotSurvivalSwinger swinger)
	// {
	// 	CancelWaitingForSwinger();
	// 	physicsEnabled = false;
	// 	swinging = true;
	// 	while (swinger.onSwinging)
	// 	{
	// 		// Debug.Log("Waiting for teleport end.. " + Time.time);
	// 		yield return null;
	// 	}
	// 	physicsEnabled = true;
	// 	swinging = false;
	// 	ChangeCommandState(properties.followCommand);
	// }

	public void StartSwing()
	{
		CancelWaitingForSwinger();
		physicsEnabled = false;
		swinging = true;
	}

	public void EndSwing()
	{
		physicsEnabled = true;
		swinging = false;
		ChangeCommandState(properties.followCommand);
	}

	// public IEnumerator TeletransportationProcess(GadgebotSurvivalTeletransporter teletransporter)
	// {
	//  CancelWaitingForTeletransporter();
	// 	// Debug.Log("Local Wait Started " + Time.time);
	// 	walk = false;
	// 	teletransporting = true;

	// 	// yield return teletransporter.process;
	// 	while (teletransporter.onTeletransportation)
	// 	{
	// 		// Debug.Log("Waiting for teleport end.. " + Time.time);
	// 		yield return null;
	// 	}
	// 	// Debug.Log("Teleport end " + Time.time);
	// 	walk = true;
	// 	teletransporting = false;
	// 	ChangeCommandState(properties.followCommand);
	// }

	public void StartTeletransportation()
	{
		CancelWaitingForTeletransporter();
		walk = false;
		teletransporting = true;
	}

	public void EndTeletransportation()
	{
		walk = true;
		teletransporting = false;
		ChangeCommandState(properties.followCommand);
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
	public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.CancelWaitingForSwinger();
	}
	public override void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot)
	{
		if (!gadgebot.swinging)
		{
			// REQUEST SWING
			if (!gadgebot.isGrounded
			|| gadgebot.swingToWait == null
			|| !gadgebot.swingToWait.RequestSwing(gadgebot)) return;
			// gadgebot.swingToWait.StartCoroutine(gadgebot.SwingProcess(gadgebot.swingToWait));
			gadgebot.StartSwing();
		}
	}
	public override void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		RequestSwing(gadgebot, other);
	}

	public override void OnTriggerStay2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		RequestSwing(gadgebot, other);
	}

	void RequestSwing(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		if (!gadgebot.isGrounded || gadgebot.swinging || gadgebot.swingToWait != null) return;
		if (other.TryGetComponent(out GadgebotSurvivalSwinger swinger))
		{
			// WAIT FOR SWINGER
			gadgebot.StartWaitingForSwinger(swinger);
		}
	}

	public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot)
	{
		Gizmos.color = new Color(1, 1, 1, 0);

		if (gadgebot.swingToWait != null)
		{
			Gizmos.color = Color.gray;
		}

		if (gadgebot.swinging)
		{
			Gizmos.color = Color.white;
		}

		Gizmos.DrawWireCube(gadgebot.transform.position + Vector3.up, 0.2f * Vector2.one);
	}

	public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return !gadgebot.swinging; }
}
[System.Serializable]
public class GadgebotElectrifyCommand : GadgebotState
{
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.CancelWaitingForTeletransporter();
	}

	public override void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot)
	{
		if (!gadgebot.isGrounded
			|| gadgebot.teletransporterToWait == null
			|| !gadgebot.teletransporterToWait.RequestTeleport(gadgebot)) return;
		// gadgebot.teletransporterToWait.StartCoroutine(gadgebot.TeletransportationProcess(gadgebot.teletransporterToWait));
		gadgebot.StartTeletransportation();
	}
	public override void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		RequestTeletransport(gadgebot, other);
	}
	public override void OnTriggerStay2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		RequestTeletransport(gadgebot, other);
	}
	void RequestTeletransport(GadgebotSurvivalGadgebotController gadgebot, Collider2D other)
	{
		if (other.TryGetComponent(out GadgebotSurvivalTeletransporter teletransporter))
		{
			// RequestTeleport(gadgebot, teletransporter);
			// if (!teletransporter.RequestTeleport(gadgebot) || !gadgebot.isGrounded) return;
			// teletransporter.StartCoroutine(gadgebot.TeletransportationProcess(teletransporter));
			if (
				!gadgebot.isGrounded
				|| gadgebot.teletransporting
				|| gadgebot.teletransporterToWait != null
				|| !teletransporter.canBeWaitedFor) return;
			gadgebot.StartWaitingForTeletransporter(teletransporter);
		}
	}

	// public override void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) {}
	public override bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return !gadgebot.teletransporting; }
}
[System.Serializable]
public class GadgebotBridgeCommand : GadgebotState
{
	public GameObject bridgeBlockPrefab;
	public float jumpMaxApex = 1;
	public float jumpDuration = 1;
	// public override void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) {}
	// public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) {}

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
		gadgebot.StartDetonation(this);
	}

	public override void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot)
	{
		gadgebot.CancelDetonation();
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
	[SerializeField, ColorUsage(true, true)] Color _lightColor = new Color(1, 1, 1, 1);
	public Color lightColor { get { return _lightColor; } }
	public virtual void OnCommandEnter(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void OnCommandExit(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void FixedUpdate(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual void OnTriggerEnter2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) { }
	public virtual void OnTriggerStay2D(GadgebotSurvivalGadgebotController gadgebot, Collider2D other) { }
	public virtual void OnDrawGizmos(GadgebotSurvivalGadgebotController gadgebot) { }
	public virtual bool CanChangeCommand(GadgebotSurvivalGadgebotController gadgebot) { return true; }
}
// }
