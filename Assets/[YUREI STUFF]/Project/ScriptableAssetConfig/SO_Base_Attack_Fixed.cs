using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SO_Base_Attack_Fixed : SO_Enemy_Substate
{
    public AttackPowerType power;
    public AttackRangeType range;
    protected Vector3 target;
    public int motionValue;
    protected int index = 0;
    public SO_PlayerInfo playerInfo;
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Attack((int)power, (int)range);
    }

}

[CreateAssetMenu(fileName = "Moveset_Teleport", menuName = "Enemy/Moveset/Melee/Teleport")]
public class SO_Melee_Attack_Teleport : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        float time = enemy._status.WaitTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            //teleport
        }
        yield break;
    }
}


    [CreateAssetMenu(fileName = "Moveset_Stomp", menuName = "Enemy/Moveset/Melee/Stomp")]
public class SO_Melee_Attack_Stomp : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        //jump whether its an arc or a simple up, and then movetoward the target directly
        float time = enemy._status.WaitTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
        }
        yield break;
    }



}

[CreateAssetMenu(fileName = "Moveset_Pr_Standing", menuName = "Enemy/Moveset/Projectile/Stomp")]
public class SO_Projectile_Attack_Standing : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        //jump whether its an arc or a simple up, and then movetoward the target directly
        float time = enemy._status.WaitTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
        }
        yield break;
    }



}
public enum AttackRangeType
{
    Default, Close, Midrange, Long
}

public enum AttackPowerType
{
    Default, Weak, Medium, Strong
}

public enum SubstateProgerss
{
    InProgress, Finished
}