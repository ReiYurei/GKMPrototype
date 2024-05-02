using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using UnityEditor;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "Mission Data", menuName = "Miscellaneous/Mission/Mission Data")]
public class SO_MissionData : ScriptableObject
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public List<Requirement> RequirementsToBeListed { get; private set; }
    [field: SerializeField] public string MissionName { get; private set; }
    [field: SerializeField] public GameObject AstralEntity { get; private set; }
    [field: SerializeField] public bool Repeatable { get; private set; } = true;
    [field: SerializeField] public StageInfo StageInfo { get;private set; } //need a proper initialization class data
    [field: SerializeField] public List<BaseQuestReward> Rewards { get; private set; }
    [field: SerializeField] public bool MinimumRequirement { get; private set; }
    [SerializeField][ShowIf(nameof(MinimumRequirement), true)] private int MinCompleted;
    private bool[] fulfilledRequirement;
    private int completedCount;

    public bool RequirementToListedFulfilled()
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
            if (MinimumRequirement)
            {
                if (!fulfilledRequirement[i]) continue;
                completedCount++;

            }
            else if (!fulfilledRequirement[i]) return false;
        }

        if (MinimumRequirement) return (completedCount >= MinCompleted && MinimumRequirement);
        return true;
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
[System.Serializable]
public class StageInfo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public StageName SceneName { get; private set; }

    [field: SerializeField] public string Description { get; private set; }

}
