using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Void Game Event", menuName ="Event/Void Event")]
public class SO_VoidGameEvent : ScriptableObject
{
    [SerializeField]private List<VoidEventListener> _gameEvent = new List<VoidEventListener>();
    [Button("Raise Event")]
    public void Raise()
    {
        for(int i = _gameEvent.Count - 1; i >= 0; i--)
        {
            _gameEvent[i].OnEventRaised();
        }
    }
    public void RegisterListener(VoidEventListener listener)
    {
        _gameEvent.Add(listener);
    }
    public void UnregisterListener(VoidEventListener listener)
    {
        _gameEvent.Remove(listener);
    }
}
[System.Serializable]
public class VoidGameEventWithKey<T>
{
    [field: SerializeField]public T Key { get; private set; }
    [field: SerializeField]public SO_VoidGameEvent GameEvent {  get; private set; }
    public void SetValue(T key, SO_VoidGameEvent gameEvent)
    {
        Key = key;
        GameEvent = gameEvent;
    }
}
[System.Serializable]
public class ParameterGameEventWithKey<T>
{
    [field: SerializeField] public T Key { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent GameEvent { get; private set; }

    public void SetValue(T key, SO_ParameterGameEvent gameEvent)
    {
        Key = key;
        GameEvent = gameEvent;
    }
}
