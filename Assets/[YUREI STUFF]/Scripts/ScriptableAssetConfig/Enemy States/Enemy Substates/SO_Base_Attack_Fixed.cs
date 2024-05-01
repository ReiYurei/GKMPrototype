using System;
using System.Collections;
using TriInspector;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

public abstract class SO_Base_Attack_Fixed : SO_Enemy_Substate
{


    [Space(15)]
    [Header("Main Properties")]

    protected Vector3 target;
    public int motionValue;
    protected int index = 0;
    public SO_PlayerInfo playerInfo;


}



[CreateAssetMenu(fileName = "Moveset_Teleport", menuName = "Enemy/Moveset/Melee/Teleport")]
public class SO_Melee_Attack_Teleport : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        throw new NotImplementedException();
    }

    public override int GetAnimation()
    {
        throw new NotImplementedException();
    }
}
