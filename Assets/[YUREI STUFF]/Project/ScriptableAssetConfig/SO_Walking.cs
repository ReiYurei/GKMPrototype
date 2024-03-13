using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Move", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Moving")]
public class SO_Walking : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Moving;
    [SerializeField] SO_PlayerInfo playerInfo;
    [SerializeField] bool isBackward;
    [SerializeField] float travelDistance;
    [SerializeField] float travelSpeed;
    int moveEndHash;
    Vector3 targetPosition;
    bool hasExecutedOnce = false;
    float distance;


    public override IEnumerator Execute(Enemy enemy)
    {
        hasExecutedOnce = false;
        enemy._status.SetNoFlip(false);
        enemy._status.SetIsMoving(true);
        if ((int)playerInfo.position.x < (int)enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance) * -1; //Towards

        }
        else if (playerInfo.position.x > enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance); //Reverted Towards
        }
        MoveDirection(enemy, isBackward);

        while  (enemy._status._isMoving == true) 
        {
            distance = Vector3.Distance(enemy.transform.position,targetPosition );
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, travelSpeed * Time.deltaTime );
            if (distance / travelSpeed <= 0.25f && hasExecutedOnce == false)
            {
                enemy._status.SetNoFlip(true);
                enemy._status.SetIsMoving(false);
                enemy._status.SetAnimationHashAndNotify(moveEndHash);
                hasExecutedOnce = true;
                yield return null;
            }
            yield return null;
        }
        enemy._status.SetNoFlip(false);
        enemy._status.SetIsMoving(false);
        yield break;
    }

    void MoveDirection(Enemy enemy, bool backward)
    {
        switch (backward)
        {
            case false:
                targetPosition = new Vector3(enemy.transform.position.x + travelDistance, enemy.transform.position.y, enemy.transform.position.z);
                enemy._status.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Forward);
                moveEndHash = AnimationHash.Enemy_Move_F_Stop;
                break;
            case true:
                targetPosition = new Vector3(enemy.transform.position.x - travelDistance, enemy.transform.position.y, enemy.transform.position.z);
                enemy._status.SetAnimationHashAndNotify(AnimationHash.Enemy_Move_Backward);
                moveEndHash = AnimationHash.Enemy_Move_B_Stop;
                break;

        }
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
