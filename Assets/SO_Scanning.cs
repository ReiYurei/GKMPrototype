using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Scanning", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Scanning")]
public class SO_Scanning : SO_Enemy_Substate
{
    //public AnimationClip animation;
    public override void Execute()
    {
        Debug.Log("Scanning");
    }
}

