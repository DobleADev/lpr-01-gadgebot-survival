using UnityEngine;
using UnityEngine.Events;

public class OnKeyEnter : MonoBehaviour
{
    [SerializeField] KeyCode _key;
    [SerializeField] UnityEvent _onKeyEnter;

    void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            _onKeyEnter?.Invoke();
        }
    }
}
