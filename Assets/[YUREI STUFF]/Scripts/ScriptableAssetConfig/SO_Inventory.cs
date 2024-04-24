using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using YansaFork;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory Data")]
public class SO_Inventory : ScriptableObject
{
    private  HashSet<SO_QuestItem> QuestItemInventory;
    private HashSet<SO_Combo> LearnedSpells;

    [SerializeField] private List<SO_QuestItem> _viewQuestItemData;
    [SerializeField] private List<SO_Combo> _viewLearnedSpelltData;

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
    public bool CheckSpellItem(SO_Combo spell)
    {
        return (LearnedSpells.Contains(spell));
    }
    public bool debug;
    private void OnValidate()
    {
        if (debug)
        {
            if(QuestItemInventory.Count > 0)
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
    [Button("Clear Data")]
    public void ClearData()
    {
        QuestItemInventory.Clear();
        LearnedSpells.Clear();
        _viewLearnedSpelltData.Clear();
        _viewQuestItemData.Clear();
    }

}