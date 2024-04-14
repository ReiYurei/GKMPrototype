using UnityEngine;
using TriInspector;

[CreateAssetMenu(fileName = "Money Reward Data", menuName = "Miscellaneous/Quest/Quest Reward/Money")]

public class SO_MoneyReward : BaseQuestReward
{
    [field: SerializeField]public int Amount { get; private set; }
    public override void ClaimReward( )
    {
        Debug.Log("Money Collected!");

    }
}
