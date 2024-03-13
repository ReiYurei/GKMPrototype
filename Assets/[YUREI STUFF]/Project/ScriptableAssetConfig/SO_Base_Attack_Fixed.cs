using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;

#endif
using UnityEngine;

public abstract class SO_Base_Attack_Fixed : SO_Enemy_Substate
{
#if UNITY_EDITOR




    [Header("Overridden Clip Info")]
    public AnimatorOverrideController controller;
    public AnimatorController baseController;
    AnimationClip overriddenClip;
    [ShowInInspector] public AnimationClip OverriddenClip => overriddenClip;
    string clipName;
    [ShowInInspector] public string ClipName => clipName;

    private void OnValidate()
    {
       
            overriddenClip = GetAnimationClipByHash(controller, AnimationHash.Enemy_Attack((int)power, (int)range));
            clipName = overriddenClip != null ? overriddenClip.name : "Animation Clip Not Found";
    }

    private AnimationClip GetAnimationClipByHash(AnimatorOverrideController controller, int hash)
    {
    if (controller != null && baseController != null)
    {
        for (int i = 0; i < baseController.animationClips.Length; i++)
        {
            if (Animator.StringToHash(baseController.animationClips[i].name) == hash)
            {
                return (controller.animationClips[i]);
            }
        }        
    }
        return null;
    }
#endif
    [Header("Main Properties")]

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
    public override IEnumerator Execute(Enemy enemy)
    {
        enemy._status.NotifyAttacking(true);
        yield return new WaitUntil(() => enemy._status._isNextAttackReady == true);
        enemy._status.NotifyAttacking(false);

    }
}



[CreateAssetMenu(fileName = "Moveset_Teleport", menuName = "Enemy/Moveset/Melee/Teleport")]
public class SO_Melee_Attack_Teleport : SO_Base_Attack_Fixed
{

}



[CreateAssetMenu(fileName = "Moveset_Projectile_Standing", menuName = "Enemy/Moveset/Projectile/Stomp")]
public class SO_Projectile_Attack_Standing : SO_Base_Attack_Fixed
{
    public ProjectileSlot projectile;
    public override IEnumerator Execute(Enemy enemy)
    {
        yield return base.Execute(enemy);
        
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Projectile((int)projectile);
    }

}
