using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = " Pass Parameter Game Event", menuName = "Event/Pass Parameter Event")]
public class SO_ParameterGameEvent : ScriptableObject
{
    [SerializeField] private bool debug;

    [field : SerializeField]public ScriptableObject ParameterType { get; private set; }
    [SerializeField]private List<PassParameterEventListener> _gameEvent = new List<PassParameterEventListener>();

    [Button("Raise Event")]
    public void Raise(GameObject parameter)
    {
        for (int i = _gameEvent.Count - 1; i >= 0; i--)
        {
            _gameEvent[i].OnEventRaised(parameter);
        }
    }
    [Button("Raise Event")]
    public void Raise(ScriptableObject parameter)
    {
        if (debug)
        {
            Debug.Log("DEBUG IS ON");
            for (int i = _gameEvent.Count - 1; i >= 0; i--)
            {
                _gameEvent[i].OnEventRaised(ParameterType);
            }
            return;
        }
        if (parameter == null && !debug) return;
        if (parameter.GetType() == ParameterType.GetType() && !debug)
        {
            for (int i = _gameEvent.Count - 1; i >= 0; i--)
            {
                _gameEvent[i].OnEventRaised(parameter);
            }
            return;
        }
        else if (parameter.GetType().IsSubclassOf(ParameterType.GetType())&& !debug)
        {
            for (int i = _gameEvent.Count - 1; i >= 0; i--)
            {
                _gameEvent[i].OnEventRaised(parameter);
            }
            return;
        }
    }
    public void RegisterListener(PassParameterEventListener listener)
    {
        _gameEvent.Add(listener);
    }
    public void UnregisterListener(PassParameterEventListener listener)
    {
        _gameEvent.Remove(listener);
    }
}
