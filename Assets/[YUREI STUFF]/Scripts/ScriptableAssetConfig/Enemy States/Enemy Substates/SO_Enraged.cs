using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substates_Enraged", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Enraged")]

public class SO_Enraged : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Enraged;

    public override IEnumerator Execute(Enemy enemy)
    {
        yield break;
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
