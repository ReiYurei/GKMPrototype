using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

public abstract class SO_Base_Attack_Fixed : SO_Enemy_Substate
{
#if UNITY_EDITOR


    string clipName;
    [ShowInInspector] public string ClipName => clipName;


    public AnimatorOverrideController controller;
    public AnimatorController baseController;
    AnimationClip overriddenClip;
    [ShowInInspector] public AnimationClip OverriddenClip => overriddenClip; 

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



[CreateAssetMenu(fileName = "Moveset_Pr_Standing", menuName = "Enemy/Moveset/Projectile/Stomp")]
public class SO_Projectile_Attack_Standing : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        yield return base.Execute(enemy);
        
    }



}
