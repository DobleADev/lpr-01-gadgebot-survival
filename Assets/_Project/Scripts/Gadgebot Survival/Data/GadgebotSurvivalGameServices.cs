using UnityEngine;

[CreateAssetMenu(fileName = "Gadgebot Survival Game Services", menuName = "Scriptable Object/Gadgebot Survival Game Services")]
public class GadgebotSurvivalGameServices : ScriptableObject
{
    public float gameSpeed { get; private set; } = 1;
    public void SetGameSpeed(float speed) { gameSpeed = speed; }
    public Camera mainCamera { get; private set; }
    public void SetMainCamera(Camera camera) { mainCamera = camera; }
}
