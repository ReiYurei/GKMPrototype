using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public abstract class BaseStatus : ScriptableObject
{
    [Header("General Status")]
    [SerializeField] float _maxHealth;

    [SerializeField][GUIColor(1f, 1f, 0f)] public FloatVariable currentHealth;
    [field: SerializeField] public SO_VoidGameEvent HealthChange { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OnDeathEvent { get; private set; }

    public virtual void OnSpawn()
    {
        currentHealth.value = _maxHealth;
    }
    
    public float GetHealth()
    {
        return currentHealth.value;
    }
    [Button("Set Health")]
    public void SetHealth(float health)
    {
        if(HealthChange != null) HealthChange.Raise();
        currentHealth.value = health;
        if (health <= 0)
        {
            OnDeathEvent.Raise(); 
        }
    }

    public float _rawPower;
    public float _movementSpeed;
    public float Power(float modifier)
    {
        return _rawPower * modifier;
    }

    


}
