using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Move", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Moving")]
public class SO_Walking : SO_Enemy_Substate
{
    [SerializeField] private string _name = "Enemy_Move";
    public override void Execute()
    {
        Debug.Log("Walking");
    }
    public override string GetName()
    {
        return _name;
    }
}
