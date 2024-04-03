using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = " Pass Parameter Game Event", menuName = "Event/Pass Parameter Event")]
public class SO_ParameterGameEvent : ScriptableObject
{
    [SerializeField] private bool debug;

    [SerializeField]private ScriptableObject _parameterType;
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
            for (int i = _gameEvent.Count - 1; i >= 0; i--)
            {
                _gameEvent[i].OnEventRaised(_parameterType);
            }
            return;
        }
        if (parameter == null && debug == false) return;
        if (parameter.GetType() != _parameterType.GetType() && !debug) return;
       
        for (int i = _gameEvent.Count - 1; i >= 0; i--)
        {
            _gameEvent[i].OnEventRaised(parameter);
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
