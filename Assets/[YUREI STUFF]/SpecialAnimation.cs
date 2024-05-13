using System.Collections;
using UnityEngine;
using TriInspector;
[RequireComponent(typeof(EventListenerComponent))]
public class SpecialAnimation : MonoBehaviour
{
    public EnemyStates state;
    public Animator animator;
    public string animName;
    [Tooltip("EXPERIMENTAL : DON'T USE IT")]
    public bool moveToPoint;

    [ShowIf(nameof(moveToPoint), true)]public Transform waypoint;
    [ShowIf(nameof(moveToPoint), true)]public float timeToMove;
    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void MoveToPoint()
    {
        if (!moveToPoint) return;
        StartCoroutine(Move());
        IEnumerator Move()
        {
            float time = 0f;
            float speed;
            Vector3 origin = animator.gameObject.transform.position;
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1); 
            while(time < timeToMove)
            {
                time += Time.deltaTime;
                speed = curve.Evaluate(time / timeToMove);
                animator.gameObject.transform.position = Vector3.Lerp(origin, waypoint.position, speed);
                yield return null;

            }
        }
    }
    public void PlaySpecialAnimation()
    {
        animator.Play(animName,default,0f);
    }
}