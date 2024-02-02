using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Taunt",menuName = "Enemy/Enemy Behaviour/Enemy Substate/Taunt")]
public class SO_Taunt : SO_Enemy_Substate
{
    public AnimationClip animation;
    public override void Execute()
    {
        Debug.Log("Taunt");
    }
}


