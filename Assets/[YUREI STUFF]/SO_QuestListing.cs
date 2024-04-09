using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Quest Listing", menuName = "Hub/Quest Listing")]
public class SO_QuestListing : ScriptableObject
{
    [field: SerializeField] public List<SO_QuestData> Quests {get;private set;}
}
