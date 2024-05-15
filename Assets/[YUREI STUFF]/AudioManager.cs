using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class AudioManager : MonoBehaviour
{
    [field: SerializeField] public SO_AudioFMODEventCollection MusicCollection { get; private set; }
    [field: SerializeField] public SO_AudioFMODEventCollection ProjectileCollection { get; private set; }
    [field: SerializeField] public SO_AudioFMODEventCollection GenericSoundCollection { get; private set; }

    public static AudioManager Instance { get; private set; }
    public void Start()
    {
        if (MusicCollection != null) MusicCollection.InitializeStartData();
        if (ProjectileCollection != null) ProjectileCollection.InitializeStartData();
        if (GenericSoundCollection != null) GenericSoundCollection.InitializeStartData();
    }
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void Stop()
    {
        if (MusicCollection != null) MusicCollection.StopAllInstance("Volume", 0, 1, 2f);
        if (ProjectileCollection != null) ProjectileCollection.StopAllInstance();
        if (GenericSoundCollection != null) GenericSoundCollection.StopAllInstance();
    }
}
