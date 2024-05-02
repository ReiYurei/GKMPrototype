using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
#if UNITY_EDITOR

#endif

[System.Serializable]
public class Requirement //Used for event type of data (i.e _mission, _quest, Story)
{
    [field: SerializeField] public RequirementType Type { get;private set; }
    [SerializeField][ShowIf(nameof(Type), RequirementType.Quest)] private SO_QuestData _quest;
    [SerializeField][ShowIf(nameof(Type), RequirementType.AssignedQuest)] private SO_QuestData _assignedQuest;
    [SerializeField][ShowIf(nameof(Type), RequirementType.Mission)] private SO_MissionData _mission;
    [SerializeField][ShowIf(nameof(Type), RequirementType.AssignedMission)] private SO_MissionData _assignedMission;
    [SerializeField][ShowIf(nameof(Type), RequirementType.EnemyEncounter)] private GameObject _enemy;
    [SerializeField][ShowIf(nameof(Type), RequirementType.Item)] private SO_QuestItem _item;
    [SerializeField][ShowIf(nameof(Type), RequirementType.Item)] private SO_Inventory _inventory;
    [SerializeField][ShowIf(nameof(Type), RequirementType.Story)] private SO_StoryData _story;
    [SerializeField][ShowIf(nameof(Type), RequirementType.Fact)] private IntVariable _factVariable; 
    [SerializeField][ShowIf(nameof(Type), RequirementType.Fact)] private int _requiredAmount;
    
    public bool CheckRequirement(SO_CompletionObserver observer)
    {
        switch(Type)
        {
            case RequirementType.Quest:
                return (observer.CheckQuesReqCompletion(_quest));
            case RequirementType.Mission:
                return (observer.CheckMissionReqCompletion(_mission));
            case RequirementType.Item:
                return (_inventory.CheckQuestItem(_item));
            case RequirementType.Story:
                return(observer.CheckStoryReqCompletion(_story));
            case RequirementType.Fact:
                return (_factVariable.value >= _requiredAmount);
            case RequirementType.AssignedQuest:
                return (observer.AssignedQuest == _assignedQuest);
            case RequirementType.AssignedMission:
                return (observer.AssignedMission == _assignedMission);
            case RequirementType.EnemyEncounter:
                return (observer.AssignedMission.AstralEntity == _enemy);
        }
        return false;
    }
    //Required for Observer to Contain a Certain Data
    //Data can be a _quest, _mission, BaseItem, or Integer of certain stuff as been met
    //_quest and _mission access Observer Completion data, and check whether or not it's contain this quest/_mission
    //BaseItem access Inventory and check whether or not it contain this BaseItem
    //Integer check between 2 value whether or not the condition has been met, such as 3 times of doing x
}
public enum RequirementType
{
    Quest,Mission,Item,Story,Fact, AssignedMission, AssignedQuest, EnemyEncounter
}