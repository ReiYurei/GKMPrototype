using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage, bool isGuardable = true);

}
public interface IProjectileAttack
{

}
public interface IAudioSource
{
    [SerializeField]public SO_AudioFMODEventCollection AudioCollection { get; }
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