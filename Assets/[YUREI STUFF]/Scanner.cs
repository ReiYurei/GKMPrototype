using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Substates_Scanning", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Scanning")]
public class Scanner : MonoBehaviour
{
    [field: SerializeField] public GameStateManager StateManager { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OutOfRangeEvent {  get; private set; }
    [field: SerializeField] public SO_VoidGameEvent InRangeEvent { get; private set; }

    [SerializeField] private GameObject _player;
    [SerializeField] private Rigidbody2D _playerRb;
    [SerializeField] private SO_PlayerInfo _playerInfo;
    [SerializeField] private Vector3 _playerLocation;
    [SerializeField] private Vector3 _playerOffset;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _rangeRadius;
    [SerializeField] private Vector3 _radiusOffset;
    [SerializeField] private float _distance;
    private EnemyStates _state;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        _playerLocation = _player.transform.position;

    }
    void LateUpdate()
    {
        _distance = GetDistance();
        _playerLocation = _player.transform.position;
        if(StateManager.CurrentState.State is RegularGameplayState)
        {
            if (_distance > _rangeRadius && _state != EnemyStates.OutOfRange)
            {
                StartCoroutine(DistanceTolerance());
            }
            else if (_distance <= _rangeRadius && _state == EnemyStates.OutOfRange)
            {
                _state = EnemyStates.InRange;
                StopAllCoroutines();
                InRangeEvent.Raise();
            }
            else if (_distance <= _rangeRadius)
            {
                _state = EnemyStates.InRange;
                StopAllCoroutines();
            }
        }
    }
    IEnumerator DistanceTolerance()
    {
        float time = 0f;
        float tolerance = 3f;
        while (time < tolerance)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _state = EnemyStates.OutOfRange;
        OutOfRangeEvent.Raise();
    }
    public float GetDistance()
    {
        return Vector2.Distance(this.transform.position, _playerLocation);
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
        Gizmos.DrawLine(transform.position + _offset, _playerLocation);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + _radiusOffset, _rangeRadius);
    }

    // [SerializeField] private string _name = "Enemy_Scan";
    // public override void Execute()
    // {
    //     Debug.Log("Scanning");
    // }
    // public override string GetName()
    // {
    //     return _name;
    // }
}

