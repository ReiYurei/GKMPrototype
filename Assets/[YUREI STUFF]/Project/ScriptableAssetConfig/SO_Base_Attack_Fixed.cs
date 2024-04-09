using System;
using System.Collections;
using TriInspector;

#if UNITY_EDITOR
using UnityEditor.Animations;
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
        if (this is SO_Projectile_Attack_Standing)
        {
            _isProjectile = true;
        }
        else
        {
            _isProjectile = false;
        }
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
    bool _isProjectile;
    [Space(15)]
    [Header("Main Properties")]
    [ShowIf(nameof(_isProjectile), true)] public ProjectileSlot projectile;
    [HideIf(nameof(_isProjectile), true)] public AttackPowerType power;
    [HideIf(nameof(_isProjectile), true)] public AttackRangeType range;
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
        enemy.StatusData.NotifyAttacking(true);
        yield return new WaitUntil(() => enemy.StatusData.IsNextAttackReady == true);
        enemy.StatusData.NotifyAttacking(false);

    }
}



[CreateAssetMenu(fileName = "Moveset_Teleport", menuName = "Enemy/Moveset/Melee/Teleport")]
public class SO_Melee_Attack_Teleport : SO_Base_Attack_Fixed
{

}
