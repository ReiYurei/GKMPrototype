using UnityEngine;
using TriInspector;
[System.Serializable]
public abstract class BaseQuestReward : ScriptableObject, IQuestReward
{
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    public abstract void ClaimReward(); //PASSING DATA/INVENTORY PARAMETER
}


public interface IQuestReward
{
    public void ClaimReward();
}