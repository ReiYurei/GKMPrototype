using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class StageManagerComponent : MonoBehaviour
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_StoryData ExterminateDialogue { get; private set; }
    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeOverallStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExterminationStartEvent { get; private set; }

    [Header("Canvas")]
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
    public void OnStageEnter() //Listen to Event
    {
        EnqueueStageEvents();
    }
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
        StartCoroutine(PlayQueuedEvents());
    }
    private void EnqueueStageEvents()
    {
       if(_storyQueue == null) _storyQueue = new Queue<SO_StoryData>();
       foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
       {
           if (story.PlayAt != PlayAt.EnteringStage) continue;
           if (story.HasSeen() || story.TempSeen()) continue;
           _storyQueue.Enqueue(story);
       }
       PlayEvents();
    }
    private void PlayEvents()
    {

        if (_storyQueue.Count <= 0)
        {

            if (ExterminateDialogue == null)
            {
                ExterminationStart();
                return;
            }
            if (ExterminateDialogue.TempSeen() || ExterminateDialogue.HasSeen())
            {
                ExterminationStart();
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

    }
    IEnumerator PlayQueuedEvents()
    {
        yield return new WaitForSeconds(0.25f);
        PlayEvents();
    }
}
