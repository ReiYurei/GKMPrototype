using UnityEngine;
using TriInspector;

[System.Serializable]
public class Quest
{
    [field: SerializeField] public QuestType QuestType { get; private set; }
    [field: SerializeField] public string QuestName { get; private set; }
    [field: SerializeField] public string ClientName { get; private set; }

    [field: SerializeField][field: TextArea(3, 15)] public string QuestDesc { get; private set; }
    [field: SerializeField]public bool Repeateable { get; private set; }


    [ShowInInspector][ShowIf(nameof(QuestType), QuestType.Elimination)]public Enemy EliminationTarget { get; private set; }
    [ShowInInspector][ShowIf(nameof(QuestType), QuestType.Elimination)]public int EliminationCount { get; private set; }

    [ShowInInspector][ShowIf(nameof(QuestType), QuestType.Mission)]public SO_MissionData ClearedMissionTarget { get; private set; }
    [ShowInInspector][ShowIf(nameof(QuestType), QuestType.Mission)]public int MinClearCount { get; private set; }

}
