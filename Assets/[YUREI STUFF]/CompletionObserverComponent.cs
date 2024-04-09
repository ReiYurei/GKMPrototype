using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[RequireComponent(typeof(EventListenerComponent))]
public class CompletionObserverComponent : MonoBehaviour 
{
    [InlineEditor][ShowInInspector][field: SerializeField] public SO_CompletionObserver Observer { get; private set; }

    private void OnDisable()
    {
        Observer.QuestObserver.ClearData();
        Observer.StoryObserver.ClearData();
        Observer.MissionObserver.ClearData();
        Observer.ResetAllValue();
    }
    public void OnEmbark()
    {
        Observer.QuestObserver.ClearTemp();
        Observer.StoryObserver.ClearTemp();
        Observer.MissionObserver.ClearTemp();
    }
    public void OnAssignedQuestComplete()
    {
        Observer.AssignedQuestComplete();
    }
    public void OnAssignedMissionComplete()
    {
        Observer.AssignedMissionComplete();
    }
    public void OnAssignedMissionFailed()
    {
        Observer.AssignedMissionFailed();
    }

}
