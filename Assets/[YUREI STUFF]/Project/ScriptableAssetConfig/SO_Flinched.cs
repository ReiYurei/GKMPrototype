using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substates_Flinched", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Flinched")]

public class SO_Flinched : SO_Enemy_Substate
{
    public override void Execute(Enemy enemy)
    {
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Flinched;
    }
}
