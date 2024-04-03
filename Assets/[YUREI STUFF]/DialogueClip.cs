using UnityEngine;
using UnityEngine.Playables;

public class DialogueClip : PlayableAsset
{
    public bool pauseAtEnd;
    public bool isInterruptable;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour behaviour = playable.GetBehaviour();
        behaviour.pauseAtEnd = pauseAtEnd;
        behaviour.isInterruptable = isInterruptable;
        return playable;
    }
}