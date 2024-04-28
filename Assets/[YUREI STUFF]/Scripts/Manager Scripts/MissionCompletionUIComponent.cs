using System.Collections;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class MissionCompletionUIComponent : MonoBehaviour
{
    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent LoadHubEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ResultScreenEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ResultFailedEvent { get; private set; }

    [Header("Fade Canvas")]
    [SerializeField] private GameObject _fadeCanvas;
    [SerializeField] private Image _halfFade;

    [Header("State")]
    [SerializeField] private LoadingScreenState _loadingState;

    public enum CompletionMark { Failed, Clear}

    [Button("Debug Raise : Mission Failed")]
    public void OnMissionFailed() //Played on Abandon/Player Death
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(MissionFailed());
        IEnumerator MissionFailed()
        {
            yield return StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.75f), 1.5f));
            yield return StartCoroutine(CompletionMarking(CompletionMark.Failed));
            LoadHubEvent.Raise();
        }
    }
    [Button("Debug Raise : Mission Clear")]
    public void OnStageClear() //Played on Result Screen
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(MissionClear());
        IEnumerator MissionClear()
        {
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(Fade(Color.clear,Color.black,2.5f));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(CompletionMarking(CompletionMark.Clear));

        }
    }
    IEnumerator CompletionMarking(CompletionMark mark)
    {
        switch (mark)
        {
            case CompletionMark.Failed:
                ResultFailedEvent.Raise();
                yield break;
            case CompletionMark.Clear:
                ResultScreenEvent.Raise();
                yield break;
            default:
                yield break;
        }

    }
    IEnumerator Fade(Color start, Color target, float timeToFade)
    {
        _fadeCanvas.SetActive(true);
        float _time = 0f;
        float _fadeSpeed;
        AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        _halfFade.gameObject.SetActive(true);
        _halfFade.color = start;
        while (_halfFade.color != target)
        {
            _time += Time.deltaTime;
            _fadeSpeed = _speedCurve.Evaluate(_time / timeToFade);
            _halfFade.color = Color.Lerp(start, target, _fadeSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
    }
}
