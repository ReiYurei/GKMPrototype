using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage);

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