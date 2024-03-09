using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Idling", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Idling")]
public class SO_Idling : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Idle;

    public override IEnumerator Execute(Enemy enemy)
    {
        float time = enemy._status.WaitTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
