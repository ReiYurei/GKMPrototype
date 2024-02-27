using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TriInspector;
using UnityEngine;
[CreateAssetMenu(fileName ="Enemy Status",menuName ="Enemy/Enemy Status")]
public class Enemy_Status : BaseStatus
{
    public delegate void OnEnraged();
    public event OnEnraged InitiateEnrage;
    public delegate void OnBreak();
    public event OnEnraged InitiateBreak;
    public delegate void OnStunned();
    public event OnEnraged InitiateStun;
    public delegate void OnPoisoned();
    public event OnEnraged InitiatePoison;

    [Header("Enemy Parameter")]
    [Required] public BooleanVariable _enraged;
    [Required] public BooleanVariable _stunned;
    [Required] public BooleanVariable _poisoned;
    [Required] public BooleanVariable _break;
    [Required] public BooleanVariable _statusBuildUp;

    [Header("Enemy Base Threshold")]
    [SerializeField]float baseStamina;
    [SerializeField]float baseRageThreshold;
    [SerializeField]float baseStunThreshold;
    [SerializeField]float basePoisonThreshold;
    public float _stamina;
    public void AffectRage(float damage)
    {
        if (_break.value != false) return;
        Debug.Log(1);
        if (_enraged.value == true)
        {
            Debug.Log(2);

            _rageMeter.value -= damage * 0.2f;
            if (_rageMeter.value <= 0)
            {
                Debug.Log(3);

                _enraged.value = false;
                _rageMeter.value = 0;
                InitiateBreak?.Invoke();
            }
            Debug.Log(_rageMeter.value);
            return;
        }
        if (_enraged.value == false)
        {
            Debug.Log(4);
            _rageMeter.value += damage * 0.15f;
            if (_rageMeter.value >= baseRageThreshold)
            {
                Debug.Log(5);

                _rageMeter.value = baseRageThreshold;
                _enraged.value = true;
                InitiateEnrage?.Invoke();
            }
            Debug.Log(_rageMeter.value);
            return;

        }

    }
    public void AffectStun(float stunValue)
    {
        if (_statusBuildUp.value != true) return;

        if (_stunned.value != false) return;
        _stunMeter.value += stunValue;

        if (_stunMeter.value >= baseStunThreshold)
        {
            InitiateStun?.Invoke();
            _stunMeter.value = 0;
        }

    }
    public void AffectPoison(float poisonValue)
    {
        if (_statusBuildUp.value != true) return;
        if (_poisoned.value != false) return;

        _poisonMeter.value += poisonValue;
        if (_poisonMeter.value >= basePoisonThreshold)
        {
            InitiatePoison?.Invoke();
            _poisonMeter.value = 0;
        }

    }


    [Header("Modifier")]
    [SerializeField]float baseDamageModifier;
    [SerializeField]float baseWeakpointModifier;
    [SerializeField]float baseWaitTime;
    [SerializeField]float baseAnimationSpeed;
    public float DamageModifier { get;private set; }
    public float WeakpointModifier { get; private set; }
    public float WaitTime { get; private set; }
    public float AnimationSpeed { get; private set; }
    public void RageModifier(float damageModifier, float animationSpeed)
    {
        DamageModifier = damageModifier / 100;
        AnimationSpeed = animationSpeed / 100;
    }
    public void DefaultModifier()
    {
        _rageMeter.value = 0;
        _stunMeter.value = 0;
        _poisonMeter.value = 0;
        DamageModifier = baseDamageModifier;
        WeakpointModifier = baseWeakpointModifier;
        WaitTime = baseWaitTime;
        AnimationSpeed = baseAnimationSpeed;
        _enraged.value = false;

    }
    [Header("========DEBUG AREA========")]
    [Header("Enemy Rage")]
    [InlineEditor]
    [Required] public FloatVariable _rageMeter;

    [Header("Enemy Stun")]

    [InlineEditor]
    [Required] public FloatVariable _stunMeter;


    [Header("Enemy Poison")]
    [InlineEditor]
    [Required]public FloatVariable _poisonMeter;

    [Header("Animation")]
    public int _animationHash;

    [Header("States")]
    public SO_Enemy_States states;
    public SO_Enemy_Substate substates;


    public override void OnSpawn()
    {
        base.OnSpawn();
        DefaultModifier();
        _statusBuildUp.value = true;
        _stunned.value = false;
        _enraged.value = false;
        _break.value = false;
        _poisoned.value = false;
        _rageMeter.value = 0; 
        _stunMeter.value = 0; 
        _poisonMeter.value = 0;
    }


}

