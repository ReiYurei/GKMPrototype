using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class Enemy : MonoBehaviour, IDamageable, IStatusInflictable
{
    [InlineEditor]
    public Enemy_Status _status;
    public EnemyAnimator _enemyAnimator;
    public EnemyBehaviour _enemyBehaviour;
    public StatusEffectContainer _statusEffectContainer;


    public void OnDamage(float damage)
    {
        _status.AffectRage(damage);
        _status.SetHealth(_status.GetHealth() - damage);
    }


    public void OnStatusInflicted(float value, BaseStatusEffect effect)
    {

    }


    public void Start()
    {
        _status.OnSpawn();
        _statusEffectContainer = GetComponent<StatusEffectContainer>();
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

}
