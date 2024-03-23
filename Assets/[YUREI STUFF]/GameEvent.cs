using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName ="Event/Game Event")]
public class GameEvent : ScriptableObject
{
    List<EventListener> gameEvent = new List<EventListener>();
    [Button("Raise Event")]
    public void Raise()
    {
        for(int i = gameEvent.Count - 1; i >= 0; i--)
        {
            gameEvent[i].OnEventRaised();
        }
    }
    public void RegisterListener(EventListener listener)
    {
        gameEvent.Add(listener);
    }
    public void UnregisterListener(EventListener listener)
    {
        gameEvent.Remove(listener);
    }
}
[System.Serializable]
public class KeyGameEvent
{
    public string key;
    public GameEvent gameEvent;

}