using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class EnemyAnimator : MonoBehaviour
{
    public Enemy_Status status;
    public Animator _animator;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
    }

     private void Update()
     {
        PlayAnim(status._animationName);
     }

    public void PlayAnim(string animationName)
    {

        _animator.Play(animationName);

    }

}
