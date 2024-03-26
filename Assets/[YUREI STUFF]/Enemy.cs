using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

#if UNITY_EDITOR
#endif
public class Enemy : MonoBehaviour, IDamageable, IStatusInflictable
{
    [field: SerializeField][InlineEditor][Required]public EnemyStatus StatusData { get; private set; }
    [field: SerializeField] public EnemyAnimator EnemyAnimatorComponent { get; private set; }
    [field: SerializeField] public EnemyBehaviour EnemyBehaviourComponent { get; private set; }
    [field: SerializeField] public StatusEffectContainer StatusEffectContainerComponent { get; private set; }
    [field: SerializeField] public EventListenerComponent EventListenerComponent { get; private set; }
    [field: SerializeField] public Rigidbody2D RigidbodyComponent { get; private set; }
    [field: SerializeField] public Animator AnimatorComponent { get; private set; }

    public void OnDamage(float damage)
    {
        StatusData.AffectRage(damage);
        StatusData.SetHealth(StatusData.GetHealth() - damage);
    }


    public void OnStatusInflicted(float value, BaseStatusEffect effect)
    {

    }
    public float speed;
    public float acceleration;
    void OnAnimatorMove()
    {
        RigidbodyComponent.velocity = new Vector3 (AnimatorComponent.deltaPosition.x / Time.deltaTime, AnimatorComponent.deltaPosition.y * speed  / Time.deltaTime);


    }

    public void Start()
    {
        StatusData.OnSpawn();
        StatusData.AttackEnd += OnAttackEnd;
        StatusData.ShootEnd += OnShootEnd;

        StatusEffectContainerComponent = GetComponent<StatusEffectContainer>();
        EnemyAnimatorComponent = GetComponent<EnemyAnimator>();
        EnemyBehaviourComponent = GetComponent<EnemyBehaviour>();
        RigidbodyComponent = GetComponent<Rigidbody2D>();
        AnimatorComponent = GetComponent<Animator>();
        EventListenerComponent = GetComponent<EventListenerComponent>();
        //RigidbodyComponent.gravityScale = 10;

    }

    private void OnAttackEnd(bool isAnimEnd)
    {

        switch (isAnimEnd)
      {
          case false:
                RigidbodyComponent.gravityScale = 40;
              break;
          case true:
                RigidbodyComponent.gravityScale = 0;
                break;    
      }
    }
    private void OnShootEnd(bool isAnimEnd)
    {
        switch (isAnimEnd)
        {
            case false:
                RigidbodyComponent.gravityScale = 40;
                break;
            case true:
                RigidbodyComponent.gravityScale = 0;
                break;

        }
    }
    //Used by Animation Event
    public void GravityDecrease(float value)
    {
        RigidbodyComponent.gravityScale = value;

    }
    //Used by Animation Event
    public void GravityIncrease(float value)
    {
        RigidbodyComponent.gravityScale = value;
    }
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {

        StatusEffectContainerComponent ??= TryGetComponent<StatusEffectContainer>(out StatusEffectContainer statusComponent) ?
        StatusEffectContainerComponent = statusComponent : StatusEffectContainerComponent = gameObject.AddComponent<StatusEffectContainer>();


        EnemyAnimatorComponent ??= TryGetComponent<EnemyAnimator>(out EnemyAnimator enemyAnimatorComponent) ?
        EnemyAnimatorComponent = enemyAnimatorComponent : EnemyAnimatorComponent = gameObject.AddComponent<EnemyAnimator>();


        EnemyBehaviourComponent ??= TryGetComponent<EnemyBehaviour>(out EnemyBehaviour behaviourComponent) ?
        EnemyBehaviourComponent = behaviourComponent : EnemyBehaviourComponent = gameObject.AddComponent<EnemyBehaviour>();

        EventListenerComponent ??= TryGetComponent<EventListenerComponent>(out EventListenerComponent listenerComponent) ?
        EventListenerComponent = listenerComponent : EventListenerComponent = gameObject.AddComponent<EventListenerComponent>();

        TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent);
        if(rbComponent == null) { RigidbodyComponent = gameObject.AddComponent<Rigidbody2D>(); }
        else { RigidbodyComponent = rbComponent; }

        TryGetComponent<Animator>(out Animator animatorComponent);
        if(animatorComponent == null) { gameObject.AddComponent<Animator>(); }
        else { AnimatorComponent = animatorComponent; }

    }
}
