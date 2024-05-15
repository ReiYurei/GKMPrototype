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
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent BulletHellPhaseEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent RegularPhaseEvent { get; private set; }

    [Header("READ ONLY PROPERTIES")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private SO_EnemyStatus _status;
  
    //heehee

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
        //StartCoroutine(Behave());
        //Debug.Log("Initiate Behave");

    }
    [Button("Initialize State")]
    public void InitializeState()
    {
        OnHealthChange();
    }
    [Button("Start Behave")]
    public void OnPhaseChanged()
    {
        StartCoroutine(Behave());
    }
    public void OnHealthChange() //Listen to event
    {
        foreach (SO_DynamicState dynamicState in _dynamicStates)
        {
            if (dynamicState.CheckCondition())
            {
                if (_status.DynamicState == dynamicState) continue;
                _subStateNum = 0;
                Debug.Log(dynamicState.ToString());
                Debug.Log(_status.DynamicState);

                _status.SetDynamicStates(dynamicState);
                ChangeStateEvent.Raise(dynamicState.gameplayState);
                Interrupt();
                _status.SetState(_status.PreviousState());
                _subStateNum = 0;
                if (dynamicState.gameplayState is BulletHellGameplayState) BulletHellPhaseEvent.Raise();
                else RegularPhaseEvent.Raise();
            
                //StartCoroutine(Behave());
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
        Debug.Log("SUBSTATE INDEX:  "+_subStateNum);
        StartCoroutine(SubstateExecution());
        StartCoroutine(ChangeState());
        //Debug.Log("Now Wait");
        yield return new WaitUntil(() => isFinished ==true);
        SwitchSubstate();
        StartCoroutine(Behave());
    }

    public void OnDeath()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState fixedState in _fixedStates)
        {
            if (fixedState.GetName() == EnemyStates.Death)
            {
                _status.SetState(fixedState.state);
                StartCoroutine(SubstateExecution());
                return;
            }

        }
    }
    public void OnPlayerOutOfRange()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState fixedState in _fixedStates)
        {
            if (fixedState.GetName() == EnemyStates.OutOfRange)
            {
                _status.SetState(fixedState.state);
                StartCoroutine(SubstateExecution());
                return;
            }

        }

    }
    public void OnEnrageInitiated()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState fixedState in _fixedStates)
        {
        
            if (fixedState.GetName() == EnemyStates.Enraged)
            {
        
                _status.SetState(fixedState.state);
                StartCoroutine(SubstateExecution());
                StartCoroutine(TimedExecution(2f));
        
                return;
            }
        }
    }
    public void OnStunInitiated()
    {
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState state in _fixedStates)
        {
            if (state.GetName() == EnemyStates.Stunned)
            {
                _enemy.GravitySet(50);
                _status.SetState(state.state);
                StartCoroutine(SubstateExecution());
                return;
            }
        }
    }
    public void OnBreakInitiated()
    {
        Debug.Log("Break Initiated");
        Interrupt();
        _subStateNum = 0;
        foreach (FixedState fixedState in _fixedStates)
        {
            if (fixedState.GetName() == EnemyStates.Break)
            {
                _enemy.GravitySet(50);
                _status.SetState(fixedState.state);
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
        StopAllCoroutines();
        _status.SetState(_status.PreviousState());
        StartCoroutine(Behave());
    }
    public void Interrupt()
    {
        StopAllCoroutines();
    }

    public IEnumerator SubstateExecution()
    {
        var currentState = _status.States; 
        yield return StartCoroutine(currentState.Execute(_enemy, _subStateNum));

    }

    public IEnumerator ChangeState()
    {
        var currentState = _status.States;
        _status.SetState(currentState);
        yield break;
    }
    public void SwitchSubstate()
    {
        var subStateCount = _status.States._subStates.Count - 1;
        if (_subStateNum >= subStateCount)
        {
            _subStateNum = 0;
            return;
        }
        _subStateNum++;
    }

}