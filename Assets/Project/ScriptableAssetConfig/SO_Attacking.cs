using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substates_Attack", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Attack")]
public class SO_Attacking : SO_Enemy_Substate
{
    public AttackPowerType power;
    public AttackRangeType range;
    public AnimationClip _clip;
    public override void Execute()
    {
        Debug.Log("Atacking");
    }
    public override string GetName()
    {
        return $"Enemy_{power}_{range}_Attack";
    }
}

public enum AttackRangeType
{
    Default, Close, Midrange, Long
}

public enum AttackPowerType
{
    Default,Weak, Medium ,Strong
}