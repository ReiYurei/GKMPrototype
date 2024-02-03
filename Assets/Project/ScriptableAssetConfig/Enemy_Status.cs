using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy Status",menuName ="Enemy/Enemy Status")]
public class Enemy_Status : BaseStatus
{


    [Header("Enemy Threshold")]
    public float _stamina; // optional
    public float _rageThreshold;
    public float _stunThreshold;
    public float _poisonThreshold;

    [Header("Modifier")]
    public float _damageModifier;
    public float _weakpointModifier;
    public float _waitTime;
    public float _animationSpeed;

    [Header("Animation")]
    public string _animationName;

    [Header("States")]
    public SO_Enemy_States states;
    public SO_Enemy_Substate substates;

    public override void OnSpawn()
    {
        base.OnSpawn();
        
    }


}
