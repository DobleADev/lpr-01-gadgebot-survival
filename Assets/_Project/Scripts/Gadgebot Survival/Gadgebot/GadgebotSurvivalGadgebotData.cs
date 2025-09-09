using UnityEngine;

// namespace DobleADev.GadgebotSurvival.Gadgebot
// {
	[CreateAssetMenu(fileName = "NewGadgebotProperties", menuName = "Scriptable Object/Gadgebot Properties")]
	public class GadgebotSurvivalGadgebotData : ScriptableObject
	{
		[Header("Stats")]
		public float speed = 1;
		public float gravityScale = 1;
		public float maxFallSpeed = 12;
		public float fallSpeedDeathThreshold = 8;
		public float walkDelayAfterGround = 0.3f;
		public LayerMask groundLayer = 1;
		[Header("Commands")]
		public GadgebotFollowCommand followCommand = new GadgebotFollowCommand();
		public GadgebotSwingCommand swingCommand = new GadgebotSwingCommand();
		public GadgebotElectrifyCommand electrifyCommand = new GadgebotElectrifyCommand();
		public GadgebotBridgeCommand bridgeCommand = new GadgebotBridgeCommand();
		public GadgebotDetonateCommand detonateCommand = new GadgebotDetonateCommand();
	}
// }
