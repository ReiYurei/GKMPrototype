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


    public override IEnumerator Execute(Enemy enemy)
    {
        if ((int)playerInfo.position.x < (int)enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance) * -1; //Towards

        }
        else if (playerInfo.position.x > enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance); //Reverted Towards
        }
        MoveDirection(enemy, isBackward);

        while (enemy.transform.position != targetPosition )
        {
            var distance = Vector3.Distance(enemy.transform.position,targetPosition );
            if (distance / travelSpeed <= 0.5f && hasExecutedOnce == false)
            {
                enemy._status.noFlip = true;
                enemy._status.SetAnimationHash(moveEndHash);
                hasExecutedOnce = true;
                yield return null;
            }
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, travelSpeed * Time.deltaTime);
            yield return null;
        }
        enemy._status.noFlip = false;
        hasExecutedOnce = false;
        yield break;
    }

    void MoveDirection(Enemy enemy, bool backward)
    {
        switch (backward)
        {
            case false:
                targetPosition = new Vector3(enemy.transform.position.x + travelDistance, enemy.transform.position.y, enemy.transform.position.z);
                enemy._status.SetAnimationHash(AnimationHash.Enemy_Move_Forward);
                moveEndHash = AnimationHash.Enemy_Move_F_Stop;
                break;
            case true:
                targetPosition = new Vector3(enemy.transform.position.x - travelDistance, enemy.transform.position.y, enemy.transform.position.z);
                enemy._status.SetAnimationHash(AnimationHash.Enemy_Move_Backward);
                moveEndHash = AnimationHash.Enemy_Move_B_Stop;
                break;

        }
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
