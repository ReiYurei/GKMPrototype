using UnityEngine;
using TriInspector;
[System.Serializable]
public abstract class BaseQuestReward : ScriptableObject, IQuestReward
{
    public abstract void ClaimReward(); //PASSING DATA/INVENTORY PARAMETER
}


public interface IQuestReward
{
    public void ClaimReward();
}