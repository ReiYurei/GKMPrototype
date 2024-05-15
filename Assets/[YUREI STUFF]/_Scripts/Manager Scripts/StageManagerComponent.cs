using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TriInspector;
using UnityEngine.Playables;

[RequireComponent(typeof(EventListenerComponent))]
public class StageManagerComponent : MonoBehaviour
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_StoryData ExterminateDialogue { get; private set; }
    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeOverallStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExterminateInitializeEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExterminationStartEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent StageClearEvent { get; private set; }

    private PlayAt _playState;
    [Header("Canvas")]
    [Header("Pre-Fight Canvas")]
    [SerializeField] private GameObject _exterminateCanvas;
    [SerializeField] private GameObject _exterminateScreen;
    [SerializeField] private Image _exterminateObject;
    [SerializeField] private TextMeshProUGUI _exterminateText;
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private float _speed;


    [Header("State")]
    [SerializeField] private ExterminateState _exterminateState;
    [SerializeField] private LoadingScreenState _loadingState;

    private Queue<SO_StoryData> _storyQueue;

    [Header("Spawn Point")]
    public Transform playerSpawnPoint;
    public Transform astralSpawnPoint;

    public Transform playerBulletHellPoint;
    public Transform astralBulletHellPoint;
    public bool SpawnPointReady()
    {
        if(playerSpawnPoint == null || astralSpawnPoint == null || playerBulletHellPoint == null || astralBulletHellPoint == null)
        {
            Debug.Log("<color=yellow>Spawn Point is not properly initialized!</color>");
            return false;
        }
        return true;
    }

    [TriInspector.Button("Debug Raise : Entering Stage")]
    public void OnStageEnter() //Listen to Event
    {
        _playState = PlayAt.EnteringStage;
        EnqueueStageEvents();
    }
    [TriInspector.Button("Debug Raise : End ofStage")]
    public void OnEnemyDeathAnimEnd() //Listen to Event
    {
        _playState = PlayAt.EndOfStage;
        EnqueueStageEvents();
    }



    [TriInspector.Button("Debug Raise : Custom Event")]
    public void OnStageClear(SO_VoidGameEvent voidEvent)
    {
        voidEvent.Raise();
    }
    [TriInspector.Button("Debug Raise : Initialize Exterminate")]
    public void ExterminationStart()
    {
        StartCoroutine(InitializeExtermination());
    }
    public IEnumerator InitializeExtermination()
    {
        yield return StartCoroutine(ExterminateScreen());
        ExterminationStartEvent.Raise();
        IEnumerator ExterminateScreen()
        {
            _exterminateCanvas.SetActive(true);
            _exterminateScreen.transform.localScale = new Vector3(10,10,10);
            var startScale = new Vector3(10, 10, 10);
            var targetScale = Vector3.one;
            ChangeStateEvent.Raise(_loadingState);
            bool executeOnce = false;
            float timeToScale = 5f;
            float time = 0f;
            while (_exterminateScreen.transform.localScale != targetScale)
            {
         
                time += Time.deltaTime;
                if (time > timeToScale * 0.8f && !executeOnce)
                {
                    executeOnce = true;
                    StartCoroutine(LocalFadeOut(time));
                }
                _speed = _speedCurve.Evaluate(time / timeToScale);
                _exterminateScreen.transform.localScale = Vector3.Lerp(startScale, targetScale, _speed);
                yield return null;
            }
            _exterminateScreen.SetActive(false);

        }
        IEnumerator LocalFadeOut(float time)
        {
            _exterminateObject.color = Color.white;
            _exterminateText.color = Color.black;
            var transparent = Color.clear;
            var white = Color.white;
            _exterminateText.gameObject.SetActive(true);
            _exterminateObject.gameObject.SetActive(true);
            float _time = 0f;
            float _fadeSpeed;
            float _timeToFade = time;

            while (_exterminateObject.color != transparent)
            {
                _time += Time.deltaTime;
                _fadeSpeed = _speedCurve.Evaluate(_time / _timeToFade);
                _exterminateObject.color = Color.Lerp(white, transparent, _fadeSpeed);
                _exterminateText.color = Color.Lerp(Color.black, transparent, _fadeSpeed);
                yield return null;
            }
            _exterminateObject.gameObject.SetActive(false);
            _exterminateText.gameObject.SetActive(false);

        }

    }
    public void OnDialogueEnd() //Listen to Event
    {
        StartCoroutine(PlayQueuedStageEvents());
    }

    private void EnqueueStageEvents()
    {
        if (_storyQueue == null) _storyQueue = new Queue<SO_StoryData>();
        switch (_playState)
        {
            case PlayAt.EnteringStage:
                foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
                {
                    if (story.PlayAt != PlayAt.EnteringStage) continue;
                    if (story.HasSeen() || story.TempSeen()) continue;
                    if (!story.CheckRequirement()) continue;
                    _storyQueue.Enqueue(story);
                }
                break;
            case PlayAt.EndOfStage:
                foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
                {
                    if (story.PlayAt != PlayAt.EndOfStage) continue;
                    if (story.HasSeen() || story.TempSeen()) continue;
                    if (!story.CheckRequirement()) continue;
                    _storyQueue.Enqueue(story);
                }
                break;
 
        }
        PlayStageEvents();

    }

    private void PlayStageEvents()
    {
        switch (_playState)
        {
            case PlayAt.EnteringStage:
                if (_storyQueue.Count <= 0)
                {

                    if (ExterminateDialogue == null)
                    {
                        ExterminateInitializeEvent.Raise();

                        return;
                    }
                    if (ExterminateDialogue.TempSeen() || ExterminateDialogue.HasSeen())
                    {
                        ExterminateInitializeEvent.Raise();
                        return;
                    }

                    ExterminateDialogue.StartStoryDialogue();
                    return;
                }
                foreach (SO_StoryData story in _storyQueue)
                {
                    _storyQueue.Dequeue().StartStoryDialogue();
                    return;
                }
                break;
            case PlayAt.EndOfStage:
                if (_storyQueue.Count <= 0)
                {
                    StageClearEvent.Raise();
                    return;
                }
                foreach (SO_StoryData story in _storyQueue)
                {
                    _storyQueue.Dequeue().StartStoryDialogue();
                    return;
                }
                break;

        }

    }
    IEnumerator PlayQueuedStageEvents()
    {
        yield return new WaitForSeconds(0.25f);
        PlayStageEvents();
    }
}
