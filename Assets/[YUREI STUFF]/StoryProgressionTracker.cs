using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class StoryProgressionTracker : MonoBehaviour
{
    [SerializeField]private int progress = 0;
    [field: SerializeField] public List<SO_Story> Story { get; private set; }
    public HashSet<SO_Story> StoryHash { get; private set; }
    private void Start()
    {
        progress = 0; //Initialization
        StoryHash = new HashSet<SO_Story>();
    }
    public void CheckProgression()
    {
        if (!Story[progress].CheckRequirement()) return;
        ProgressIncrease();
        StoryHash.Add(Story[progress]);
    }
    private void ProgressIncrease()
    {
        if (progress >= Story.Count - 1) return;
        progress++;
    }
}
