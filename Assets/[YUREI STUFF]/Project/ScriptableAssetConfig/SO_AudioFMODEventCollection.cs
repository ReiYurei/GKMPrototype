using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using FMODUnity;
[CreateAssetMenu(fileName = "Audio Event References", menuName = "Event/Audio Event References")]
public class SO_AudioFMODEventCollection : ScriptableObject
{
    [SerializeField] private List<AudioFMODEvent<string>> _audioEvents;
    public Dictionary<string, EventReference> AudioEventsDict { get; private set; }
    public void InitializeAwakeData()
    {
        AudioEventsDict = new Dictionary<string, EventReference>();
    }
    public void InitializeStartData()
    {
        for (int i = 0; i < _audioEvents.Count; i++)
        {
            if (_audioEvents[i] == null) continue;
            AudioEventsDict.Add(_audioEvents[i].Key, _audioEvents[i].AudioReference);
        }
    } 
}
[System.Serializable]
public class AudioFMODEvent<T>
{
    [field: SerializeField] public T Key { get; private set; }
    [field: SerializeField] public EventReference AudioReference { get; private set; }
}