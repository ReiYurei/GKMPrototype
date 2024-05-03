public enum EnemyStates
{
    Default, Normal, Enraged ,Raging, Break, Stunned, Taunt,Flinched,Death, Moving, Idle, OutOfRange, InRange
}
public enum ComparatorType
{
    Equal, Inequal, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
}
public enum BooleanComparatorType
{
    Equal, Inequal
}
public enum AttackRangeType
{
    Default, Close, Midrange, Long
}

public enum AttackPowerType
{
    Default, Weak, Medium, Strong
}

public enum ProjectileSlot
{
    Default, Projectile_1, Projectile_2, Projectile_3, Projectile_4, Projectile_5,
}
public enum GameState
{
    Overworld, Gameplay, Cutscene
}
[System.Serializable]
public enum CharacterID
{
    None, MC, Dummy1, Dummy2
}
[System.Serializable]
public enum ExpressionID
{
    Default, Neutral, Happy, Laugh, Smile ,Angry, Despised, Sad, Pensive, Shocked, Confused
}
[System.Serializable]
public enum ActiveTalker
{
    None,Left,Right,Both
}
public enum SceneName
{
    MainMenu_Scene, Hub_Scene, Stage_MistyForest, Stage_
}
public enum StageName
{
    Stage_Tutorial,Stage_MistyForest, Stage_Swamp, Stage_Forest, Stage_MountAndHills, Stage_Village
}

public enum CompletionMark 
{
    Failed, Clear, None
}
public enum Replayability
{
    Once, OncePerSession, Repeatable
}
public enum PlayAt
{
    EnteringHub, QuestEmbark, EnteringStage, Independent, HubCounterInteraction, EndOfStage, Tutorial
}