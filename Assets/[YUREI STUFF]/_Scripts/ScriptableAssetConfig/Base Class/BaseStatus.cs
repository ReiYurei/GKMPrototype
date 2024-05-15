using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public abstract class BaseStatus : ScriptableObject
{
    [field: Header("General Status")]
    [field: SerializeField] public float MaxHealth { get; private set; }

    [SerializeField][GUIColor(1f, 1f, 0f)] public FloatVariable currentHealth;
    [field: SerializeField] public SO_VoidGameEvent HealthChange { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OnDeathEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OnDeathAnimEndEvent { get; private set; }

    public virtual void OnSpawn()
    {
        currentHealth.value = MaxHealth;
    }
    
    public float GetHealth()
    {
        return currentHealth.value;
    }
    [Button("Set Health")]
    public void SetHealth(float health)
    {
        currentHealth.value = health;
        if (HealthChange != null) HealthChange.Raise();
        if (health <= 0)
        {
            OnDeathEvent.Raise(); 
        }
    }

    [field:SerializeField]public float RawPower { get; private set; }
    [field:SerializeField]public float MovementSpeed { get; private set; }
    [field: SerializeField][Required] public int MotionValue { get; private set; }
    public bool isGuardable;
    public void SetMotionValue(int value)
    {
        MotionValue = value;
    }

}
