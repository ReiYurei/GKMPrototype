using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] EnemyStates statePicker;
    [SerializeField] float waitTime;
    [SerializeField] Enemy_Status status;
    private int subStateNum = 0;


    [InlineEditor]
    public List<SO_Enemy_States> states;
    public List<StateCondition> _condition;



    void OnEnable()
    {
        InvokeRepeating(nameof(CheckCondition), 0.25f,0.5f);
        statePicker = 0;
        status.InitiateEnrage += OnEnrageInitiated;
        status.InitiateStun += OnStunInitiated;
        StartCoroutine(Behave(waitTime, statePicker));

    }
    private void OnDisable()
    {
        CancelInvoke();
        status.InitiateEnrage -= OnEnrageInitiated;
        status.InitiateStun -= OnStunInitiated;
    }
    public IEnumerator Behave(float time, EnemyStates index)
    {
        var currenStates = states[(int)index];
        currenStates.Execute(subStateNum);
        SetState(index);
        SetSubstate(index);
        SetAnimationName(index);
        yield return new WaitForSeconds(time);
        SwitchState();
        Interrupt();
        StartCoroutine(Behave(waitTime, statePicker));
    }
    void CheckCondition()
    {
        foreach (StateCondition condition in _condition) 
        { 
            if (condition.CheckValue())
            {
                status.states = condition.state;
            }
        }
    }
    public void OnEnrageInitiated()
    {

    }
    public void OnStunInitiated()
    {

    }
    public void Interrupt()
    {
        StopAllCoroutines();
    }
    public void SetAnimationName(EnemyStates index)
    {
        var currenStates = states[(int)index];
        status._animationHash = currenStates.GetAnimation(subStateNum);
    }
    public void SetState(EnemyStates index)
    {
        var currenStates = states[(int)index];
        status.states = currenStates;

    }
    public void SetSubstate(EnemyStates index)
    {
        var currenStates = states[(int)index];
        status.substates = currenStates._subStates[subStateNum];

    }
    public void SwitchState()
    {
        var subStateCount = states[(int)statePicker]._subStates.Count - 1;
        if (subStateNum >= subStateCount)
        {
            subStateNum = 0;
            return;
        }
        subStateNum++;
    }
   
}