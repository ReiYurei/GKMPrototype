using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Ignore if Enemy component exist in object")]
    [ReadOnly]      public Enemy _enemy;
    [SerializeField]public Enemy_Status _status;
    [Header("Main Field")]
    [SerializeField] SO_PlayerInfo _playerInfo;
    public Animator animator;


    private void OnEnable()
    {
        if (_status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            if (component == null)
            {
                Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                    $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(Enemy_Status)}");
                return;
            }
            _enemy = component;
            _status = _enemy.status;
        }
        animator = GetComponent<Animator>();
        _status.NotifyAnimChange += OnStateChange;
    }
    private void OnDisable()
    {
        _status.NotifyAnimChange -= OnStateChange;
    }

    private void OnStateChange()
    {
        if (_status.noFlip == false)
        {
            FlipAnimation();
        }
        PlayAnim(_status.GetAnimationHashFromStatus(), _status.AnimationSpeed);
    }
    protected void OnHalved()
    {
        if (_status.isHalved == false)
        {
            return;
        }
        _status.NotifyEndOfAnim(true);

    }
    protected void OnEnded()
    {
        _status.NotifyEndOfAnim(true);
    }
    public void OnProjectile()
    {
        _status.NotifyProjectile();
    }
    private void PlayAnim(int animationName, float animSpeed)
    {
        _status.NotifyEndOfAnim(false);
        animator.speed = animSpeed;
        animator.Play(animationName, default,0f);
    }
    private void FlipAnimation()
    {

        if (_playerInfo.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (_playerInfo.position.x > transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
    }

}
