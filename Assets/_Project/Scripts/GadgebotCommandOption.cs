using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCommandOption", menuName = "Scriptable Object/Gadgebot Command Option")]
public class GadgebotCommandOption : ScriptableObject
{
    private enum GadgebotCommands { Swing, Electrify, Bridge, Detonate }
    [SerializeField] private GadgebotCommands _command;
    [SerializeField] private Sprite _sprite;
    public Sprite sprite { get { return _sprite; }}
    public string command
    {
        get
        {
            return _command.ToString();
        }
    }
}
