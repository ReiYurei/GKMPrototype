using UnityEngine;
using TriInspector;

[CreateAssetMenu(fileName = "EXP Reward Data", menuName = "Miscellaneous/Quest/Quest Reward/EXP")]
public class SO_ExperienceReward : BaseQuestReward
{
    [ShowInInspector]public int Amount { get; private set; }
    public override void ClaimReward( )
    {
        Debug.Log("Exp Collected!");

    }
}
