using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;


//- EnemyBehaviourComponent:
//- Manages the behavior of an _enemy using State Machine.
//- Controls dynamic and fixed _cutsceneState changes.
//- Utilizes coroutines for _cutsceneState execution and timed _actions.
public class EnemyBehaviour : MonoBehaviour
{
    [Header("READ ONLY PROPERTIES")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private SO_EnemyStatus _status;
    private int _subStateNum = 0;
    [SerializeField][InlineEditor] private List<SO_DynamicState> _dynamicStates;
    [SerializeField] private List<FixedState> _fixedStates;

    void OnEnable()
    {
        if (_status == null)
        {
            TryGetComponent(out Enemy component);
            if (component == null)
            {
                Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                    $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(SO_EnemyStatus)}");
                return;
            }
            _enemy = component;
            _status = _enemy.StatusData;
        }     
        _status.InitiateEnrage += OnEnrageInitiated;
        _status.InitiateStun += OnStunInitiated;
        _status.InitiateBreak += OnBreakInitiated;
        _status.StunEnd += OnStunEnd;
        _status.AnimEnd += OnAnimEnd;
        OnValueChange();
        StartCoroutine(Behave());
        //Debug.Log("Initiate Behave");

    }
    public void OnValueChange()
    {
        _subStateNum = 0;
        Debug.Log("Behaviour Event Raised");
        foreach (SO_DynamicState dynamicState in _dynamicStates)
        {
            if (dynamicState.CheckCondition())
            {
                Debug.Log(dynamicState.states.name);
                if (_status.States == dynamicState.states) continue;
                Interrupt();
                _status.SetStateAndSubstate(dynamicState.states,_subStateNum);
                _subStateNum = 0;
                return;
            }           
        }
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
        StartCoroutine(ChangeState());
        //Debug.Log("Now Wait");
        yield return new WaitUntil(() => isFinished ==true);
        SwitchSubstate();
        StartCoroutine(Behave());
    }


    public void OnEnrageInitiated()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState fixedState in _fixedStates)
        {
        
            if (fixedState.GetName() == EnemyStates.Enraged)
            {
        
                _status.SetStateAndSubstate(fixedState.state, _subStateNum);
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
        foreach (FixedState state in _fixedStates)
        {
            if (state.GetName() == EnemyStates.Stunned)
            {
                _status.SetStateAndSubstate(state.state, _subStateNum);
                StartCoroutine(SubstateExecution());
                return;
            }
        }
    }
    public void OnBreakInitiated()
    {
        Interrupt();
        _subStateNum = 0;

        foreach (FixedState fixedState in _fixedStates)
        {
            if (fixedState.GetName() == EnemyStates.Flinched)
            {
                _status.SetStateAndSubstate(fixedState.state, _subStateNum);
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
        List<SO_DynamicState> states = new List<SO_DynamicState>();
        foreach (SO_DynamicState dynamicState in _dynamicStates)
        {

        }
        //_status.SetStateAndSubstate(_status.GetPreviousState(states),_subStateNum);
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

    public IEnumerator ChangeState()
    {
        var currentState = _status.GetState();
        _status.SetStateAndSubstate(currentState, _subStateNum);
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