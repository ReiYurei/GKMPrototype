using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Idling", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Idling")]
public class SO_Idling : SO_Enemy_Substate
{
    [SerializeField] private string _name = "Enemy_Idling";
    public override void Execute()
    {
        Debug.Log("Idling");
    }
    public override string GetName()
    {
        return _name;
    }
}
