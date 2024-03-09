using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Taunt",menuName = "Enemy/Enemy Behaviour/Enemy Substate/Taunt")]
public class SO_Taunt : SO_Enemy_Substate
{
    [SerializeField] EnemyStates enemyStates = EnemyStates.Taunt;
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


