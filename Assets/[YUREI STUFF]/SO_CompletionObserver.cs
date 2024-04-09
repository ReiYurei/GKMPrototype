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
    public bool CheckStoryCompletion(SO_StoryData story)
    {
        return (StoryObserver != null && StoryObserver.Completion.Contains(story));
    }
    public bool CheckMissionCompletion(SO_MissionData mission)
    {
        return (MissionObserver != null && MissionObserver.Completion.Contains(mission));
    }
    public bool CheckQuestCompletion(SO_QuestData quest)
    {
        return(QuestObserver != null && QuestObserver.Completion.Contains(quest));
    }
    public void AssignQuest(SO_QuestData quest)
    {
        AssignedQuest = quest;
    }
    public void AssignMission(SO_MissionData mission)
    {
        AssignedMission = mission;
    }
    public void AssignedQuestComplete()
    {
        if (AssignedQuest == null) return;
        if (QuestObserver.Completion.Contains(AssignedQuest) || AssignedQuest.QuestInfo.Repeateable)
        {
            AssignedQuest = null;
            return;
        }
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
}