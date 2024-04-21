using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Move_Point", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Move to a Point")]
public class SO_MoveToPoint : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Moving;
    [SerializeField] SO_PlayerInfo playerInfo;
    [SerializeField] private int waypointIndex;
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private float _timeToReachPoint = 2f;
    [SerializeField] private Vector3 _offset;
    private float _moveSpeed;
    private float _time;
    int moveEndHash;

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
        Vector3 targetPos = enemy.Waypoints[waypointIndex].position;
        Vector3 originPos = enemy.gameObject.transform.position + _offset;
        while (enemy.gameObject.transform.position != targetPos)
        {
            _time += Time.deltaTime;
            _moveSpeed = _speedCurve.Evaluate(_time / _timeToReachPoint);

            enemy.gameObject.transform.position = Vector3.Lerp(originPos, targetPos, _moveSpeed);
            yield return null;
        }
        enemy.StatusData.SetAnimationHashAndNotify(moveEndHash);
        Debug.Log("Move End");
        yield break;
    }

    void MoveDirection(Enemy enemy)
    {
        if (enemy.Waypoints[waypointIndex].position.x < playerInfo.position.x)
        {
            enemy.StatusData.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Forward);
            moveEndHash = AnimationHash.Enemy_Move_F_Stop;
            return;
        }
        enemy.StatusData.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Backward);
        moveEndHash = AnimationHash.Enemy_Move_B_Stop;
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
