using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CutscenePlayer : MonoBehaviour
{
    public TimelineAsset timeline;
    public PlayableDirector director;
    public void Start()
    {
        director = GetComponent<PlayableDirector>();
    }
    [ContextMenu("Play")]
    public void TestPlay()
    {
        director.Play();
    }
    [ContextMenu("Pause")]
    public void TestPause()
    {
        director.Pause();
    }
    public void OnDialoguePause(GameObject data)
    {
        Debug.Log("Pause");
        director.Pause();
    }
    [ContextMenu("Resume")]
    public void TestResume()
    {
        Debug.Log("Resume");
        director.Resume();
    }
    [ContextMenu("MoveTimeline")]

    public void MoveTimeline()
    {
        foreach (var track in timeline.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                clip.clipIn += 10f;
            }
        }
    }
}
[TrackBindingType(typeof(SO_Story_Dialogue))]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    public SO_ParameterGameEvent pauseEvent;
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueMixer>.Create(graph, inputCount);
    }
}
public class DialogueMixer : PlayableBehaviour
{
    private int inputCount;
    private bool endOfClip;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        inputCount = playable.GetInputCount(); //get the number of all clips on this track

    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SO_Story_Dialogue data = playerData as SO_Story_Dialogue;
        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<DialogueBehaviour> inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
            float inputWeight = playable.GetInputWeight(i);
            if(inputWeight > 0f && !endOfClip)
            {
                Debug.Log(data.dialogue[i].SpeechText);
                DialogueBehaviour input = inputPlayable.GetBehaviour();
                Debug.Log($"Input Index {i}, Weight{inputWeight}");
                Debug.Log($"Is Intteruptable : {input.isInterruptable}, Pause At End : {input.pauseAtEnd}" );
            }
            
        }
    }
 
}

public class DialogueBehaviour : PlayableBehaviour
{
    public bool pauseAtEnd;
    public bool isInterruptable;
}
