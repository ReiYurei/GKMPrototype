using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Quest Listing", menuName = "Miscellaneous/Quest/Quest Listing")]
public class SO_QuestListing : ScriptableObject
{
    [field: SerializeField] public List<SO_QuestData> Quests {get;private set;}
    public void ResetValue()
    {
        Quests.Clear();
    }
    public void InitalizeListingData()
    {
        Quests = new List<SO_QuestData>();
    }
}
