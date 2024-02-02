using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] EnemyStates statePicker;
    [SerializeField]private float waitTime;

    private int subStateNum = 0;

    [InlineEditor]
    public List<SO_Enemy_States> states;
    public void Start()
    {
        statePicker = 0;
        StartCoroutine(Behave(waitTime, statePicker));

    }
    public IEnumerator Behave(float time, EnemyStates currentState)
    {
        states[(int)currentState].UseState(subStateNum);
        SwitchState();
        yield return new WaitForSeconds(time);
        StartCoroutine(Behave(waitTime, statePicker));
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
    public void Update()
    {
        
    }
}