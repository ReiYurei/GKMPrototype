using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questName;
    [TextArea(1, 5)] public string questDesc;
}

[CreateAssetMenu(fileName = "Quest Data", menuName = "Quest/Quest Data")]
public class SO_QuestData : ScriptableObject
{
    public Quest quest;
    public List<BaseQuestReward> rewards;
    public bool isCompleted;
    public void ClaimReward()
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            rewards[i].GetReward();
        }
        isCompleted = true;
    }
}


[System.Serializable]
public abstract class BaseQuestReward : ScriptableObject, IQuestReward 
{
    public abstract void GetReward(); //PASSING DATA/INVENTORY PARAMETER
}
[CreateAssetMenu(fileName = "Story Progression Reward Data", menuName = "Quest/Quest Reward/Story")]
public class StoryProgressionReward : BaseQuestReward
{
    public override void GetReward()
    {
    }
}
[CreateAssetMenu(fileName = "Money Reward Data", menuName = "Quest/Quest Reward/Money")]

public class MoneyReward : BaseQuestReward
{
    public int amount;
    public override void GetReward()
    {
    }
}
[CreateAssetMenu(fileName = "EXP Reward Data", menuName = "Quest/Quest Reward/EXP")]

public class ExperienceReward : BaseQuestReward
{
    public int amount;
    public override void GetReward()
    {
    }
}

public interface IQuestReward
{
    public void GetReward();
}