using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Mission Listing", menuName = "Hub/Mission Listing")]
public class SO_MissionListing : ScriptableObject
{
    [field: SerializeField] public List<SO_MissionData> Missions { get; private set; }
}