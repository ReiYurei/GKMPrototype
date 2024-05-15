using UnityEngine;

[CreateAssetMenu(fileName = "Item Reward Data", menuName = "Miscellaneous/Quest/Quest Reward/Item")]
public class SO_ItemReward : BaseQuestReward
{
    [field: SerializeField]public BaseItem Item { get;private set; }
    public override void ClaimReward()
    {
        Debug.Log("Item Collected!");
    }
}
