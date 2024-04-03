using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Story Data", menuName = "Story/Story")]
public class SO_Story : ScriptableObject
{
    [field: SerializeField] public List<SO_QuestData> Requirement { get; private set; }

    [SerializeField] private bool minimalRequirement;
    [SerializeField] private int minCompleted;
    private bool[] fulfilledRequirement;
    private int completedCount;
    public bool CheckRequirement()
    {
        completedCount = 0;
        fulfilledRequirement = new bool[Requirement.Count];
        for (int i = 0; i < Requirement.Count; i++)
        {
            fulfilledRequirement[i] = Requirement[i].isCompleted;
        }
        for (int i = 0; i < fulfilledRequirement.Length; i++)
        {
            if (!fulfilledRequirement[i] && minimalRequirement) continue;
            else if (!fulfilledRequirement[i] && !minimalRequirement) return false;
            completedCount++;
        }
        return (completedCount >= minCompleted && minimalRequirement);
    }
}
