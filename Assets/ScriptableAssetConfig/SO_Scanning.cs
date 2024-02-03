using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Scanning", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Scanning")]
public class SO_Scanning : SO_Enemy_Substate
{
    [SerializeField] private string _name = "Enemy_Scanning";
    public override void Execute()
    {
        Debug.Log("Scanning");
    }
    public override string GetName()
    {
        return _name;
    }
}

