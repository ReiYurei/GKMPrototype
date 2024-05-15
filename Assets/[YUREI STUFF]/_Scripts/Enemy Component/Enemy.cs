using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;
using FMOD;

#if UNITY_EDITOR
#endif
public class Enemy : MonoBehaviour, IAudioSource
{
    [field: SerializeField][InlineEditor]public StateObserver GameState { get; private set; }
    [field: SerializeField][InlineEditor][Required]public SO_EnemyStatus StatusData { get; private set; }
    [field: SerializeField][InlineEditor][Required]public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public EnemyAnimator EnemyAnimatorComponent { get; private set; }
    [field: SerializeField] public EnemyBehaviour EnemyBehaviourComponent { get; private set; }
    [field: SerializeField] public StatusEffectContainer StatusEffectContainerComponent { get; private set; }
    [field: SerializeField] public EventListenerComponent EventListener { get; private set; }
    [field: SerializeField] public Rigidbody2D RigidbodyComponent { get; private set; }
    [field: SerializeField] public Animator AnimatorComponent { get; private set; }
    [field: SerializeField] public List<Transform> Waypoints { get; private set; }

    private void OnEnable()
    {
        if (StatusData != null)
        {
            StatusData.OnSpawn();
            StatusData.AttackEnd += OnAttackEnd;
            StatusData.ShootEnd += OnShootEnd;
        }
    }
    private void OnDisable()
    {
        if (StatusData != null)
        {
            StatusData.OnSpawn();
            StatusData.AttackEnd -= OnAttackEnd;
            StatusData.ShootEnd -= OnShootEnd;
        }
    }
    public void Start()
    {
        AudioCollection.InitializeStartData();

        StatusEffectContainerComponent = StatusEffectContainerComponent != null ? StatusEffectContainerComponent : TryGetComponent<StatusEffectContainer>(out StatusEffectContainer statusEffect) ? StatusEffectContainerComponent = statusEffect : null;
        EnemyAnimatorComponent = EnemyAnimatorComponent != null ? EnemyAnimatorComponent : TryGetComponent<EnemyAnimator>(out EnemyAnimator enemyAnimator) ? EnemyAnimatorComponent = enemyAnimator : null;
        EnemyBehaviourComponent = EnemyBehaviourComponent != null ? EnemyBehaviourComponent : TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemyBehaviour) ? EnemyBehaviourComponent = enemyBehaviour : null;
        RigidbodyComponent = RigidbodyComponent != null ? RigidbodyComponent : TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d) ? RigidbodyComponent = rb2d : null;
        AnimatorComponent = AnimatorComponent != null ? AnimatorComponent : TryGetComponent<Animator>(out Animator animator) ? AnimatorComponent = animator : null;
        EventListener = EventListener != null ? EventListener : TryGetComponent<EventListenerComponent>(out EventListenerComponent eventListener) ? EventListener = eventListener : null;
        //RigidbodyComponent.gravityScale = 10;

    }
    public void PlayTheme()
    {
        AudioCollection.Play("Music Theme");
    }
    public void StopTheme()
    {
        AudioCollection.StopInstance("Music Theme","Volume",0,1,2f);

    }
    public void OnBulletHellPhase()
    {
        StatusData.B_StatusBuildUp.value = false;
        StatusData.F_RageMeter.value = 0;
        GravitySet(0);
    }
    public void OnRegularPhase()
    {
        StatusData.B_StatusBuildUp.value = true;
        GravitySet(50);
    }




    public float speed;
    public float acceleration;
    void OnAnimatorMove()
    {
        if(RigidbodyComponent != null)
        {
            RigidbodyComponent.velocity = new Vector3(AnimatorComponent.deltaPosition.x / Time.deltaTime, AnimatorComponent.deltaPosition.y * speed / Time.deltaTime);
        }
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
    public void GravitySet(float value)
    {
        RigidbodyComponent.gravityScale = value;

    }
    //Used by Animation Event
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {

        StatusEffectContainerComponent ??= TryGetComponent(out StatusEffectContainer statusComponent) ?
        StatusEffectContainerComponent = statusComponent : StatusEffectContainerComponent = gameObject.AddComponent<StatusEffectContainer>();


        EnemyAnimatorComponent ??= TryGetComponent<EnemyAnimator>(out EnemyAnimator enemyAnimatorComponent) ?
        EnemyAnimatorComponent = enemyAnimatorComponent : EnemyAnimatorComponent = gameObject.AddComponent<EnemyAnimator>();


        EnemyBehaviourComponent ??= TryGetComponent<EnemyBehaviour>(out EnemyBehaviour behaviourComponent) ?
        EnemyBehaviourComponent = behaviourComponent : EnemyBehaviourComponent = gameObject.AddComponent<EnemyBehaviour>();

        EventListener ??= TryGetComponent<EventListenerComponent>(out EventListenerComponent listenerComponent) ?
        EventListener = listenerComponent : EventListener = gameObject.AddComponent<EventListenerComponent>();

        TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent);
        if(rbComponent == null) { RigidbodyComponent = gameObject.AddComponent<Rigidbody2D>(); }
        else { RigidbodyComponent = rbComponent; }

        TryGetComponent<Animator>(out Animator animatorComponent);
        if(animatorComponent == null) { gameObject.AddComponent<Animator>(); }
        else { AnimatorComponent = animatorComponent; }

    }
}
