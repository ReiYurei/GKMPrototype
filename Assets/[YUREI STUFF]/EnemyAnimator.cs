using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]Enemy_Status status;
    [SerializeField]SO_PlayerInfo playerInfo;
    public Animator _animator;


    private void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            status = component._status;

        }
        _animator = GetComponent<Animator>();
        status.NotifyAnimChange += OnStateChange;
    }
    private void OnDisable()
    {
        status.NotifyAnimChange -= OnStateChange;
    }

    private void OnStateChange()
    {
        if (status._noFlip == false)
        {
            FlipAnimation();
        }
        PlayAnim(status.GetAnimationHashFromStatus(), status.AnimationSpeed);
    }
    protected void OnHalved()
    {
        if (status._isHalved == false)
        {
            return;
        }
        status.NotifyEndOfAnim(true);

    }
    protected void OnEnded()
    {

        status.NotifyEndOfAnim(true);
    }
    public void OnProjectile()
    {
        status.NotifyProjectile();
    }
    private void PlayAnim(int animationName, float animSpeed)
    {
        status.NotifyEndOfAnim(false);
        _animator.speed = animSpeed;
        _animator.Play(animationName, default,0f);
    }
    private void FlipAnimation()
    {

        if (playerInfo.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerInfo.position.x > transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
    }

}
