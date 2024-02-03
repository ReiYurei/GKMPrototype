using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Taunt",menuName = "Enemy/Enemy Behaviour/Enemy Substate/Taunt")]
public class SO_Taunt : SO_Enemy_Substate
{
    [SerializeField]private string _name = "Enemy_Taunt";
    public override void Execute()
    {
        Debug.Log("Taunting");
    }
    public override string GetName()
    {
        return _name;
    }
}


