using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Stunned", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Stunned")]
public class SO_Stunned : SO_Enemy_Substate
{
    public override void Execute(Enemy enemy)
    {
        Debug.Log("Stunned");
    }
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Stunned;
    }
}
