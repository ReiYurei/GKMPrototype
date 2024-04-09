using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class HubCounterComponent : MonoBehaviour, IInteractable
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_QuestListing AllQuest { get; private set; }
    [field: SerializeField] public SO_QuestListing AvailableQuest { get; private set; }
    [field: SerializeField] public SO_MissionListing AllMission { get; private set; }
    [field: SerializeField] public SO_MissionListing AvailableMissions { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent InteractEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OpenListingEvent { get; private set; }
    [field: SerializeField] private Queue<SO_StoryData> _storyQueue;

    public void OnDisable()
    {
        AvailableQuest.ResetValue();
        AvailableMissions.ResetValue();
    }
    private void Start()
    {
        _storyQueue = new Queue<SO_StoryData>();
    }
    public void OnHubEnter()
    {
        CheckMissionListing();
        CheckQuestLising();
        EnqueueEvents();
        PlayEvents();
    }

    private void CheckMissionListing()
    {
        foreach (SO_MissionData mission in AllMission.Missions)
        {
            if (mission.CheckRequirement() && !AvailableMissions.Missions.Contains(mission))
                AvailableMissions.Missions.Add(mission);
        }
    }

    private void CheckQuestLising()
    {
        foreach (SO_QuestData quest in AllQuest.Quests)
        {
            if (quest.CheckRequirement() && !AvailableQuest.Quests.Contains(quest))
                AllQuest.Quests.Add(quest);
        }
    }

    private void PlayEvents()
    {
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
    public void OpenListing()
    {
        OpenListingEvent?.Raise();
    }
    public void OnDialogueEnd()
    {
        StartCoroutine(CheckStoryEventAgain());
        //Invoke Change Game State
    }
    IEnumerator CheckStoryEventAgain()
    {
        yield return new WaitForSeconds(0.25f);
        PlayEvents();
    }
    [ContextMenu("Interact")]
    public void OnInteract() //Check _quest
    {
        InteractEvent?.Raise();
        EnqueueEvents();

    }
}
