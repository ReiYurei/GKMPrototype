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
    [field: SerializeField] public SO_StoryData InteractDialogue { get; private set; }
    [field: SerializeField] public SO_StoryData ExitDialogue { get; private set; }

    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent CompleteQuestEvent { get; private set; }

    private Queue<SO_StoryData> _storyQueue;

    public void OnDisable()
    {
        AvailableQuest.ResetValue();
        AvailableMissions.ResetValue();
    }
    private void Start()
    {
        _storyQueue = new Queue<SO_StoryData>();
    }
    [ContextMenu("On Hub Enter")]
    public void OnHubEnter()
    {
        CheckMissionListing();
        CheckQuestLising();
        EnqueueEvents();
    }
    public void CheckQuestCompletion()
    {
        if (Observer.AssignedQuest == null) return;
        if (!Observer.AssignedQuest.RequirementFulfilled()) 
        {
            PlayEvents();
            return;
        }
        CheckMissionListing();
        CheckQuestLising();
        CompleteQuestEvent.Raise();
        _storyQueue.Enqueue(Observer.AssignedQuest.CompletionInteraction);
        PlayEvents();
    }
    public void OnExitListing()
    {
        Debug.Log("Exit Listing");
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
            if (quest.RequirementFulfilled() && !AvailableQuest.Quests.Contains(quest))
                AvailableQuest.Quests.Add(quest);
        }
    }

    private void PlayEvents()
    {
        if(_storyQueue.Count <= 0)
        {
            if(InteractDialogue != null) InteractDialogue.StartStoryDialogue();
            return;
        }
        foreach (SO_StoryData story in _storyQueue)
        {
            _storyQueue.Dequeue().StartStoryDialogue();
            return;
        }

    }

    public void EnqueueEvents()
    {
        foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
        {
            if (story.HasSeen() || story.TempSeen()) continue;
            _storyQueue.Enqueue(story);
        }
        PlayEvents();

    }
    public void OnDialogueEnd()
    {
        Debug.Log("Dialogue End");
        StartCoroutine(PlayQueuedEvents());
        //Invoke Change Game State
    }
    IEnumerator PlayQueuedEvents()
    {
        yield return new WaitForSeconds(0.25f);
        PlayEvents();
    }
    [ContextMenu("Interact")]
    public void OnInteract() //Check _quest
    {
       CheckQuestCompletion();
       EnqueueEvents();

    }
}
