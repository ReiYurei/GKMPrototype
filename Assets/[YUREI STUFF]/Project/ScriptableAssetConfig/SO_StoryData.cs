using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "Story Data", menuName = "Hub/Story Data")]
public class SO_StoryData : ScriptableObject
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public List<Requirement> Requirements { get; private set; }
    [field: SerializeField] public bool Repeatable { get; private set; }
    [field: SerializeField] public SO_StoryDialogue DialogueEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent RaiseCutscene { get; private set; }
    [field: SerializeField] public bool MinimumRequirement { get; private set; }
    [SerializeField][ShowIf(nameof(MinimumRequirement), true)] private int MinCompleted;
    private bool[] fulfilledRequirement;
    private int completedCount;
    private bool CheckRequirement()
    {
        completedCount = 0;
        if(Requirements.Count <=0) 
        {
            return true;
        }
        fulfilledRequirement = new bool[Requirements.Count];
        for (int i = 0; i < Requirements.Count; i++)
        {
            fulfilledRequirement[i] = Requirements[i].CheckRequirement(Observer);
        }
        for (int i = 0; i < fulfilledRequirement.Length; i++)
        {
            if (!fulfilledRequirement[i] && MinimumRequirement) continue;
            else if (!fulfilledRequirement[i] && !MinimumRequirement) return false;
            completedCount++;
        }
        return (completedCount >= MinCompleted && MinimumRequirement);
    }
    public bool HasSeen()
    {
        if (Observer.StoryObserver.Completion.Contains(this)) return true;
        return false;
    }
    public bool TempSeen()
    {
        if (Observer.StoryObserver.TempCompletion.Contains(this)) return true;
        return false;
    }
    public void StartStoryDialogue()
    {
        if (!CheckRequirement()) return;
        if (Repeatable) 
        {
            RaiseCutscene.Raise(DialogueEvent);
            Observer.StoryObserver.AddToTemp(this);
            return; 
        }

        RaiseCutscene.Raise(DialogueEvent);
        Observer.StoryObserver.AddToCompletion(this);
    }
}
