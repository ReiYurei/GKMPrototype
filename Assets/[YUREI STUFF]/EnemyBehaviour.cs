using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;


public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    Enemy_Status status;

    private int subStateNum = 0;
    public List<StateCondition> _condition;
    private void Start()
    {
       foreach(StateCondition condition in _condition)
       {
           if (condition.GetName() == EnemyStates.Normal)
           {
               status.SetState(condition.state, subStateNum);
           }
           else continue;
       }
    }
    void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            status = component._status;

        }
        status.InitiateEnrage += OnEnrageInitiated;
        status.InitiateStun += OnStunInitiated;
        status.InitiateBreak += OnBreakInitiated;
        status.StunEnd += OnStunEnd;
        StartCoroutine(Behave(status.WaitTime));

    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        status.InitiateEnrage -= OnEnrageInitiated;
        status.InitiateStun -= OnStunInitiated;
        status.InitiateBreak -= OnBreakInitiated;
        status.StunEnd -= OnStunEnd;
    }
 
    public IEnumerator Behave(float time)
    {
        StateExecution();
        SwitchSubstate();
        yield return new WaitForSeconds(time);
        StartCoroutine(Behave(status.WaitTime));
    }

    public void OnEnrageInitiated()
    {
        Interrupt();
        subStateNum = 0;
        foreach (StateCondition condition in _condition)
        {

            if (condition.GetName() == EnemyStates.Enraged)
            {

                status.SetState(condition.state, subStateNum);
                StateExecution();
                StartCoroutine(TimedExecution(status.WaitTime));
                return;
            }
        }
    }
    public void OnStunInitiated()
    {
        status.SetPreviousState(status.GetState());
        Interrupt();
        subStateNum = 0;
        foreach (StateCondition condition in _condition)
        {
            if (condition.GetName() == EnemyStates.Stunned)
            {
                status.SetState(condition.state, subStateNum);
                StateExecution();
                return;
            }
        }
    }
    public void OnBreakInitiated()
    {
        Interrupt();
        subStateNum = 0;

        foreach (StateCondition condition in _condition)
        {
            if (condition.GetName() == EnemyStates.Flinched)
            {
                status.SetState(condition.state, subStateNum);
                StateExecution();
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
            Debug.Log(time);
        }
        BackToPreviousState();
       
    }


    public void BackToPreviousState()
    {
        Debug.Log(subStateNum);
        List<StateCondition> states = new List<StateCondition>();
        foreach (StateCondition condition in _condition)
        {
            if (condition.GetName() != EnemyStates.Stunned || condition.GetName() != EnemyStates.Enraged)
            {
                states.Add(condition);
            }
        }
        status.SetState(status.GetPreviousState(states),subStateNum);
        StartCoroutine(Behave(status.WaitTime));
    }
    public void Interrupt()
    {
        StopAllCoroutines();
    }

    public void StateExecution()
    {
        var currentState = status.GetState();

        currentState.Execute(enemy,subStateNum);
        status.SetState(currentState, subStateNum);
        status.SetAnimationHash(status.GetAnimationHashFromSubstate());
    }
    public void SwitchSubstate()
    {
        var subStateCount = status.GetState()._subStates.Count - 1;
        if (subStateNum >= subStateCount)
        {
            subStateNum = 0;
            return;
        }
        subStateNum++;
    }
   
}