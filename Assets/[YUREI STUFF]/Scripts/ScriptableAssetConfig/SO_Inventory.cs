using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory Data")]
public class SO_Inventory : ScriptableObject
{
    [ShowInInspector]public HashSet<SO_QuestItem> QuestItemInventory { get; private set; }
    [SerializeField] private List<SO_QuestItem> _viewData;
    [field: SerializeField] public int Gold { get; private set; }
    public void AddGold(int amount)
    {
        Gold += amount;
    }
    public void RemoveGold(int amount)
    {
        Gold -= amount;

    }
    public bool CheckQuestItem(SO_QuestItem item)
    {
        return (QuestItemInventory.Contains(item));
    }
    public bool debug;
    private void OnValidate()
    {
        if (debug)
        {
            foreach (SO_QuestItem item in QuestItemInventory)
            {
                _viewData.Add(item);
            }
        }
    }
    [Button("Clear Data")]
    public void ClearData()
    {
        foreach (SO_QuestItem item in QuestItemInventory)
        {
            QuestItemInventory.Remove(item);
        }
        _viewData.Clear();
    }

}