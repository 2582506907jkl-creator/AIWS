using UnityEngine;
using UnityEngine.Events;

public class OnDestroyTrigger : MonoBehaviour
{
    public UnityEvent onDestroyEvent;

    private void OnDestroy()
    {
        onDestroyEvent?.Invoke();
    }
}