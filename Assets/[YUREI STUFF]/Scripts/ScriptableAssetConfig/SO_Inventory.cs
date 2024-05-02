using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using YansaFork;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory Data")]
public class SO_Inventory : ScriptableObject
{
    public HashSet<SO_QuestItem> QuestItemInventory { get; private set; }
    public HashSet<SO_Combo> LearnedSpells { get; private set; }

    [SerializeField] private List<SO_QuestItem> _viewQuestItemData;
    [SerializeField] private List<SO_Combo> _viewLearnedSpelltData;

    [field: SerializeField] public int Gold { get; private set; }
    public void AddGold(int amount)
    {
        int currentGold = Gold;
        Gold += amount;
        if (InventoryUIController.Instance == null) return;
        InventoryUIController.Instance.AnimateGold(currentGold, Gold, ComparatorType.LessThan);
    }
    public void ReduceGold(int amount)
    {
        int currentGold = Gold;
        Gold -= amount;
        if (InventoryUIController.Instance == null) return;
        InventoryUIController.Instance.AnimateGold(currentGold, Gold,ComparatorType.GreaterThan);
    }
    [Button("Debug Raise : Learn Spell ")]
    public void LearnSpell(SO_Combo spell)
    {
        LearnedSpells ??= new();
        LearnedSpells.Add(spell);
    }
    [Button("Debug Raise : Add Quest Item ")]
    public void AddQuestItem(SO_QuestItem item)
    {
        QuestItemInventory ??= new();
        QuestItemInventory.Add(item);
    }
    public bool CheckQuestItem(SO_QuestItem item)
    {
        return (QuestItemInventory.Contains(item));
    }
    public bool CheckSpellItem(SO_Combo spell)
    {
        return (LearnedSpells.Contains(spell));
    }
    public bool debug;
    [Button("Refresh")]
    public void Resfresh()
    {
        if (debug)
        {
      

            if (QuestItemInventory.Count > 0)
            {
                foreach (SO_QuestItem item in QuestItemInventory)
                {
                    if (!_viewQuestItemData.Contains(item)) _viewQuestItemData.Add(item);
                }
            }

            if(LearnedSpells.Count > 0)
            {
                foreach (SO_Combo spell in LearnedSpells)
                {
                    if (!_viewLearnedSpelltData.Contains(spell)) _viewLearnedSpelltData.Add(spell);
                }
            }
        }
    }
    public void InitializeData()
    {
        _viewQuestItemData ??= new();
        _viewLearnedSpelltData ??= new();
        QuestItemInventory ??= new();
        LearnedSpells ??= new();
    }
    [Button("Clear Data")]
    public void ClearData()
    {
        QuestItemInventory.Clear();
        LearnedSpells.Clear();
        _viewLearnedSpelltData.Clear();
        _viewQuestItemData.Clear();
    }

}