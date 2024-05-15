using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substates_Flinched", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Flinched")]

public class SO_Flinched : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Flinched;
    public override IEnumerator Execute(Enemy enemy)
    {
        yield break;
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
