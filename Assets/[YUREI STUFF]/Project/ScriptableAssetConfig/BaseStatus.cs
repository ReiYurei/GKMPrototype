using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public abstract class BaseStatus : ScriptableObject
{
    [Header("General Status")]
    [SerializeField] float _maxHealth;

    [SerializeField][GUIColor(1f, 1f, 0f)] public FloatVariable currentHealth;
    public virtual void OnSpawn()
    {
        currentHealth.value = _maxHealth;
    }
    
    public float GetHealth()
    {
        return currentHealth.value;
    }
    public float SetHealth(float health)
    {
        return currentHealth.value = health;
    }

    public float _rawPower;
    public float _movementSpeed;
    public float Power(float modifier)
    {
        return _rawPower * modifier;
    }

    


}
