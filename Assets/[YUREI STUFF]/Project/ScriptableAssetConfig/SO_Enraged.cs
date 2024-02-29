using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substates_Enraged", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Enraged")]

public class SO_Enraged : SO_Enemy_Substate
{
    public override void Execute(Enemy enemy)
    {
        Debug.Log("Raging");
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Raging;
    }
}
