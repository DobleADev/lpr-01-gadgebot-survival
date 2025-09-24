using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleADev.ScriptableEvent
{
    internal class ScriptableUnityEventListener : MonoBehaviour
    {
        public ScriptableUnityEvent Event;
        [SerializeField, Min(0)] private float executionDelay;
        [SerializeField] private UnityEvent actions;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            StartCoroutine(ExecuteEvent());
        }

        IEnumerator ExecuteEvent()
        {
            yield return new WaitForSecondsRealtime(executionDelay);
            actions.Invoke();
        }
    }

    internal class ScriptableEventListener<T> : MonoBehaviour
    {
        public ScriptableEvent<T> Event;
        [SerializeField, Min(0)] private float executionDelay;
        [SerializeField] private UnityEvent<T> actions;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(T action)
        {
            StartCoroutine(ExecuteEvent(action));
        }

        IEnumerator ExecuteEvent(T action)
        {
            yield return new WaitForSecondsRealtime(executionDelay);
            actions.Invoke(action);
        }
    }
}
