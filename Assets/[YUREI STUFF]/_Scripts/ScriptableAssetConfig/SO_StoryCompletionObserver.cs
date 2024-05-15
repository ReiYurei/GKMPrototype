using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Story Tracker", menuName = "Player/Progression Tracker/Story Tracker")]
public class SO_StoryCompletionObserver : BaseCompletionObserver<SO_StoryData>
{
    [field: SerializeField]public List<SO_StoryData> AllStoryData { get; private set; } = new List<SO_StoryData>();
}
