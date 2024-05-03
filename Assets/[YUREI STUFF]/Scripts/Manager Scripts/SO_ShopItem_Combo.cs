using UnityEngine;
using System.Collections.Generic;
using YansaFork;
using TriInspector;

[CreateAssetMenu(fileName = "Shop Item Combo", menuName = "Shop/Item/Combo")]
public class SO_ShopItem_Combo : ScriptableObject
{
    [field : SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_Combo SpellCombo { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public List<Requirement> RequirementsToBeListed { get; private set; }
    [field: SerializeField] public bool Sold { get; private set; }
    [SerializeField] private bool _minimumRequirementToList;
    [SerializeField][ShowIf(nameof(_minimumRequirementToList), true)] private int _minCompletedToList;
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
            if (!fulfilledRequirement[i] && _minimumRequirementToList) continue;
            else if (!fulfilledRequirement[i] && !_minimumRequirementToList) return false;
            completedCount++;
        }
        return (completedCount >= _minCompletedToList && _minimumRequirementToList);
    }
    public void Resale()
    {
        Sold = false;
    }
    public void SoldOut()
    {
        Sold = true;
    }
}