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
    [ShowInInspector]public bool isHalved { get; private set; }
    public void SetHalvedAnim(bool isHalved)
    {
        this.isHalved = isHalved;
    }

    [ShowInInspector]public bool isAttacking { get; private set; }
    public void SetAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }
    [ShowInInspector]public bool isNextAttackReady { get; private set; }
    [ShowInInspector]public bool isMoving { get; private set; }
    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
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
        this.isAttacking = isAttacking;
        isNextAttackReady = false;
        if(isAttacking == false) { NotifyEndOfAnim(true); }

    }
    public void NotifyEndOfAnim(bool animEnd)
    {
        if (isAttacking == true)
        {
            isNextAttackReady = animEnd;
            return;
        }
        AnimEnd?.Invoke(animEnd);
    }



    [Header("Enemy Parameter")]
    [Required]public BooleanVariable b_Enraged;
    [Required]public BooleanVariable b_Stunned;
    [Required]public BooleanVariable b_Poisoned;
    [Required]public BooleanVariable b_Break;
    [Required]public BooleanVariable b_StatusBuildUp;

    [Header("Enemy Base Threshold")]
    [SerializeField]float baseStamina = 100f;
    [SerializeField]float baseRageThreshold = 100f;
    [SerializeField]float baseStunThreshold = 100f;
    [SerializeField]float basePoisonThreshold = 100f;
    public void ReduceStamina(float decrement)
    {
        f_Stamina.value -= decrement;
    }
    public void AffectRage(float damage)
    {
        if (cannotRage == true) return;
        if (b_Break.value == true) return;
        if (b_Enraged.value == true)
        {
            f_RageMeter.value -= damage * 0.2f;
            if (f_RageMeter.value <= 0 && b_StatusBuildUp.value == true)
            {
                b_Enraged.value = false;
                f_RageMeter.value = 0;
                InitiateBreak?.Invoke();
            }
            return;
        }
        if (b_Enraged.value == false)
        {
            f_RageMeter.value += damage * 0.15f;
            if (f_RageMeter.value >= baseRageThreshold && b_StatusBuildUp.value == true)
            {
                f_RageMeter.value = baseRageThreshold;
                b_Enraged.value = true;
                InitiateEnrage?.Invoke();
                
            }
            return;

        }

    }
    public void AffectStun(float stunValue)
    {
        if (b_StatusBuildUp.value == false) return;
        if (b_Stunned.value == true) return;

        f_StunMeter.value += stunValue;
        if (f_StunMeter.value >= baseStunThreshold)
        {
            InitiateStun?.Invoke();
            f_StunMeter.value = 0;
        }
    
    }
    public void AffectPoison(float poisonValue)
    {
        if (b_Poisoned.value != false) return;

        f_PoisonMeter.value += poisonValue;
        if (f_PoisonMeter.value >= basePoisonThreshold)
        {
            InitiatePoison?.Invoke();
            f_PoisonMeter.value = 0;
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
    [Required] public FloatVariable f_Stamina;

    [Header("Enemy Rage")]
    [InlineEditor]
    [Required] public FloatVariable f_RageMeter;

    [Header("Enemy Stun")]

    [InlineEditor]
    [Required] public FloatVariable f_StunMeter;


    [Header("Enemy Poison")]
    [InlineEditor]
    [Required]public FloatVariable f_PoisonMeter;

    [Header("Animation")]
    [ShowInInspector][ReadOnly]public int _animationHash;
    public delegate void OnAnimChange();
    public event OnAnimChange NotifyAnimChange;
    public bool noFlip { get; private set; }
    public void SetNoFlip(bool noFlip)
    {
        this.noFlip = noFlip;
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
        return _substates.GetAnimation();
    }
    [Header("States")]
    [ReadOnly] public SO_Enemy_States _states;
    [ReadOnly] public SO_Enemy_Substate _substates;
    [ReadOnly] public SO_Enemy_States _previousStates;

    public void SetPreviousState(SO_Enemy_States state)
    {
        _previousStates = state;
    }
    public SO_Enemy_States GetPreviousState(List<StateCondition> states) 
    {  
        if(b_Enraged.value == true) 
        {
            foreach (StateCondition condition in states)
            {
                if (condition.GetName() == EnemyStates.Raging)
                {
                    _previousStates = condition.state;
                }
            }
            return _previousStates;
        }
        else
        {
            foreach (StateCondition condition in states)
            {
                if (condition.GetName() == EnemyStates.Normal)
                {
                    _previousStates = condition.state;
                }
            }
            return _previousStates;
        }
    }
    public SO_Enemy_States GetState()
    {
        return _states;
    }
    public void SetStateAndSubstate(SO_Enemy_States state, int index)
    {
        _states = state;
        _substates = state._subStates[index];
    }

    [Button("Reset to Default")]
    public override void OnSpawn()
    {
        base.OnSpawn();
        DefaultModifier();
        isMoving = false;
        isAttacking = false;
        isNextAttackReady = false;
        b_StatusBuildUp.value = true;
        b_Stunned.value = false;
        b_Enraged.value = false;
        b_Break.value = false;
        b_Poisoned.value = false;
        f_Stamina.value = baseStamina;
        f_RageMeter.value = 0; 
        f_StunMeter.value = 0; 
        f_PoisonMeter.value = 0;
        noFlip = false;
        isHalved = false;
    }

    public void DefaultModifier()
    {
        
        DamageModifier = baseDamageModifier;
        WeakpointModifier = baseWeakpointModifier;
        AnimationSpeed = baseAnimationSpeed;
        b_Enraged.value = false;
        b_Break.value = false;
    }
}

