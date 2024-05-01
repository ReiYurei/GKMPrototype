using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage);

}
public interface IProjectileAttack
{

}
public interface IAudioSource
{
    [SerializeField]public SO_AudioFMODEventCollection AudioCollection { get; }
    public void PlaySound_OneShot(string key)
    {
        if (AudioCollection.AudioEventsDict.TryGetValue(key, out EventReference eventReference))
        {
            RuntimeManager.PlayOneShot(eventReference);
        }
    }
    public void Pause()
    {
        RuntimeManager.PauseAllEvents(true);
    }
    public void Resume()
    {
        RuntimeManager.PauseAllEvents(false);
    }
}

public interface INumericVariable
{
    public float GetValue();

}
public interface IBoolVariable
{
    public bool GetValue();

}
public interface IStatusInflictable
{
    void OnStatusInflicted(float value, BaseStatusEffect effect);

}

public interface IInteractable
{
    public void OnInteract();
}