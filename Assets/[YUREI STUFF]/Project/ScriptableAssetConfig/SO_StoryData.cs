using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "Story Data", menuName = "Miscellaneous/Story Data")]
public class SO_StoryData : ScriptableObject
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public List<Requirement> Requirements { get; private set; }
    [field: SerializeField] public Replayability Replayability { get; private set; }
    [field: SerializeField] public PlayAt PlayAt { get; private set; } = PlayAt.Independent;

    [field: SerializeField] public SO_Dialogue DialogueEvent { get; private set; }
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
    public bool HasSeen() //Seen for the entire progress
    {
        if (Observer.StoryObserver.Completion.Contains(this)) return true;
        return false;
    }
    public bool TempSeen()//Seen for every once entering the hub each time, one time
    {
        if (Observer.StoryObserver.TempCompletion.Contains(this)) return true;
        return false;
    }
    public void StartStoryDialogue()
    {
        if (!CheckRequirement()) return;
        Debug.Log(DialogueEvent.eventName);
        switch (Replayability)
        {
            case Replayability.Once:
                RaiseCutscene.Raise(DialogueEvent);
                if(!HasSeen()) Observer.StoryObserver.AddToCompletion(this);
                return;
            case Replayability.OncePerSession:
                RaiseCutscene.Raise(DialogueEvent);
                if(!TempSeen()) Observer.StoryObserver.AddToTemp(this);
                return;
            case Replayability.Repeatable:
                RaiseCutscene.Raise(DialogueEvent);
                return;
        }

    }
    
}
public enum Replayability
{
    Once, OncePerSession, Repeatable
}
public enum PlayAt
{
    EnteringHub,QuestEmbark, Independent
}