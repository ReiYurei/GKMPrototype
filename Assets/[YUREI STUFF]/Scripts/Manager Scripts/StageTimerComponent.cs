using TMPro;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class StageTimerComponent : MonoBehaviour
{
    [field: SerializeField] public SO_ParameterGameEvent TimerEvent { get; private set; }
    [SerializeField]private FloatVariable _time;
    //public TextMeshProUGUI timerText;

    //private string minutes;
    //private string seconds;
    //private string miliSeconds;

    bool timerTicking;
    public void StartTimer()
    {
        timerTicking = true;
        _time.value = 0;
    }
    public void StopTimer()
    {
        timerTicking = false;
        TimerEvent.Raise(_time);
    }
    private void Start()
    {
        //minutes = Mathf.Floor(time / 60).ToString("00");
        //seconds = Mathf.Floor(time % 60).ToString("00");
        //miliSeconds = Mathf.Floor((time * 1000) % 1000).ToString("000");
    }
    private void Update()
    {
        if(timerTicking)
        {
            _time.value += Time.deltaTime;
        }
    }

}
