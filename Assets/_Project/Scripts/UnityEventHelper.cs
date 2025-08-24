using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class GameObjectUnityEvent : UnityEvent<GameObject> { }

[Serializable]
public class GadgebotUnityEvent : UnityEvent<Gadgebot> { }
[Serializable]
public class GadgebotCommandOptionUnityEvent : UnityEvent<GadgebotCommandOption> { }

[Serializable]
public class IntUnityEvent : UnityEvent<int> { }