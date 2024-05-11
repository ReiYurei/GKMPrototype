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

    public static AudioManager Instance { get; private set; }
    public void Start()
    {
        MusicCollection.InitializeStartData();
        ProjectileCollection.InitializeStartData();
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
        MusicCollection.StopAllInstance("Volume", 0,1,2f);
        ProjectileCollection.StopAllInstance();
    }
}
