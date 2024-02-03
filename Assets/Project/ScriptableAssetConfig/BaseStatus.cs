using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStatus : ScriptableObject
{
    [Header("General Status")]
    public float _maxHealth;
    private float currentHealth;
    public virtual void OnSpawn()
    {
        currentHealth = _maxHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    public float SetHealth(float health)
    {
        return currentHealth = health;
    }

    public float _rawPower;
    public float _movementSpeed;
    public float Power(float modifier)
    {
        return _rawPower * modifier;
    }
}
