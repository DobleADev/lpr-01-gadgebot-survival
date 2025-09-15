using UnityEngine;

public class GadgebotSurvivalGadgebotCommandTrigger : MonoBehaviour
{
    [SerializeField] GadgebotCommandOption command;
    public void OnInteract(GadgebotSurvivalGadgebotController gadgebot)
    {
        gadgebot.RequestCommandChange(command);
    }
}
