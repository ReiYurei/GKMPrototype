using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Move", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Moving")]
public class SO_Walking : SO_Enemy_Substate
{
    public override void Execute(Enemy enemy)
    {
        Debug.Log("Walking");
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Move;
    }
}
