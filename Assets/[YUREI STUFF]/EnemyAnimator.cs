using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]Enemy_Status status;
    public Animator _animator;


    private void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            status = component._status;

        }
        _animator = GetComponent<Animator>();
        status.NotifyAnimChange += OnAnimChange;
    }
    private void OnDisable()
    {
        status.NotifyAnimChange -= OnAnimChange;
    }

    private void OnAnimChange()
     {
        PlayAnim(status.GetAnimationHash(), status.AnimationSpeed);
     }

    public void PlayAnim(int animationName, float animSpeed)
    {
        _animator.speed = animSpeed;
        _animator.Play(animationName, default,0f);
    }

}
