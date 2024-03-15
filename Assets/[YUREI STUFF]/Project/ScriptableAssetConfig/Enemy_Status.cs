using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;
#if UNITY_EDITOR

#endif

[CreateAssetMenu(fileName ="Enemy Status",menuName ="Enemy/Enemy Status")]
public class Enemy_Status : BaseStatus
{
    public delegate void OnStatusStart();
    public event OnStatusStart InitiateEnrage, InitiateBreak, InitiateStun, InitiatePoison;
    public delegate void OnStatusEnd();
    public event OnStatusEnd EnrageEnd, BreakEnd, StunEnd, PoisonEnd;
    public delegate void OnActionEnd(bool isAnimEnd);
    public event OnActionEnd AnimEnd, AttackEnd;
    public delegate void OnProjectile();
    public event OnProjectile InitiateProjectile;

    [SerializeField] bool cannotRage;
    public bool _isHalved { get; private set; }
    public void SetHalvedAnim(bool isHalved)
    {
        _isHalved = isHalved;
    }

    public bool _isAttacking { get; private set; }
    public void SetAttacking(bool isAttacking)
    {
        _isAttacking = isAttacking;
    }
    public bool _isNextAttackReady { get; private set; }
    public bool _isMoving { get; private set; }
    public void SetIsMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }

    public void NotifyProjectile()
    {
        InitiateProjectile?.Invoke();
    }
    public void NotifyEndOfStatus(BaseStatusEffect status)
    {
        switch (status)
        {         
            case SO_Rage: EnrageEnd?.Invoke();
                break;           
            case SO_Break: BreakEnd?.Invoke();
                break;
            case SO_Stun: StunEnd?.Invoke();
                break;
            case SO_Poison: PoisonEnd?.Invoke();
                break;

        }
    }
    public void NotifyAttacking(bool isAttacking)
    {
        AttackEnd?.Invoke(isAttacking);
        _isAttacking = isAttacking;
        _isNextAttackReady = false;
        if(isAttacking == false) { NotifyEndOfAnim(true); }

    }
    public void NotifyEndOfAnim(bool animEnd)
    {
        if (_isAttacking == true)
        {
            _isNextAttackReady = animEnd;
            return;
        }

        AnimEnd?.Invoke(animEnd);
    }



    [Header("Enemy Parameter")]
    [Required]public BooleanVariable _enraged;
    [Required]public BooleanVariable _stunned;
    [Required]public BooleanVariable _poisoned;
    [Required]public BooleanVariable _break;
    [Required]public BooleanVariable _statusBuildUp;

    [Header("Enemy Base Threshold")]
    [SerializeField]float baseStamina = 100f;
    [SerializeField]float baseRageThreshold = 100f;
    [SerializeField]float baseStunThreshold = 100f;
    [SerializeField]float basePoisonThreshold = 100f;
    public void ReduceStamina(float decrement)
    {
        _stamina.value -= decrement;
    }
    public void AffectRage(float damage)
    {
        if (cannotRage == true) return;
        if (_break.value == true) return;
        if (_enraged.value == true)
        {
            _rageMeter.value -= damage * 0.2f;
            if (_rageMeter.value <= 0 && _statusBuildUp.value == true)
            {
                _enraged.value = false;
                _rageMeter.value = 0;
                InitiateBreak?.Invoke();
            }
            return;
        }
        if (_enraged.value == false)
        {
            _rageMeter.value += damage * 0.15f;
            if (_rageMeter.value >= baseRageThreshold && _statusBuildUp.value == true)
            {
                _rageMeter.value = baseRageThreshold;
                _enraged.value = true;
                InitiateEnrage?.Invoke();
                
            }
            return;

        }

    }
    public void AffectStun(float stunValue)
    {
        if (_statusBuildUp.value == false) return;
        if (_stunned.value == true) return;

        _stunMeter.value += stunValue;
        if (_stunMeter.value >= baseStunThreshold)
        {
            InitiateStun?.Invoke();
            _stunMeter.value = 0;
        }
    
    }
    public void AffectPoison(float poisonValue)
    {
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
    public float AnimationSpeed { get; private set; }
    public void Modifier(float damageModifier, float animationSpeed)
    {
        DamageModifier = damageModifier / 100;
        AnimationSpeed = animationSpeed / 100;
    }
  

    [Header("========DEBUG AREA========")]
    [Header("Stamina Meter")]
    [InlineEditor]
    [Required] public FloatVariable _stamina;

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
    [ShowInInspector] int _animationHash;
    public delegate void OnAnimChange();
    public event OnAnimChange NotifyAnimChange;
    public bool _noFlip { get; private set; }
    public void SetNoFlip(bool noFlip)
    {
        _noFlip = noFlip;
    }
    public void SetAnimationHashAndNotify(int hash)
    {
        _animationHash = hash;
        NotifyAnimChange?.Invoke();
    }
    public int GetAnimationHashFromStatus()
    {
        return _animationHash;
    }
    public int GetAnimationHashFromSubstate()
    {
        return substates.GetAnimation();
    }

    [Header("States")]
    [SerializeField] SO_Enemy_States states;
    SO_Enemy_Substate substates;
    [SerializeField] SO_Enemy_States previousStates;

    public void SetPreviousState(SO_Enemy_States state)
    {
        previousStates = state;
    }
    public SO_Enemy_States GetPreviousState(List<StateCondition> states) 
    {  
        if(_enraged.value == true) 
        {
            foreach (StateCondition condition in states)
            {
                if (condition.GetName() == EnemyStates.Raging)
                {
                    previousStates = condition.state;
                }
            }
            return previousStates;
        }
        else
        {
            foreach (StateCondition condition in states)
            {
                if (condition.GetName() == EnemyStates.Normal)
                {
                    previousStates = condition.state;
                }
            }
            return previousStates;
        }
    }
    public SO_Enemy_States GetState()
    {
        return states;
    }
    public void SetState(SO_Enemy_States state, int index)
    {
        states = state;
        substates = state._subStates[index];
    }

    [Button("Reset to Default")]
    public override void OnSpawn()
    {
        base.OnSpawn();
        DefaultModifier();
        _isMoving = false;
        _isAttacking = false;
        _isNextAttackReady = false;
        _statusBuildUp.value = true;
        _stunned.value = false;
        _enraged.value = false;
        _break.value = false;
        _poisoned.value = false;
        _stamina.value = baseStamina;
        _rageMeter.value = 0; 
        _stunMeter.value = 0; 
        _poisonMeter.value = 0;
        _noFlip = false;
        _isHalved = false;
    }

    public void DefaultModifier()
    {
        
        DamageModifier = baseDamageModifier;
        WeakpointModifier = baseWeakpointModifier;
        AnimationSpeed = baseAnimationSpeed;
        _enraged.value = false;
        _break.value = false;
    }
}

