using System.Collections;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Move_Point", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Move to a Point")]
public class SO_MoveToPoint : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Moving;
    [SerializeField] SO_PlayerInfo _playerInfo;
    [SerializeField] private bool _toPlayer;
    [ShowIf(nameof(_toPlayer), true)][SerializeField] private float _distance;
    [ShowIf(nameof(_toPlayer),false)] [SerializeField] private int _waypointIndex;
    [ShowIf(nameof(_constantSpeed), false)][SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [ShowIf(nameof(_constantSpeed), false)][SerializeField] private float _timeToReachPoint = 2f;
    [ShowIf(nameof(_toPlayer), false)][SerializeField] private Vector3 _offset;
    [SerializeField] private bool _constantSpeed;
    [ShowIf(nameof(_constantSpeed), true)][SerializeField] private float _moveSpeed;
    private float _time;
    private int _moveEndHash;

    public override IEnumerator Execute(Enemy enemy)
    {
        _time = 0f;
        enemy.GravitySet(0);
        MoveDirection(enemy);
        if(enemy.Waypoints.Count <= 0)
        {
            enemy.StatusData.SetNoFlip(false);
            enemy.StatusData.SetIsMoving(false);
            yield break;
        }
        Vector3 targetPos;
        Vector3 originPos = enemy.gameObject.transform.position + _offset;
        if (_toPlayer)
        {
            targetPos = new Vector3(_playerInfo.position.x, enemy.gameObject.transform.position.y, enemy.gameObject.transform.position.z);
        }
        else targetPos = enemy.Waypoints[_waypointIndex].position;
        float speed;
        float distance = Vector3.Distance(originPos, targetPos);
        if (_toPlayer)
        {
            while (distance >= _distance)
            {
                _time += Time.deltaTime;
                if(!_constantSpeed) speed = _speedCurve.Evaluate(_time / _timeToReachPoint);
                else speed = _moveSpeed * Time.deltaTime;

                enemy.gameObject.transform.position = Vector3.Lerp(originPos, targetPos, speed);
                distance = Vector3.Distance(enemy.gameObject.transform.position, targetPos);
                yield return null;
            }
            enemy.StatusData.SetAnimationHashAndNotify(_moveEndHash);
            Debug.Log("Move End");
            yield break;
        }
    
        while (enemy.gameObject.transform.position != targetPos)
        {
            _time += Time.deltaTime;
            if (!_constantSpeed) speed = _speedCurve.Evaluate(_time / _timeToReachPoint);
            else speed = _moveSpeed * Time.deltaTime;

            enemy.gameObject.transform.position = Vector3.Lerp(originPos, targetPos, speed);
            yield return null;
        }
        enemy.StatusData.SetAnimationHashAndNotify(_moveEndHash);
        Debug.Log("Move End");
        yield break;
    }

    void MoveDirection(Enemy enemy)
    {
        if (enemy.Waypoints[_waypointIndex].position.x < enemy.transform.position.x | _playerInfo.position.x < enemy.transform.position.x)
        {
            enemy.StatusData.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Forward);
            _moveEndHash = AnimationHash.Enemy_Move_F_Stop;
            return;
        }
        enemy.StatusData.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Backward);
        _moveEndHash = AnimationHash.Enemy_Move_B_Stop;
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
