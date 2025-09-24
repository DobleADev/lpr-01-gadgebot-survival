using System.Collections.Generic;
using UnityEngine;

namespace DoubleADev.ScriptableEvent
{
    [CreateAssetMenu(fileName = "NewUnityEvent", menuName = "Scriptable Object/Scriptable Unity Event")]
    internal class ScriptableUnityEvent : ScriptableObject
    {
        private List<ScriptableUnityEventListener> listeners = new List<ScriptableUnityEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(ScriptableUnityEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(ScriptableUnityEventListener listener)
    {
        listeners.Remove(listener);
    }
    }

    internal class ScriptableEvent<T> : ScriptableObject
    {
        private List<ScriptableEventListener<T>> listeners = new List<ScriptableEventListener<T>>();

    public void Raise(T action)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(action);
        }
    }

    public void RegisterListener(ScriptableEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(ScriptableEventListener<T> listener)
    {
        listeners.Remove(listener);
    }
    }
}
