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
    public void Start()
    {
        statePicker = 0;
        StartCoroutine(Behave(waitTime, statePicker));

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
        StartCoroutine(Behave(waitTime, statePicker));
    }
    public void SetAnimationName(EnemyStates index)
    {
        var currenStates = states[(int)index];
        status._animationName = currenStates.GetName(subStateNum);
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