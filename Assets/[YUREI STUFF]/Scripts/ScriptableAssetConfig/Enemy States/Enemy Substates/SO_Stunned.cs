using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Stunned", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Stunned")]
public class SO_Stunned : SO_Enemy_Substate
{
    [SerializeField]EnemyStates enemyStates = EnemyStates.Stunned;
    public override IEnumerator Execute(Enemy enemy)
    {
        yield break;
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_General_Animation((int)enemyStates);
    }
}
