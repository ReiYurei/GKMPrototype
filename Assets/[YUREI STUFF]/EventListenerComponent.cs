using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

    [System.Serializable]
 public class EventListener
 {
    [ValidateInput(nameof(ValidateVariable))]
    public GameEvent gameEvent;
    public UnityEvent response;
    public void OnEventRaised()
    {
       response.Invoke();       
    }
    TriValidationResult ValidateVariable()
    {
        if (gameEvent == null) return TriValidationResult.Info("GameEvent is currently null");
        return TriValidationResult.Valid;
    }

}

public class EventListenerComponent : MonoBehaviour
{
    public List<EventListener> listeners;

    private void OnEnable()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            if (listeners[i].gameEvent == null) continue;
            listeners[i].gameEvent.RegisterListener(listeners[i]);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            if (listeners[i].gameEvent == null) continue;
            listeners[i].gameEvent.UnregisterListener(listeners[i]);
        }
    }
}
