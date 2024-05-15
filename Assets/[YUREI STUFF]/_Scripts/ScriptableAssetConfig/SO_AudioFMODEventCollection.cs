using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
[CreateAssetMenu(fileName = "Audio Event References", menuName = "Miscellaneous/Audio Event References")]
public class SO_AudioFMODEventCollection : ScriptableObject
{
    [SerializeField] private List<AudioFMODEvent<string>> _audioEvents;
    public Dictionary<string, EventReference> AudioEventsDict { get; private set; }
    private List<EventInstance> _eventInstance;
    private void OnEnable()
    {
        AudioEventsDict ??= new Dictionary<string, EventReference>();
    }
    public void InitializeStartData()
    {
        AudioEventsDict ??= new Dictionary<string, EventReference>();
        for (int i = 0; i < _audioEvents.Count; i++)
        {
            if (_audioEvents[i] == null) continue;
            if (!AudioEventsDict.ContainsKey(_audioEvents[i].Key)) AudioEventsDict.Add(_audioEvents[i].Key, _audioEvents[i].AudioReference);
        }
    }
    [Button("Debug : Play")]
    public void Play(string key)
    {
        _eventInstance ??= new List<EventInstance>();
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        foreach (var _instance in _eventInstance)
        {
            _instance.getDescription(out EventDescription desc);
            desc.getID(out FMOD.GUID id);
            if (eventReference.Guid == id) return;
        }
        _eventInstance.Add(instance);
        instance.start();
    }
    public void PlayRepeat(string key)
    {
        _eventInstance ??= new List<EventInstance>();
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        _eventInstance.Add(instance);
        instance.start();
    }
    public void StopInstance(string key)
    {
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;
        if (_eventInstance == null || _eventInstance.Count == 0) return;
        foreach (var instance in _eventInstance)
        {
            instance.getDescription(out EventDescription des);
            des.getID(out FMOD.GUID id);

            if (eventReference.Guid == id)
            {
                int index = _eventInstance.IndexOf(instance);
                AudioManager.Instance.StartCoroutine(EventParamter(index, "Volume", 0, 1, 2.5f));
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
                return;
            }

        }
    }
    public void StopInstance(string key, string parameterName, float startValue, float endValue, float timeToCalculate)
    {
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;
        if (_eventInstance == null || _eventInstance.Count == 0) return;
        foreach (var instance in _eventInstance)
        {
            instance.getDescription(out EventDescription des);
            des.getID(out FMOD.GUID id);

            if (eventReference.Guid == id)
            {
                int index = _eventInstance.IndexOf(instance);
                AudioManager.Instance.StartCoroutine(EventParamter(index, parameterName, startValue, endValue, timeToCalculate));
                return;
            }

        }
    }
    public void SetEventParameter(string key, string parameterName, float value)
    {
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;
        if (_eventInstance == null || _eventInstance.Count == 0) return;
        foreach (var instance in _eventInstance)
        {
            instance.getDescription(out EventDescription desc);
            desc.getID(out FMOD.GUID id);

            if (eventReference.Guid == id)
            {
                instance.setParameterByName(parameterName, value);
            }

        }
    }
    IEnumerator EventParamter(int index, string parameterName, float startValue, float endValue, float timeToCalculate)
    {
        var instance = _eventInstance[index];
        float value;
        float speed;
        float time = 0f;
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while (time < timeToCalculate)
        {
            time += Time.deltaTime;
            speed = curve.Evaluate(time / timeToCalculate);
            value = Mathf.Lerp(startValue, endValue, speed);
            instance.setParameterByName(parameterName, value);
            yield return null;
        }
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
    public void Play_OneShot(string key)
    {
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;

        RuntimeManager.PlayOneShot(eventReference);

    }
    public void Play_OneShot(string key, string parameterName, float value)
    {
        if (!AudioEventsDict.TryGetValue(key, out EventReference eventReference)) return;

        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        instance.setParameterByName(parameterName, value);
        instance.start();
        instance.release();


    }
    [Button("Debug : Stop All Instance")]

    public void StopAllInstance()
    {
        if (_eventInstance == null || _eventInstance.Count == 0) return;
        foreach (var instance in _eventInstance)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }
    public void StopAllInstance(string parameterName, float startValue, float endValue, float timeToCalculate)
    {
        if (_eventInstance == null || _eventInstance.Count == 0) return;
        foreach (var instance in _eventInstance)
        {
            int index = _eventInstance.IndexOf(instance);
            AudioManager.Instance.StartCoroutine(EventParamter(index, parameterName, startValue, endValue, timeToCalculate));
        }
    }
    [Button("Debug : Check Instance")]
    public void CheckInstance()
    {
        Debug.Log(_eventInstance.Count);
    }
}
[System.Serializable]
public class AudioFMODEvent<T>
{
    [field: SerializeField] public T Key { get; private set; }
    [field: SerializeField] public EventReference AudioReference { get; private set; }
}