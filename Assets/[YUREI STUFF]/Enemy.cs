using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

#if UNITY_EDITOR
#endif
public class Enemy : MonoBehaviour, IDamageable, IStatusInflictable
{
    [InlineEditor]
    [Required] public Enemy_Status status;
    public EnemyAnimator enemyAnimator;
    public EnemyBehaviour enemyBehaviour;
    public StatusEffectContainer statusEffectContainer;
    public Rigidbody2D rb;
    private Animator _animator;
 
    public void OnDamage(float damage)
    {
        status.AffectRage(damage);
        status.SetHealth(status.GetHealth() - damage);
    }


    public void OnStatusInflicted(float value, BaseStatusEffect effect)
    {

    }
    public float speed;
    void OnAnimatorMove()
    {
        rb.velocity = new Vector3 (_animator.deltaPosition.x / Time.deltaTime, _animator.deltaPosition.y * speed / Time.deltaTime);


    }

    public void Start()
    {
        status.OnSpawn();
        status.AttackEnd += OnAttackEnd;
        statusEffectContainer = GetComponent<StatusEffectContainer>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        rb.gravityScale = 10;

    }

    private void OnAttackEnd(bool isAnimEnd)
    {

      switch (isAnimEnd)
      {
          case false:
                // rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 10;
              break;
          case true:
                //  rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 1;
                break;
      
      }
    }
    public void GravityDecrease(float value)
    {
        rb.gravityScale = value;

    }
    public void GravityIncrease(float value)
    {
        rb.gravityScale = value;
    }
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {

        statusEffectContainer ??= TryGetComponent<StatusEffectContainer>(out StatusEffectContainer statusComponent) ?
        statusEffectContainer = statusComponent : statusEffectContainer = gameObject.AddComponent<StatusEffectContainer>();


        enemyAnimator ??= TryGetComponent<EnemyAnimator>(out EnemyAnimator enemyAnimatorComponent) ?
        enemyAnimator = enemyAnimatorComponent : enemyAnimator = gameObject.AddComponent<EnemyAnimator>();


        enemyBehaviour ??= TryGetComponent<EnemyBehaviour>(out EnemyBehaviour behaviourComponent) ?
        enemyBehaviour = behaviourComponent : enemyBehaviour = gameObject.AddComponent<EnemyBehaviour>();

        TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent);
        if(rbComponent == null) { rb = gameObject.AddComponent<Rigidbody2D>(); }
        else { rb = rbComponent; }

        TryGetComponent<Animator>(out Animator animatorComponent);
        if(animatorComponent == null) { gameObject.AddComponent<Animator>(); }

    }
}
