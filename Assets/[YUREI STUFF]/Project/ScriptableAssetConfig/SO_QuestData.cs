using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Hub/Quest Data")]
public class SO_QuestData : ScriptableObject
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public List<Requirement> RequirementsToBeListed { get; private set; }

    public Quest QuestInfo;
    [field: SerializeField] public List<BaseQuestReward> Rewards { get; private set; }
    [SerializeField] private bool _minimumRequirement;
    [SerializeField][ShowIf(nameof(_minimumRequirement), true)] private int _minCompleted;
    private bool[] fulfilledRequirement;
    private int completedCount;
    public bool CheckRequirement()
    {
        completedCount = 0;
        if (RequirementsToBeListed.Count <= 0)
        {
            return true;
        }
        fulfilledRequirement = new bool[RequirementsToBeListed.Count];
        for (int i = 0; i < RequirementsToBeListed.Count; i++)
        {
            fulfilledRequirement[i] = RequirementsToBeListed[i].CheckRequirement(Observer);
        }
        for (int i = 0; i < fulfilledRequirement.Length; i++)
        {
            if (!fulfilledRequirement[i] && _minimumRequirement) continue;
            else if (!fulfilledRequirement[i] && !_minimumRequirement) return false;
            completedCount++;
        }
        return (completedCount >= _minCompleted && _minimumRequirement);
    }
    public void ClaimReward()
    {
        if (Rewards.Count <= 0) return;
        for (int i = 0; i < Rewards.Count; i++)
        {
            Rewards[i].ClaimReward();
        }
    }
}
public enum QuestType
{
    Elimination,Mission,Gathering
}