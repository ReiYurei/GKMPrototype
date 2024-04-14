using UnityEngine;

[CreateAssetMenu(fileName = "Completion Observer", menuName = "Player/Completion Observer")]
public class SO_CompletionObserver : ScriptableObject
{
    //Automate Initialization with Resources folder
    [field: SerializeField] public SO_QuestCompletionObserver QuestObserver { get; private set; }
    [field: SerializeField] public SO_MissionCompletionObserver MissionObserver { get; private set; }
    [field: SerializeField] public SO_StoryCompletionObserver StoryObserver { get; private set; }
    [field: SerializeField] public SO_QuestData AssignedQuest { get; private set; }
    [field: SerializeField] public SO_MissionData AssignedMission { get; private set; }
    public bool CheckStoryReqCompletion(SO_StoryData story)//Used by Requirement
    {
        return (StoryObserver != null && StoryObserver.Completion.Contains(story));
    }
    public bool CheckMissionReqCompletion(SO_MissionData mission)//Used by Requirement
    {
        return (MissionObserver != null && MissionObserver.Completion.Contains(mission));
    }
    public bool CheckQuesReqCompletion(SO_QuestData quest) //Used by Requirement
    {
        return(QuestObserver != null && QuestObserver.Completion.Contains(quest));
    }

    public void AssignQuest(SO_QuestData quest) //Used by Mission Listing
    {
        AssignedQuest = quest;
    }
    public void AssignMission(SO_MissionData mission) //Used by Mission Listing
    {
        AssignedMission = mission;
    }
    public void AssignedQuestComplete()
    {
        if (AssignedQuest == null) return;
        if (QuestObserver.Completion.Contains(AssignedQuest) || AssignedQuest.QuestInfo.Repeateable)
        {
            AssignedQuest.ClaimReward();
            AssignedQuest = null;
            return;
        }
        AssignedQuest.ClaimReward();
        QuestObserver.AddToCompletion(AssignedQuest);
        AssignedQuest = null;
    }
    public void AssignedMissionFailed()
    {
        if (AssignedMission == null) return;
        AssignedMission = null;
        return;
    }
    public void AssignedMissionComplete()
    {
        if (AssignedMission == null) return;
        if (MissionObserver.Completion.Contains(AssignedMission))
        {
            AssignedMission = null;
            return;
        }
        MissionObserver.AddToCompletion(AssignedMission);
        AssignedMission = null;

    }
    public void ResetAllValue()
    {
        AssignedQuest = null;
        AssignedMission = null;
    }
}