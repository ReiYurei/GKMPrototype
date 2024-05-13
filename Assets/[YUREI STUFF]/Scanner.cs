using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[RequireComponent(typeof(EventListenerComponent))]
public class Scanner : MonoBehaviour
{
    [field: SerializeField] public StateObserver Observer { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OutOfRangeEvent {  get; private set; }
    [field: SerializeField] public SO_VoidGameEvent InRangeEvent { get; private set; }

    [field: SerializeField] public FloatVariable Distance { get; private set; }
    [field: SerializeField] public FloatVariable Height { get; private set; }

    [ReadOnly] public Enemy _enemy;
    [ReadOnly] public GameObject _player;
    [ReadOnly] public Rigidbody2D _playerRb;
    [ReadOnly] public Vector3 _playerLocation;
    [SerializeField] private SO_PlayerInfo _playerInfo;
    [SerializeField] private Vector3 _playerOffset;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _rangeRadius;
    [SerializeField] private Vector3 _radiusOffset;
    [SerializeField] private float _outOfRangeDelayTolerance;
    [SerializeField] private float _inRangeDelayTolerance;
    [SerializeField] private float _distance;
    private EnemyStates _state;
    public bool notifyOutOfRange = true;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemy = GameObject.FindGameObjectWithTag("Astral Entity").GetComponent<Enemy>();
        _playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        _playerLocation = _player.transform.position;

    }
    public void OnLoadComplete()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemy = GameObject.FindGameObjectWithTag("Astral Entity").GetComponent<Enemy>();
        _playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        _playerLocation = _player.transform.position;
    }
    void LateUpdate()
    {
        if (_player == null) return;
        _distance = GetDistance();
        Distance.value = GetDistance();
        Height.value = GetHeight();
        _playerLocation = _player.transform.position;
        if (!notifyOutOfRange) return;
        if(Observer.State is RegularGameplayState)
        {
            if (_distance > _rangeRadius && _state != EnemyStates.OutOfRange)
            {
                _state = EnemyStates.OutOfRange;
                Debug.Log("Out of range Range");
                StopAllCoroutines();
                StartCoroutine(DistanceTolerance());
            }
            else if (_distance <= _rangeRadius && _state == EnemyStates.OutOfRange)
            {
                _state = EnemyStates.InRange;
                Debug.Log("In Range");
                StopCoroutine(InRangeDelay());
                StartCoroutine(InRangeDelay());
            }
        }
    }
    IEnumerator InRangeDelay()
    {
        float time = 0f;
        while (time < _inRangeDelayTolerance)
        {
            time += Time.deltaTime;
            yield return null;
        }
        while (_enemy.StatusData.IsShooting || _enemy.StatusData.IsAttacking)
        {
            yield return null;
        }
        InRangeEvent.Raise();
    }
    IEnumerator DistanceTolerance()
    {
        float time = 0f;
        while (time < _outOfRangeDelayTolerance)
        {
            time += Time.deltaTime;
            yield return null;
        }
        while (_enemy.StatusData.IsShooting || _enemy.StatusData.IsAttacking)
        {
            yield return null;
        }
        OutOfRangeEvent.Raise();
    }
    public float GetDistance()
    {
        return Vector2.Distance(this.transform.position, _playerLocation);
    }
    public float GetHeight()
    {
        return (transform.position.y + _offset.y) - (_playerLocation.y + _playerOffset.y);
    }
    public bool CheckPlayerJump()
    {
        if (_playerRb.velocity.y > 0.1f && _playerRb != null)
        {
            return true;
        }
        else if (_playerLocation.y > this.transform.position.y)
        {
            return true;
        }
        return false;
    }

    [SerializeField, TextArea(4, 10)]
    string DEBUG_MESSAGE;

    
    public Vector2 GetPlayerLocation()
    {
        return _playerLocation;

    }
    private void OnValidate()
    {
        if (_player == null) return;
        _playerLocation = _player.transform.position + _playerOffset;
        _distance = GetDistance();
    }
    private void OnDrawGizmos()
    {
        if (_player == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + _offset, _playerLocation + _playerOffset);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + _radiusOffset, _rangeRadius);
    }
}
