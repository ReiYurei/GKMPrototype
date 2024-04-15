using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class HubCounterComponent : MonoBehaviour, IInteractable
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }

    [field: Header("Quest")]
    [field: SerializeField] public SO_QuestListing AllQuest { get; private set; }
    [field: SerializeField] public SO_QuestListing AvailableQuest { get; private set; }

    [field: Header("Mission")]
    [field: SerializeField] public SO_MissionListing AllMission { get; private set; }
    [field: SerializeField] public SO_MissionListing AvailableMissions { get; private set; }

    [field: Header("Interaction Dialogue")]
    [field: SerializeField] public SO_StoryData EnteringHubDialogue { get; private set; }
    [field: SerializeField] public SO_StoryData InteractDialogue { get; private set; }
    [field: SerializeField] public SO_StoryData ExitDialogue { get; private set; }

    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent CompleteQuestEvent { get; private set; }
    [SerializeField] private HubState _hubState;
    private Queue<SO_StoryData> _storyQueue;
    private Queue<SO_StoryData> _hubStoryQueue;


    public void OnDisable()
    {
        AvailableQuest.ResetValue();
        AvailableMissions.ResetValue();
    }
    private void Start()
    {
        _storyQueue = new Queue<SO_StoryData>();
        _hubStoryQueue = new Queue<SO_StoryData>();

    }
    [ContextMenu("On Hub Enter")]
    public void OnHubEnter()
    {
        CheckMissionListing();
        CheckQuestLising();
        EnqueueHubEvents();
    }
    public void CheckQuestCompletion()
    {
        if (Observer.AssignedQuest == null)
        {
            EnqueueEvents();
            return;
        }
        if (!Observer.AssignedQuest.RequirementToClearFulfilled()) 
        {
            Debug.Log("Not Fulfilled");
            EnqueueEvents(); 
            return;
        }
        Debug.Log("Fulfilled");
        CheckMissionListing();
        CheckQuestLising();
        if (Observer.AssignedQuest.CompletionInteraction == null)
        {
            CompleteQuestEvent.Raise();
            EnqueueEvents();
            Debug.Log("No Completion Dialogue");
            return; }
        _storyQueue.Enqueue(Observer.AssignedQuest.CompletionInteraction);
        CompleteQuestEvent.Raise();
        EnqueueEvents();


    }
    public void OnExitListing()
    {
        if (ExitDialogue == null) return;
        ExitDialogue.StartStoryDialogue();
    }
    private void CheckMissionListing()
    {
        if (AvailableMissions.Missions == null) AvailableMissions.InitalizeListingData();

        foreach (SO_MissionData mission in AllMission.Missions)
        {
            if (mission.CheckRequirement() && !AvailableMissions.Missions.Contains(mission))
                AvailableMissions.Missions.Add(mission);

        }
    }

    private void CheckQuestLising()
    {
        if (AvailableQuest.Quests == null) AvailableQuest.InitalizeListingData();

        foreach (SO_QuestData quest in AllQuest.Quests)
        {
            if (quest.RequirementToListedFulfilled() && !AvailableQuest.Quests.Contains(quest))
                AvailableQuest.Quests.Add(quest);
        }
    }
    public void EnqueueEvents()
    {
        foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
        {
            if (story.PlayAt == PlayAt.EnteringHub) continue;
            if (story.HasSeen() || story.TempSeen()) continue;
            _storyQueue.Enqueue(story);
        }
        PlayEvents();

    }
    private void PlayEvents()
    {
        if (_storyQueue.Count <= 0)
        {
            CheckMissionListing();
            CheckQuestLising();
            if (InteractDialogue != null) InteractDialogue.StartStoryDialogue();
            return;
        }
        foreach (SO_StoryData story in _storyQueue)
        {
            _storyQueue.Dequeue().StartStoryDialogue();
            return;
        }

    }
    IEnumerator PlayQueuedEvents()
    {
        yield return new WaitForSeconds(0.25f);

        PlayEvents();
    }
    public void OnDialogueEnd()
    {
        StartCoroutine(PlayQueuedEvents());
    }


    //HubEvents
    public void EnqueueHubEvents()
    {
        foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
        {
            if (story.PlayAt != PlayAt.EnteringHub) continue;
            if (story.HasSeen() || story.TempSeen()) continue;
            _hubStoryQueue.Enqueue(story);
        }
        PlayHubEvents();
    }

    private void PlayHubEvents()
    {
        if (_hubStoryQueue.Count <= 0)
        {

            if (EnteringHubDialogue == null) 
            {
                ChangeStateEvent.Raise(_hubState);
                return;
            }
            if(EnteringHubDialogue.TempSeen() || EnteringHubDialogue.HasSeen()) return;
               EnteringHubDialogue.StartStoryDialogue();
            return;
        }
        foreach (SO_StoryData story in _hubStoryQueue)
        {
            _hubStoryQueue.Dequeue().StartStoryDialogue();
            return;
        }
    }
    IEnumerator PlayQueuedHubEvents()
    {
        yield return new WaitForSeconds(0.15f);

        PlayHubEvents();
    }
    public void OnHubDialogueEnd()
    {
        StartCoroutine(PlayQueuedHubEvents());
    }


    [ContextMenu("Interact")]
    public void OnInteract() //Check _quest
    {
       CheckQuestCompletion();
    }
}
