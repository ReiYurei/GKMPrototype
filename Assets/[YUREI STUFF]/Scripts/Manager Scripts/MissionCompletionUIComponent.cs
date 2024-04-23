using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class MissionCompletionUIComponent : MonoBehaviour
{
    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent LoadHubEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ResultScreenEvent { get; private set; }


    [Header("Mission Completion Canvas")]
    [SerializeField] private GameObject _missionCompletionCanvas;
    [SerializeField] private Image _halfFade;

    [Header("Mission Failed")]
    [SerializeField] private GameObject _missionFailedCanvas;

    [Header("Mission Clear")]
    [SerializeField] private GameObject _missionClearCanvas;

    [Header("State")]
    [SerializeField] private LoadingScreenState _loadingState;

    [Header("Other")]
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private GameObject _objToScale;
    public enum CompletionMark { Failed, Clear}

    public void OnMissionFailed() //Played on Abandon/Player Death
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(MissionFailed());
        IEnumerator MissionFailed()
        {

            yield return Fade(new Color(0,0,0,0), new Color(0,0,0,0.5f),1.5f);
            yield return StartCoroutine(CompletionMarking(CompletionMark.Failed));
            LoadHubEvent.Raise();
        }
    }
    public void OnStageClear() //Played on Result Screen
    {
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(MissionClear());
        IEnumerator MissionClear()
        {
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(Fade(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1f),2.5f));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(CompletionMarking(CompletionMark.Clear));

        }
    }
    IEnumerator CompletionMarking(CompletionMark mark)
    {
        switch (mark)
        {
            case CompletionMark.Failed:
                _objToScale = _missionFailedCanvas;
                break;
            case CompletionMark.Clear:
                ResultScreenEvent.Raise();
                yield break;
            default:
                yield break;
        }
        bool executeOnce = false;
        float time = 0f;
        float scaleSpeed;
        float timeToScale = 2.5f;
        var targetScale = new Vector3(10, 10, 10);
        _objToScale.SetActive(true);
        _objToScale.transform.localScale = targetScale;
        while(_objToScale.transform.localScale != Vector3.one)
        {
            if (time > timeToScale * 0.8f && !executeOnce)
            {
                executeOnce = true;
                //PlaySound
            }
            time += Time.deltaTime;
            scaleSpeed = _speedCurve.Evaluate(time / timeToScale);
            _objToScale.transform.localScale = Vector3.Lerp(targetScale, Vector3.one, scaleSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(2f);


    }
    IEnumerator Fade(Color start, Color target, float timeToFade)
    {
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
