using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Ignore if Enemy component exist in object")]
    [ReadOnly]      public Enemy _enemy;
    [SerializeField]public EnemyStatus _status;
    [Header("Main Field")]
    [SerializeField] SO_PlayerInfo _playerInfo;
    [SerializeField] public List<KeyGameEvent> projectileEvent;
    Dictionary<string, GameEvent> projectileDict;
    public Animator animator;

    private void Awake()
    {
        projectileDict = new Dictionary<string, GameEvent>();
    }

    private void OnEnable()
    {

        for(int i = 0; i < projectileEvent.Count; i++)
        {
            if (projectileEvent[i] == null) continue;
            projectileDict.Add(projectileEvent[i].key, projectileEvent[i].gameEvent);
        }
        if (_status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            if (component == null)
            {
                Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                    $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(EnemyStatus)}");
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
    public void OnProjectile(string key)
    {
        //_status.NotifyProjectile();
        //projectileEvent[index].Raise();
        projectileDict.TryGetValue(key, out GameEvent gameEvent);
        if (gameEvent == null) return;
        gameEvent.Raise();
        Debug.Log("Raising Event");
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
