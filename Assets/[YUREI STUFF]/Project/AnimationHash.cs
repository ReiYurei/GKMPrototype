using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AnimationHash 
{
    public static readonly int Enemy_Idle = Animator.StringToHash("Enemy_Idle");
    public static readonly int Enemy_Death = Animator.StringToHash("Enemy_Death");
    public static readonly int Enemy_Move = Animator.StringToHash("Enemy_Move");
    public static readonly int Enemy_Scan = Animator.StringToHash("Enemy_Scan");
    public static readonly int Enemy_Stunned = Animator.StringToHash("Enemy_Stunned");
    public static readonly int Enemy_Taunt = Animator.StringToHash("Enemy_Taunt");
    public static readonly int Enemy_Raging = Animator.StringToHash("Enemy_Taunt");


    public static readonly int Enemy_Weak_Close_Attack = Animator.StringToHash("Enemy_Weak_Close_Attack");
    public static readonly int Enemy_Weak_Midrange_Attack = Animator.StringToHash("Enemy_Weak_Midrange_Attack");
    public static readonly int Enemy_Weak_Long_Attack = Animator.StringToHash("Enemy_Weak_Long_Attack");

    public static readonly int Enemy_Medium_Close_Attack = Animator.StringToHash("Enemy_Medium_Close_Attack");
    public static readonly int Enemy_Medium_Midrange_Attack = Animator.StringToHash("Enemy_Medium_Midrange_Attack");
    public static readonly int Enemy_Medium_Long_Attack = Animator.StringToHash("Enemy_Medium_Long_Attack");

    public static readonly int Enemy_Strong_Close_Attack = Animator.StringToHash("Enemy_Strong_Close_Attack");
    public static readonly int Enemy_Strong_Midrange_Attack = Animator.StringToHash("Enemy_Strong_Midrange_Attack");
    public static readonly int Enemy_Strong_Long_Attack = Animator.StringToHash("Enemy_Strong_Long_Attack");


    public static int Enemy_Attack(int power, int range)
    {
        //WEAK
        if (power == (int)AttackPowerType.Weak && range == (int)AttackRangeType.Close) 
        {
            return Enemy_Weak_Close_Attack;
        }
        else if (power == (int)AttackPowerType.Weak && range == (int)AttackRangeType.Midrange)
        {
            return Enemy_Weak_Midrange_Attack;

        }
        else if (power == (int)AttackPowerType.Weak && range == (int)AttackRangeType.Long)
        {
            return Enemy_Weak_Long_Attack;

        }
        
        //MEDIUM
        if (power == (int)AttackPowerType.Medium && range == (int)AttackRangeType.Close)
        {
            return Enemy_Medium_Close_Attack;
        }
        else if (power == (int)AttackPowerType.Medium && range == (int)AttackRangeType.Midrange)
        {
            return Enemy_Medium_Midrange_Attack;

        }
        else if (power == (int)AttackPowerType.Medium && range == (int)AttackRangeType.Long)
        {
            return Enemy_Medium_Long_Attack;

        }

        //STRONG
        if (power == (int)AttackPowerType.Strong && range == (int)AttackRangeType.Close)
        {
            return Enemy_Strong_Close_Attack;
        }
        else if (power == (int)AttackPowerType.Strong && range == (int)AttackRangeType.Midrange)
        {
            return Enemy_Strong_Midrange_Attack;

        }
        else if (power == (int)AttackPowerType.Strong && range == (int)AttackRangeType.Long)
        {
            return Enemy_Strong_Long_Attack;

        }
        else
        {
            return Enemy_Idle;
        }
        }
    }

