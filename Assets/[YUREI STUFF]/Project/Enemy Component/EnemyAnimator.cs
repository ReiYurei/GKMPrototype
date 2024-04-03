using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;
using FMODUnity;

// Summary:
// Manages the animation behavior of an _enemy character in the game.
// It controls the _enemy's animation states, flipping animations based on player position,
// and triggering _projectileObj-related events.
public class EnemyAnimator : MonoBehaviour
{

    [Header("READ ONLY PROPERTIES")]
    [SerializeField]private Enemy _enemy;
    [SerializeField]private SO_EnemyStatus _status;

    [Header("Main Field")]
    [Tooltip("Player related information")]
    [SerializeField]private SO_PlayerInfo _playerInfo;

    [Tooltip("Key and Event to store Game Event in dictionary")]
    [SerializeField]private List<KeyGameEvent<string>> _projectileEvent;
    private Dictionary<string, SO_VoidGameEvent> _projectileDict;

    private Animator _animator;

    //Initialize the _projectileObj dictionary
    private void Awake()
    {
        _projectileDict = new Dictionary<string, SO_VoidGameEvent>();
    }

    // Subscribes to events and populates the _projectileObj dictionary and initialize the required component
    private void OnEnable()
    {
        TryGetComponent(out Enemy component);
        if (component == null)
        {
            Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(SO_EnemyStatus)}");
        }
        _enemy = component;
        if (component.StatusData != null)
        {
            _status = _enemy.StatusData;
            _status.NotifyAnimChange += OnStateChange;
        }
        for (int i = 0; i < _projectileEvent.Count; i++)
        {
            if (_projectileEvent[i] == null) continue;
            _projectileDict.Add(_projectileEvent[i].Key, _projectileEvent[i].GameEvent);
        }
   
        _animator = GetComponent<Animator>();
        
    }

    //Unsubscribes from events
    private void OnDisable()
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
            _status.NotifyAnimChange -= OnStateChange;
    }

    //Flip the animation and then play the animation
    private void OnStateChange()
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        if (_status.NoFlip == false) FlipAnimation();
        PlayAnim(_status.GetAnimationHashFromStatus(), _status.AnimationSpeed);
    }

    //Used for Animation Event function to notify the current attack reached its last frame within halved duration
    protected void OnHalved()
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        if (_status.IsHalved == false) return;

        _status?.NotifyEndOfAnim(true);

    }

    //Used for Animation Event function to notify the current attack reached its last frame
    protected void OnEnded()
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        _status.NotifyEndOfAnim(true);
    }

    //Used for Animation Event function to notify _projectileObj engine, supposedly to start the engine
    public void OnProjectile(string key)
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        _projectileDict.TryGetValue(key, out SO_VoidGameEvent gameEvent);
        if (gameEvent == null) return;
        gameEvent.Raise();
        Debug.Log("Raising Event");
    }
    public void OnPlaySound(string key)
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        var audioBank = _enemy.EnemyAudioCollection;
        if (audioBank.AudioEventsDict.TryGetValue(key, out EventReference eventReference))
        {
            RuntimeManager.PlayOneShot(eventReference);
            return;
        }
        Debug.Log("not found");
    }
    //Play Animation by Animation Hash
    private void PlayAnim(int animationName, float animSpeed)
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
        _status.NotifyEndOfAnim(false);
        _animator.speed = animSpeed;
        _animator.Play(animationName, default,0f);
    }

    //Flip Object
    private void FlipAnimation()
    {
        if (_enemy == null) return;
        if (_enemy.GameState == GameState.Cutscene) return;
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
