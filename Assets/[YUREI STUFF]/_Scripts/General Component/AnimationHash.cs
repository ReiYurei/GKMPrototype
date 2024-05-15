using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class AnimationHash
{
    public static readonly int Enemy_Idle = Animator.StringToHash("Enemy_Idle");
    public static readonly int Enemy_Death = Animator.StringToHash("Enemy_Death");
    public static readonly int Enemy_Move_Forward = Animator.StringToHash("Enemy_Move_Forward");
    public static readonly int Enemy_Move_F_Loop = Animator.StringToHash("Enemy_Move_F_Loop");
    public static readonly int Enemy_Move_F_Stop = Animator.StringToHash("Enemy_Move_F_Stop");
    public static readonly int Enemy_Move_Backward = Animator.StringToHash("Enemy_Move_Backward");
    public static readonly int Enemy_Move_B_Loop = Animator.StringToHash("Enemy_Move_B_Loop");
    public static readonly int Enemy_Move_B_Stop = Animator.StringToHash("Enemy_Move_B_Stop");
    public static readonly int Enemy_Scan = Animator.StringToHash("Enemy_Scan");
    public static readonly int Enemy_Stunned = Animator.StringToHash("Enemy_Stunned");
    public static readonly int Enemy_Taunt = Animator.StringToHash("Enemy_Taunt");
    public static readonly int Enemy_Raging = Animator.StringToHash("Enemy_Raging");
    public static readonly int Enemy_Flinched = Animator.StringToHash("Enemy_Flinched");



    public static readonly int Enemy_Weak_Close_Attack = Animator.StringToHash("Enemy_A_Weak_Close_Attack");
    public static readonly int Enemy_Weak_Midrange_Attack = Animator.StringToHash("Enemy_A_Weak_Midrange_Attack");
    public static readonly int Enemy_Weak_Long_Attack = Animator.StringToHash("Enemy_A_Weak_Long_Attack");

    public static readonly int Enemy_Medium_Close_Attack = Animator.StringToHash("Enemy_A_Medium_Close_Attack");
    public static readonly int Enemy_Medium_Midrange_Attack = Animator.StringToHash("Enemy_A_Medium_Midrange_Attack");
    public static readonly int Enemy_Medium_Long_Attack = Animator.StringToHash("Enemy_A_Medium_Long_Attack");

    public static readonly int Enemy_Strong_Close_Attack = Animator.StringToHash("Enemy_A_Strong_Close_Attack");
    public static readonly int Enemy_Strong_Midrange_Attack = Animator.StringToHash("Enemy_A_Strong_Midrange_Attack");
    public static readonly int Enemy_Strong_Long_Attack = Animator.StringToHash("Enemy_A_Strong_Long_Attack");

    public static readonly int Enemy_P_Projectile_1 = Animator.StringToHash("Enemy_P_Projectile_1");
    public static readonly int Enemy_P_Projectile_2 = Animator.StringToHash("Enemy_P_Projectile_2");
    public static readonly int Enemy_P_Projectile_3 = Animator.StringToHash("Enemy_P_Projectile_3");
    public static readonly int Enemy_P_Projectile_4 = Animator.StringToHash("Enemy_P_Projectile_4");
    public static readonly int Enemy_P_Projectile_5 = Animator.StringToHash("Enemy_P_Projectile_5");

    public static readonly int Enemy_BH_Center = Animator.StringToHash("Enemy_P_Projectile_5");
    public static readonly int Enemy_BH_Left = Animator.StringToHash("Enemy_BH_Left");
    public static readonly int Enemy_BH_Right = Animator.StringToHash("Enemy_BH_Right");


    // public static readonly int Enemy_P_Projectile_1_Loop = AnimatorComponent.StringToHash("Enemy_P_Projectile_1_Loop");
    // public static readonly int Enemy_P_Projectile_2_Loop = AnimatorComponent.StringToHash("Enemy_P_Projectile_2_Loop");
    // public static readonly int Enemy_P_Projectile_3_Loop = AnimatorComponent.StringToHash("Enemy_P_Projectile_3_Loop");
    // public static readonly int Enemy_P_Projectile_4_Loop = AnimatorComponent.StringToHash("Enemy_P_Projectile_4_Loop");
    // public static readonly int Enemy_P_Projectile_5_Loop = AnimatorComponent.StringToHash("Enemy_P_Projectile_5_Loop");

    private static Dictionary<int, int> enemyProjectileDict = new Dictionary<int, int>()
    {
        {(int)ProjectileSlot.Projectile_1, Enemy_P_Projectile_1},
        {(int)ProjectileSlot.Projectile_2, Enemy_P_Projectile_2},
        {(int)ProjectileSlot.Projectile_3, Enemy_P_Projectile_3},
        {(int)ProjectileSlot.Projectile_4, Enemy_P_Projectile_4},
        {(int)ProjectileSlot.Projectile_5, Enemy_P_Projectile_5},
    };

    private static Dictionary<int, int> enemyGeneralDict = new Dictionary<int, int>()
    {
        {(int)EnemyStates.Idle, Enemy_Idle},
        {(int)EnemyStates.Default, Enemy_Idle},
        {(int)EnemyStates.Taunt, Enemy_Taunt},
        {(int)EnemyStates.Stunned, Enemy_Stunned},
        {(int)EnemyStates.Break, Enemy_Flinched },
        {(int)EnemyStates.Flinched, Enemy_Flinched },
        {(int)EnemyStates.Enraged, Enemy_Raging },
        {(int)EnemyStates.Death, Enemy_Death },

    };
    private static Dictionary<(int,int), int> attackDict = new Dictionary<(int, int), int>()
    {
        {( (int)AttackPowerType.Weak,(int)AttackRangeType.Close )       ,Enemy_Weak_Close_Attack},
        {( (int)AttackPowerType.Weak,(int)AttackRangeType.Midrange)     ,Enemy_Weak_Midrange_Attack},
        {( (int)AttackPowerType.Weak,(int)AttackRangeType.Long)         ,Enemy_Weak_Long_Attack},
        {( (int)AttackPowerType.Medium,(int)AttackRangeType.Close)      ,Enemy_Medium_Close_Attack},
        {( (int)AttackPowerType.Medium, (int)AttackRangeType.Midrange)  ,Enemy_Weak_Midrange_Attack},
        {( (int)AttackPowerType.Medium,(int)AttackRangeType.Long)       ,Enemy_Medium_Long_Attack},
        {( (int)AttackPowerType.Strong,(int)AttackRangeType.Close)      ,Enemy_Strong_Close_Attack},
        {( (int)AttackPowerType.Strong, (int)AttackRangeType.Midrange)  ,Enemy_Strong_Midrange_Attack},
        {( (int)AttackPowerType.Strong,(int)AttackRangeType.Long)       ,Enemy_Strong_Long_Attack},

    };
    private static Dictionary<int, int> enemyBulletHellMovement = new Dictionary<int, int>()
    {
        {(int)EnemyBulletHellAnimation.Center, Enemy_BH_Center},
        {(int)EnemyBulletHellAnimation.Left, Enemy_BH_Left},
        {(int)EnemyBulletHellAnimation.Right, Enemy_BH_Right},

    };
    public static int Enemy_BulletHellPhase_Animation(int type)
    {
        if (enemyBulletHellMovement.ContainsKey(type))
        {
            return enemyBulletHellMovement[type];
        }
        else return Enemy_Idle;
    }
    public static int Enemy_Projectile(int type)
    {
        if (enemyProjectileDict.ContainsKey(type))
        {
            return enemyProjectileDict[type];
        }
        else return Enemy_Idle;
    }
    public static int Enemy_General_Animation(int states)
    {
        if (enemyGeneralDict.ContainsKey(states))
        {
            return enemyGeneralDict[states];
        }
        else return Enemy_Idle;
    }
    public static int Enemy_Attack(int power, int range)
    {
        if (attackDict.ContainsKey((power, range)))
        {
            return attackDict[(power, range)];
        }

        else return Enemy_Idle;

    }
}

