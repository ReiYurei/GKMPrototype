using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;
using Unity.VisualScripting;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]Enemy_Status status;
    [SerializeField]SO_PlayerInfo playerInfo;
    public Animator _animator;
    bool isFlipped;

    private void Update()
    {
    }

    private void FlipAnimation()
    {

        if (playerInfo.position.x < transform.position.x)
        {
           transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerInfo.position.x > transform.position.x )
        {
           transform.eulerAngles = new Vector3(0, 180, 0);

        }
    }

    private void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            status = component._status;

        }
        _animator = GetComponent<Animator>();
        status.NotifyAnimChange += OnAnimChange;
        status.WaitTime = _animator.GetCurrentAnimatorStateInfo(0).length;
    }
    private void OnDisable()
    {
        status.NotifyAnimChange -= OnAnimChange;
    }

    private void OnAnimChange()
    {
        if (status.noFlip == false)
        {
            FlipAnimation();
        }
        status.WaitTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        PlayAnim(status.GetAnimationHash(), status.AnimationSpeed);
    }

    public void PlayAnim(int animationName, float animSpeed)
    {
        _animator.speed = animSpeed;
        _animator.Play(animationName, default,0f);
    }

}
