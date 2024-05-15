using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

    [System.Serializable]
 public class VoidEventListener
 {
    [ValidateInput(nameof(ValidateVariable))]
    [Tooltip("Listen to a Game Event")]
    public SO_VoidGameEvent gameEvent; //Listening to this Game Event
    [Tooltip("Response when event raised")]
    public UnityEvent voidResponse; //List of Unity Event
    public void OnEventRaised()
    {
       voidResponse.Invoke();       
    }
    TriValidationResult ValidateVariable()
    {
        if (gameEvent == null) return TriValidationResult.Info("GameEvent is currently null");
        return TriValidationResult.Valid;
    }

}

[System.Serializable]
public class PassParameterEventListener
{
    [System.Serializable]
    public class PassParameterGameObjectEventListener : UnityEvent<GameObject> { }
    [System.Serializable]
    public class PassParameterScriptableObjectEventListener : UnityEvent<ScriptableObject> { }

    [ValidateInput(nameof(ValidateVariable))]
    [Tooltip("Listen to a Game Event")]
    public SO_ParameterGameEvent gameEvent; //Listening to this Game Event
    [Tooltip("Response when event raised")]
    public PassParameterGameObjectEventListener parameterGOResponse;
    public PassParameterScriptableObjectEventListener parameterSOResponse;
    public void OnEventRaised(GameObject parameter)
    {
        parameterGOResponse.Invoke(parameter);
    }
    public void OnEventRaised(ScriptableObject parameter)
    {
        parameterSOResponse.Invoke(parameter);
    }
    TriValidationResult ValidateVariable()
    {
        if (gameEvent == null) return TriValidationResult.Info("GameEvent is currently null");
        if(gameEvent.ParameterType == null) return TriValidationResult.Warning("Event has no Passing Parameter!");
        var type = gameEvent.ParameterType.GetType().ToString();
        return TriValidationResult.Info($"Passed Parameter : {type}");
    }

}
public class EventListenerComponent : MonoBehaviour
{
    [SerializeField]private List<VoidEventListener> _listenersVoid;
    [SerializeField]private List<PassParameterEventListener> _listenersParam;

    public void Test2(GameObject data)
    {
        Debug.Log(data.name);
    }
    public void Test(ScriptableObject data)
    {
        Debug.Log(data.name);
    }
    private void OnEnable()
    {
        for (int i = 0; i < _listenersVoid.Count; i++)
        {
            if (_listenersVoid[i].gameEvent == null) continue;
            _listenersVoid[i].gameEvent.RegisterListener(_listenersVoid[i]);
        }
        for (int i = 0; i < _listenersParam.Count; i++)
        {
            if (_listenersParam[i].gameEvent == null) continue;
            _listenersParam[i].gameEvent.RegisterListener(_listenersParam[i]);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < _listenersVoid.Count; i++)
        {
            if (_listenersVoid[i].gameEvent == null) continue;
            _listenersVoid[i].gameEvent.UnregisterListener(_listenersVoid[i]);
        }
        for (int i = 0; i < _listenersParam.Count; i++)
        {
            if (_listenersParam[i].gameEvent == null) continue;
            _listenersParam[i].gameEvent.UnregisterListener(_listenersParam[i]);
        }
    }
}
