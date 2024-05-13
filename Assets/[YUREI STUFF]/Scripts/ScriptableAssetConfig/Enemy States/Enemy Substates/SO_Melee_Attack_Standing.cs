using System.Collections;
using UnityEngine;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
[CreateAssetMenu(fileName = "Moveset_Melee_Fixed", menuName = "Enemy/Moveset/Melee/Fixed")]
public class SO_Melee_Attack_Base : SO_Base_Attack_Fixed
{
    public AttackPowerType power;
    public AttackRangeType range;
    public bool isGuardable;
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
    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Attack((int)power, (int)range);
    }
    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.StatusData.NotifyAttacking(true);
        enemy.StatusData.SetMotionValue(motionValue);
        enemy.StatusData.isGuardable = isGuardable;
        yield return new WaitUntil(() => enemy.StatusData.IsNextAttackReady == true);
        enemy.StatusData.NotifyAttacking(false);

    }
}
