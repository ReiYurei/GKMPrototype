using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Ignore if Enemy component exist in object")]
    [ReadOnly] public Enemy _enemy;
    [SerializeField] public Enemy_Status _status;
    private int _subStateNum = 0;
    public List<StateCondition> condition;

    void OnEnable()
    {
        if (_status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            if (component == null)
            {
                Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                    $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(Enemy_Status)}");
                return;
            }
            _enemy = component;
            _status = _enemy.status;
        }
        foreach (StateCondition condition in condition)
        {
            if (condition.GetName() == EnemyStates.Normal)
            {
                _status.SetState(condition.state, _subStateNum);
                break;
            }
            else continue;
        }
       
        _status.InitiateEnrage += OnEnrageInitiated;
        _status.InitiateStun += OnStunInitiated;
        _status.InitiateBreak += OnBreakInitiated;
        _status.StunEnd += OnStunEnd;
        _status.AnimEnd += OnAnimEnd;
        StartCoroutine(Behave());
        //Debug.Log("Initiate Behave");

    }
    public void OnAnimEnd(bool animEnd)
    {
        //Debug.Log("end 3, " + animEnd);
        isFinished = animEnd;
    }
    bool isFinished;
    private void OnDisable()
    {
        StopAllCoroutines();
        _status.InitiateEnrage -= OnEnrageInitiated;
        _status.InitiateStun -= OnStunInitiated;
        _status.InitiateBreak -= OnBreakInitiated;
        _status.StunEnd -= OnStunEnd;
        _status.AnimEnd -= OnAnimEnd;

    }

    public IEnumerator Behave()
    {
        //Debug.Log("Behave");
        StartCoroutine(SubstateExecution());
        StartCoroutine(SetState());
        //Debug.Log("Now Wait");
        yield return new WaitUntil(() => isFinished ==true);
        SwitchSubstate();
        StartCoroutine(Behave());
    }


    public void OnEnrageInitiated()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (StateCondition condition in condition)
        {

            if (condition.GetName() == EnemyStates.Enraged)
            {

                _status.SetState(condition.state, _subStateNum);
                StartCoroutine(SubstateExecution());
                StartCoroutine(TimedExecution(2f));

                return;
            }
        }
    }
    public void OnStunInitiated()
    {
        _status.SetPreviousState(_status.GetState());
        Interrupt();
        _subStateNum = 0;
        foreach (StateCondition condition in condition)
        {
            if (condition.GetName() == EnemyStates.Stunned)
            {
                _status.SetState(condition.state, _subStateNum);
                StartCoroutine(SubstateExecution());
                return;
            }
        }
    }
    public void OnBreakInitiated()
    {
        Interrupt();
        _subStateNum = 0;

        foreach (StateCondition condition in condition)
        {
            if (condition.GetName() == EnemyStates.Flinched)
            {
                _status.SetState(condition.state, _subStateNum);
                StartCoroutine(SubstateExecution());
                StartCoroutine(TimedExecution(2f));
                return;
            }

        }
    }
    public void OnStunEnd()
    {
        BackToPreviousState();
    }
    public IEnumerator TimedExecution(float duration)
    {
        float time = duration;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        BackToPreviousState();
       
    }


    public void BackToPreviousState()
    {
        List<StateCondition> states = new List<StateCondition>();
        foreach (StateCondition condition in condition)
        {
            if (condition.GetName() != EnemyStates.Stunned || condition.GetName() != EnemyStates.Enraged)
            {
                states.Add(condition);
            }
        }
        _status.SetState(_status.GetPreviousState(states),_subStateNum);
        StartCoroutine(Behave());
    }
    public void Interrupt()
    {
        StopAllCoroutines();
    }

    public IEnumerator SubstateExecution()
    {
        var currentState = _status.GetState();
        yield return StartCoroutine(currentState.Execute(_enemy, _subStateNum));

    }

    public IEnumerator SetState()
    {
        var currentState = _status.GetState();
        _status.SetState(currentState, _subStateNum);
        yield break;
    }
    public void SwitchSubstate()
    {
        var subStateCount = _status.GetState()._subStates.Count - 1;
        if (_subStateNum >= subStateCount)
        {
            _subStateNum = 0;
            return;
        }
        _subStateNum++;
    }
   
}