using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

#if UNITY_EDITOR
#endif
public class Enemy : MonoBehaviour, IDamageable, IStatusInflictable
{
    [InlineEditor]
    [Required] public Enemy_Status _status;
    public EnemyAnimator _enemyAnimator;
    public EnemyBehaviour _enemyBehaviour;
    public StatusEffectContainer _statusEffectContainer;
    public Rigidbody2D _rb;

 
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
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {

        _statusEffectContainer ??= TryGetComponent<StatusEffectContainer>(out StatusEffectContainer statusComponent) ?
        _statusEffectContainer = statusComponent : _statusEffectContainer = gameObject.AddComponent<StatusEffectContainer>();


        _enemyAnimator ??= TryGetComponent<EnemyAnimator>(out EnemyAnimator enemyAnimatorComponent) ?
        _enemyAnimator = enemyAnimatorComponent : _enemyAnimator = gameObject.AddComponent<EnemyAnimator>();


        _enemyBehaviour ??= TryGetComponent<EnemyBehaviour>(out EnemyBehaviour behaviourComponent) ?
        _enemyBehaviour = behaviourComponent : _enemyBehaviour = gameObject.AddComponent<EnemyBehaviour>();

        TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent);
        if(rbComponent == null) { _rb = gameObject.AddComponent<Rigidbody2D>(); }
        else { _rb = rbComponent; }

        TryGetComponent<Animator>(out Animator animatorComponent);
        if(animatorComponent == null) { gameObject.AddComponent<Animator>(); }

    }
}
