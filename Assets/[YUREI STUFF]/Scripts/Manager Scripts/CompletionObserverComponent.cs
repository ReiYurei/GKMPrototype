using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[RequireComponent(typeof(EventListenerComponent))]
public class CompletionObserverComponent : MonoBehaviour 
{
    [InlineEditor][ShowInInspector][field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    public static CompletionObserverComponent Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void OnDisable()
    {
        Observer.QuestObserver.ClearData();
        Observer.StoryObserver.ClearData();
        Observer.MissionObserver.ClearData();
        Observer.ResetAllValue();
    }
    public void OnReturnToTitle()
    {
        Observer.QuestObserver.ClearData();
        Observer.StoryObserver.ClearData();
        Observer.MissionObserver.ClearData();
        Observer.QuestObserver.ClearTemp();
        Observer.StoryObserver.ClearTemp();
        Observer.MissionObserver.ClearTemp();
        Observer.ResetAllValue();
    }
    public void OnEmbark() //Listen to Event
    {
        Observer.QuestObserver.ClearTemp();
        Observer.StoryObserver.ClearTemp();
        Observer.MissionObserver.ClearTemp();
    }
    public void OnAssignedQuestComplete() //Listen to Event
    {
        Observer.AssignedQuestComplete();
    }
    public void OnAssignedMissionComplete() //Listen to Event
    {
        Observer.AssignedMissionComplete();
    }
    public void OnAssignedMissionFailed()//Listen to Event
    {
        Observer.AssignedMissionFailed();
    }

}
