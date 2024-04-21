using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Mission Listing", menuName = "Miscellaneous/Mission/Mission Listing")]
public class SO_MissionListing : ScriptableObject
{
    [field: SerializeField] public List<SO_MissionData> Missions { get; private set; }
    public void ResetValue()
    {
        Missions.Clear();
    }
    public void InitalizeListingData()
    {
        Missions = new List<SO_MissionData>();
    }
}