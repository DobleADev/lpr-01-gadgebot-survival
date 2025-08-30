using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGadgebotCommandOption", menuName = "Scriptable Object/Gadgebot Command Option")]
public class GadgebotCommandOption : ScriptableObject
{
    public enum GadgebotCommands { Swing, Electrify, Bridge, Detonate }
    [SerializeField] private GadgebotCommands _command;
    [SerializeField] private Sprite _sprite;
    public Sprite sprite { get { return _sprite; }}
    public GadgebotCommands command
    {
        get
        {
            return _command;
        }
    }
}
