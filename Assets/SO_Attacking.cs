using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Attacking", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Attack")]
public class SO_Attacking : SO_Enemy_Substate
{
    public Animator animator;
    public override void Execute()
    {
        Debug.Log("Atacking");
    }
}