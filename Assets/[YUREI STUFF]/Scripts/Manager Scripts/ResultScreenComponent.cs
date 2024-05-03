using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class ResultScreenComponent : MonoBehaviour, IAudioSource
{
    [System.Serializable] private class RankQualifier
    {
        [field: SerializeField] public float S_Rank { get; private set; }
        [field: SerializeField] public float A_Rank{ get; private set; }
        [field: SerializeField] public float B_Rank{ get; private set; }
        [field: SerializeField] public float C_Rank{ get; private set; }
        [field: SerializeField] public float D_Rank{ get; private set; }

    }
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent MissionCompleteEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent LoadHubEvent { get; private set; }


    [Header("Result Screen Canvas")]
    [SerializeField] private GameObject _resultScreenCanvas;
    [SerializeField] private GameObject[] _rankings;
    [SerializeField] private RankQualifier _qualifier;
    [SerializeField] private TextMeshProUGUI _clearTime;
    [SerializeField] private TextMeshProUGUI _moneyAmount;
    [Header("Essentials Canvas")]
    [SerializeField] private GameObject _successStamp;
    [SerializeField] private GameObject _failedCanvas;
    [SerializeField] private GameObject _failedStamp;
    [SerializeField] private Image _blackScreen;

    [Header("Properties")]
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private InputActionAsset _input;
    private TimeRank _rankMark;
    private CompletionMark _result;
    private enum TimeRank { S,A,B,C,D,E}
    private GameObject _rank;
    private int moneyAmount;
    private string minutes;
    private string seconds;
    private string miliSeconds;

    [Header("State")]
    [SerializeField] private TitleScreenState _state;
    [SerializeField] private LoadingScreenState _loadingState;

    private void OnDisable()
    {
        _input.FindActionMap("UI").FindAction("Confirm").performed -= Skip;
    }
    private void OnApplicationQuit()
    {
        _input.FindActionMap("UI").FindAction("Confirm").performed -= Skip;
    }
    private void Skip(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_result != CompletionMark.Clear) return;
        if (!_successStamp.activeInHierarchy && _successStamp.transform.localScale != Vector3.one)
        {
            StopAllCoroutines();
            Debug.Log("SKIP");
            _rank.SetActive(true);
            _rank.transform.localScale = Vector3.one;
            _blackScreen.gameObject.SetActive(false);

            if (Observer.AssignedMission != null && Observer.AssignedMission.Rewards.Count > 0)
            {
                for (int i = 0; i < Observer.AssignedMission.Rewards.Count; i++)
                {
                    var reward = Observer.AssignedMission.Rewards[i];
                    if (i == Observer.AssignedMission.Rewards.Count - 1 && reward.GetType() != typeof(SO_MoneyReward))
                    {
                        _moneyAmount.text = "0";
                        break;
                    }
                    if (reward is SO_MoneyReward)
                    {
                        var moneyReward = reward as SO_MoneyReward;
                        moneyAmount = moneyReward.Amount;
                        _moneyAmount.text = moneyAmount.ToString();
                        StartCoroutine(Stamp(new Vector3(10, 10, 10), Vector3.one, _successStamp));
                        return;
                    }
                }            
            }
            StartCoroutine(Stamp(new Vector3(10, 10, 10), Vector3.one, _successStamp));
            MissionCompleteEvent.Raise();
            return;
        } //To Skip Animations of Rank-Money Count for the first pressed button,
        _successStamp.transform.localScale = Vector3.one;                                      //To Skip Stamp Animation and go straight to Hub
        _successStamp.SetActive(true);
        MissionCompleteEvent.Raise();
        LoadHubEvent.Raise();
        _input.FindActionMap("UI").FindAction("Confirm").performed -= Skip;
    }


    public void OnTimerStop(ScriptableObject data) //Initialize Rank Data whenever the timer is stopped
    {
        var time = data as FloatVariable;
        
        _successStamp.transform.localScale = new Vector3(10,10, 10);
        minutes = Mathf.Floor(time.value / 60).ToString("00");
        seconds = Mathf.Floor(time.value % 60).ToString("00");
        miliSeconds = Mathf.Floor((time.value * 1000) % 1000).ToString("000");
        _clearTime.text = $"{minutes} : {seconds}.{miliSeconds}";
        _rankMark = GetMark(time.value);
        _rank = GetRank(_rankMark);
    }
    public void OnMissionFailed()
    {
        _result = CompletionMark.Failed;
        StartCoroutine(MissionFailed());
        IEnumerator MissionFailed()
        {
            _failedCanvas.SetActive(true);
            Debug.Log("<color=yellow> FAILED</color>");

            yield return StartCoroutine(Stamp(new Vector3(10, 10, 10), Vector2.one,_failedStamp));
            Debug.Log("<color=yellow> STAMP CLEAR</color>");

            yield return new WaitForSeconds(1.5f);
            Debug.Log("<color=yellow> COROUTINE CLEAR</color>");
            LoadHubEvent.Raise();
        }
    }
    public void OnStageClear()
    {
        _result = CompletionMark.Clear;
        ChangeStateEvent.Raise(_state);
        StartCoroutine(ResultInitialize());
        IEnumerator ResultInitialize()
        {
            yield return StartCoroutine(FadeOut(Color.black, Color.clear));
            _input.FindActionMap("UI").FindAction("Confirm").performed += Skip; //Only Listening to Skip Input after Fading out is done
            yield return StartCoroutine(Ranking(new Vector3(10, 10, 10), Vector2.one));
            yield return StartCoroutine(MoneyCount());
            yield return StartCoroutine(Stamp(new Vector3(10, 10, 10), Vector2.one, _successStamp));
            MissionCompleteEvent.Raise();

        }
    }

    IEnumerator FadeOut(Color start, Color target)
    {
        float time = 0f;
        float timeToFade = 1.5f;
        float speed;
        AnimationCurve linear = AnimationCurve.Linear(0, 0, 1, 1);
        _blackScreen.color = start;
        _blackScreen.gameObject.SetActive(true);
        _resultScreenCanvas.SetActive(true);
        while (_blackScreen.color != target)
        {
            time += Time.deltaTime;
            speed = linear.Evaluate(time / timeToFade);
            _blackScreen.color = Color.Lerp(start, target, speed);
            yield return null;
        }

    } //Fade Out Animation
    IEnumerator Ranking(Vector3 start, Vector3 target)
    {
        //VFX
        float time = 0f;
        float timeToFade = 1.5f;
        float speed;
        bool executeOnce = false;
        if (_rank == null) _rank = GetRank(TimeRank.E);
        _rank.transform.localScale = start;
        _rank.gameObject.SetActive(true);
        while (_rank.transform.localScale != target)
        {
            time += Time.deltaTime;
            if (!executeOnce && time > timeToFade * 0.35f)
            {
                executeOnce = true;
                //playsound
            }

            speed = _speedCurve.Evaluate(time / timeToFade);
            _rank.transform.localScale = Vector3.Lerp(start, target, speed);
            yield return null;
        }
        yield return new WaitForSeconds(1);
    } //Ranking Animation
    IEnumerator MoneyCount()
    {

        if (Observer.AssignedMission == null || Observer.AssignedMission.Rewards.Count <= 0) yield break;
        for (int i = 0; i < Observer.AssignedMission.Rewards.Count; i++)
        {

            var reward = Observer.AssignedMission.Rewards[i];
            if (i == Observer.AssignedMission.Rewards.Count - 1 && reward.GetType() != typeof(SO_MoneyReward))
            {
                yield break;
            }
            if (reward is SO_MoneyReward)
            {
                var moneyReward = reward as SO_MoneyReward;
                moneyAmount = moneyReward.Amount;
            }
        }
        int count = 0;
        while (count < moneyAmount)
        {
            count += Mathf.RoundToInt(Time.deltaTime * 500f);
            _moneyAmount.text = count.ToString();
            yield return null;
        }
        _moneyAmount.text = moneyAmount.ToString();
        yield return new WaitForSeconds(1);
    } //Counting Animation
    IEnumerator Stamp(Vector3 start, Vector3 target, GameObject stamp)
    {
        //VFX
        float time = 0f;
        float timeToScale = 2f;
        float speed;
        bool executeOnce = false;
        stamp.transform.localScale = start;
        stamp.gameObject.SetActive(true);
        while (stamp.transform.localScale != target)
        {
            time += Time.deltaTime;
            if (!executeOnce && time > timeToScale * 0.35f)
            {
                executeOnce = true;
                //playsound
            }

            speed = _speedCurve.Evaluate(time / timeToScale);
            stamp.transform.localScale = Vector3.Lerp(start, target, speed);
            yield return null;
        }
        yield return new WaitForSeconds(1);
    } //Stamp Animation

    private TimeRank GetMark(float time)
    {
        if (time <= _qualifier.S_Rank) return TimeRank.S;
        else if (time > _qualifier.S_Rank && time <= _qualifier.A_Rank) return TimeRank.A;
        else if (time > _qualifier.A_Rank && time <= _qualifier.B_Rank) return TimeRank.B;
        else if (time > _qualifier.B_Rank && time <= _qualifier.C_Rank) return TimeRank.C;
        else if (time > _qualifier.C_Rank && time <= _qualifier.D_Rank) return TimeRank.D;
        else return TimeRank.E;

    }
    private GameObject GetRank(TimeRank rank)
    {
        switch (rank)
        {
            case TimeRank.S:
                return _rankings[0];
            case TimeRank.A: 
                return _rankings[1];
            case TimeRank.B: 
                return _rankings[2];
            case TimeRank.C: 
                return _rankings[3];
            case TimeRank.D:
                return _rankings[4];
            case TimeRank.E:
                return _rankings[5];
            default: return _rankings[5];

        }
    }
}