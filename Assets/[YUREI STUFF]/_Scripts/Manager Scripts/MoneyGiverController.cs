using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class MoneyGiverController : MonoBehaviour
{
    [field: SerializeField] public int GoldAmount { get; private set; }
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    bool _hasGiven;

    public void InitializeGolds(ScriptableObject data)
    {
        GoldAmount = 0;
        var story = data as SO_StoryData;
        if (story.DialogueRewards.Count == 0)
        {
            _hasGiven = true;
            return;
        }
        _hasGiven = false;

        for (int i = 0; i < story.DialogueRewards.Count; i++)
        {
            if (!(story.DialogueRewards[i] is SO_MoneyReward)) continue;
            var reward = story.DialogueRewards[i] as SO_MoneyReward;
            GoldAmount += reward.Amount;
        }
    }

    public void Give()
    {
        if (_hasGiven) return;
        Inventory.AddGold(GoldAmount);
        _hasGiven = true;

    }
    public void Clear()
    {
        if (!_hasGiven) return;
        GoldAmount = 0;
    }
}
