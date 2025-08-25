using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadgebot : MonoBehaviour
{
	public Rigidbody2D physics;
	public MeshRenderer lightRenderer;
	public GadgebotFollowCommand followCommand = new GadgebotFollowCommand();
	public GadgebotSwingCommand swingCommand = new GadgebotSwingCommand();
	public GadgebotElectrifyCommand electrifyCommand = new GadgebotElectrifyCommand();
	public GadgebotBridgeCommand bridgeCommand = new GadgebotBridgeCommand();
	public GadgebotDetonateCommand detonateCommand = new GadgebotDetonateCommand();
	public GadgebotCommandState currentCommand;
	public float speed = 1;
	public float gravityScale = 1;
	public float maxFallSpeed = 2;
	public Vector2 fallVelocity;
	public bool isGrounded;
	public int direction = 1;
	public bool walk = true;
	public Collider2D ground;
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
		RaycastHit2D[] groundHit = new RaycastHit2D[1];
		isGrounded = Physics2D.BoxCastNonAlloc(physics.position + new Vector2(0, -0.5f), new Vector2(0.8f, 0.1f), 0, Vector2.down, groundHit, 0.01f) > 0;
		Vector2 movement = Vector2.zero;

		if (isGrounded)
		{
			ground = groundHit[0].collider;
			if (walk) movement = direction * Vector2.right * speed;
			fallVelocity = Vector2.zero;
		}
		else
		{
			ground = null;
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

		if (other.TryGetComponent(out DeathZone deathzone))
		{
			Destroy(gameObject);
		}

		currentCommand.OnTriggerEnter2D(other);
	}

	void OnDestroy()
	{
		onDestroy?.Invoke(this);
	}

	void ChangeLightColor(Color color)
	{
		lightProperty.SetColor("_Color", color);
		lightRenderer.SetPropertyBlock(lightProperty);
	}

	public void Detonate()
	{
		Destroy(ground.gameObject);
		Destroy(gameObject);
	}

	public void RequestCommandChange(GadgebotCommandOption commandOption)
	{
		switch (commandOption.command)
		{
			case string str when IsLike(str, "swing"): ChangeCommandState(swingCommand); break;
			case string str when IsLike(str, "electrify"): ChangeCommandState(electrifyCommand); break;
			case string str when IsLike(str, "bridge"): ChangeCommandState(bridgeCommand); break;
			case string str when IsLike(str, "detonate"): ChangeCommandState(detonateCommand); break;
			// case "swing": ChangeCommandState(swingCommand);
			// case "electrify": ChangeCommandState(electrifyCommand); break;
			// case "bridge": ChangeCommandState(bridgeCommand); break;
			// case "detonate": ChangeCommandState(detonateCommand); break;
			default: ChangeCommandState(followCommand); break;
		}
	}

	bool IsLike(string a, string b) { return a.Equals(b, StringComparison.OrdinalIgnoreCase); }

	public void ChangeCommandState(GadgebotCommandState commandState)
	{
		if (commandState == null || currentCommand == commandState) return;
		if (currentCommand != null)
		{
			if (!currentCommand.CanChangeCommand()) return;
			currentCommand.OnCommandExit();
		}
		(currentCommand = commandState).OnCommandEnter(this);
		ChangeLightColor(currentCommand.lightColor);
	}
}

[System.Serializable]
public class GadgebotFollowCommand : GadgebotCommandState
{
	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		this.gadgebot = gadgebot;
	}

	public override void OnCommandExit()
	{

	}

	public override void OnTriggerEnter2D(Collider2D other)
	{

	}

	public override bool CanChangeCommand()
	{
		return true;
	}
}
[System.Serializable]
public class GadgebotSwingCommand : GadgebotCommandState
{
	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		this.gadgebot = gadgebot;
	}

	public override void OnCommandExit()
	{

	}

	public override void OnTriggerEnter2D(Collider2D other)
	{

	}

	public override bool CanChangeCommand()
	{
		return true;
	}
}
[System.Serializable]
public class GadgebotElectrifyCommand : GadgebotCommandState
{
	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		this.gadgebot = gadgebot;
	}

	public override void OnCommandExit()
	{

	}

	public override void OnTriggerEnter2D(Collider2D other)
	{

	}

	public override bool CanChangeCommand()
	{
		return true;
	}
}
[System.Serializable]
public class GadgebotBridgeCommand : GadgebotCommandState
{
	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		this.gadgebot = gadgebot;
	}

	public override void OnCommandExit()
	{

	}

	public override void OnTriggerEnter2D(Collider2D other)
	{

	}

	public override bool CanChangeCommand()
	{
		return true;
	}
}
[System.Serializable]
public class GadgebotDetonateCommand : GadgebotCommandState
{
	public float detonationBuildUpSeconds = 3;
	private Coroutine detonateCoroutine;

	IEnumerator ExplosionProcess()
	{
		yield return new WaitForSeconds(detonationBuildUpSeconds);
		gadgebot.Detonate();
	}

	public override void OnCommandEnter(Gadgebot gadgebot)
	{
		this.gadgebot = gadgebot;
		gadgebot.walk = false;
		detonateCoroutine = gadgebot.StartCoroutine(ExplosionProcess());
	}

	public override void OnCommandExit()
	{
		if (detonateCoroutine != null) gadgebot.StopCoroutine(detonateCoroutine);
		gadgebot.walk = true;
	}

	public override void OnTriggerEnter2D(Collider2D other)
	{

	}

	public override bool CanChangeCommand()
	{
		return true;
	}
}
public abstract class GadgebotCommandState
{
	[SerializeField] Color _lightColor = new Color(1,1,1,1);
	public Color lightColor { get { return _lightColor; }}
	protected Gadgebot gadgebot;
	public abstract void OnCommandEnter(Gadgebot gadgebot);
	public abstract void OnCommandExit();
	public abstract void OnTriggerEnter2D(Collider2D other);
	public abstract bool CanChangeCommand();
}