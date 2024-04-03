using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TriInspector;
using ReiYurei;
#if UNITY_EDITOR

#endif

[CreateAssetMenu(fileName ="Enemy Status",menuName ="Enemy/Enemy Status")]
public class SO_EnemyStatus : BaseStatus
{
    public delegate void OnStatusStart();
    public event OnStatusStart InitiateEnrage, InitiateBreak, InitiateStun, InitiatePoison;
    public delegate void OnStatusEnd();
    public event OnStatusEnd EnrageEnd, BreakEnd, StunEnd, PoisonEnd;
    public delegate void OnActionEnd(bool isAnimEnd);
    public event OnActionEnd AnimEnd, AttackEnd, ShootEnd;


    [SerializeField] private bool cannotRage;
    [field: Header("Enemy Condition")]
    [field: SerializeField] public bool IsHalved { get; private set; }
    public void SetHalvedAnim(bool isHalved) //Call from Substate (Attacking)
    {
        this.IsHalved = isHalved;
    }

    [field: SerializeField] public bool IsAttacking { get; private set; }
    public void SetAttacking(bool isAttacking) //Call from Substate (Attacking)
    {
        IsAttacking = isAttacking;
    }
    [field: SerializeField] public bool IsShooting { get; private set; }
    public void SetShooting(bool isShooting) //Call from Substate (Projectile)
    {
        IsShooting = isShooting;
    }

    [field: SerializeField]public bool IsNextAttackReady { get; private set; }
    [field: SerializeField]public bool IsMoving { get; private set; }
    public void SetIsMoving(bool isMoving) //Call from Substate (Moving)
    {
        this.IsMoving = isMoving;
    }

    //EVENTS
    public void NotifyEndOfStatus(BaseStatusEffect status)//Call from Effect Statuses
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
    public void NotifyAttacking(bool isAttacking) //Call from Substate (Attack Type Substate)
    {
        AttackEnd?.Invoke(isAttacking);
        this.IsAttacking = isAttacking;
        IsNextAttackReady = false;
        if(isAttacking == false) { NotifyEndOfAnim(true); }

    }
    public void NotifyShooting(bool isShooting) //Call from Substate (Attack Type Substate)
    {
        ShootEnd?.Invoke(isShooting);
        this.IsShooting = isShooting;
        if (isShooting == false) { NotifyEndOfAnim(true); }

    }
    public void NotifyEndOfAnim(bool animEnd) //Call from AnimatorComponent
    {
        if (IsAttacking == true)
        {
            IsNextAttackReady = animEnd;
            return;
        }
        AnimEnd?.Invoke(animEnd);
    }



    [field: Header("Enemy Parameter")] //Boolean variable
    [field: SerializeField][Required]public BooleanVariable B_Enraged { get; private set; }
    [field: SerializeField][Required]public BooleanVariable B_Stunned { get; private set; }
    [field: SerializeField][Required]public BooleanVariable B_Poisoned { get; private set; }
    [field: SerializeField][Required]public BooleanVariable B_Break { get; private set; }
    [field: SerializeField][Required]public BooleanVariable B_StatusBuildUp { get; private set; }
#if UNITY_EDITOR //Editor Variable
    [SerializeField] private bool _enraged;
    [SerializeField] private bool _poisoned;
    [SerializeField] private bool _stunned;
    [SerializeField] private bool _break;
    [SerializeField] private bool _statusBuildUp;
#endif

    [Header("Enemy Base Threshold")]
    [SerializeField]private float baseStamina = 100f;
    [SerializeField]private float baseRageThreshold = 100f;
    [SerializeField]private float baseStunThreshold = 100f;
    [SerializeField]private float basePoisonThreshold = 100f;
    public void ReduceStamina(float decrement)
    {
        F_Stamina.value -= decrement;
    }
    public void AffectRage(float damage)
    {
        if (cannotRage == true) return;
        if (B_Break.value == true) return;
        if (B_Enraged.value == true)
        {
            F_RageMeter.value -= damage * 0.2f;
            if (F_RageMeter.value <= 0 && B_StatusBuildUp.value == true)
            {
                B_Enraged.value = false;
                F_RageMeter.value = 0;
                InitiateBreak?.Invoke();
            }
            return;
        }
        if (B_Enraged.value == false)
        {
            F_RageMeter.value += damage * 0.15f;
            if (F_RageMeter.value >= baseRageThreshold && B_StatusBuildUp.value == true)
            {
                F_RageMeter.value = baseRageThreshold;
                B_Enraged.value = true;
                InitiateEnrage?.Invoke();
                
            }
            return;

        }

    }
    public void AffectStun(float stunValue)
    {
        if (B_StatusBuildUp.value == false) return;
        if (B_Stunned.value == true) return;

        F_StunMeter.value += stunValue;
        if (F_StunMeter.value >= baseStunThreshold)
        {
            InitiateStun?.Invoke();
            F_StunMeter.value = 0;
        }
    
    }
    public void AffectPoison(float poisonValue)
    {
        if (B_Poisoned.value != false) return;

        F_PoisonMeter.value += poisonValue;
        if (F_PoisonMeter.value >= basePoisonThreshold)
        {
            InitiatePoison?.Invoke();
            F_PoisonMeter.value = 0;
        }

    }


    [field: Header("Modifier")]
    [field: SerializeField]public float BaseDamageModifier { get; private set; }
    [field: SerializeField]public float BaseWeakpointModifier { get; private set; }
    [field: SerializeField]public float BaseAnimationSpeed { get; private set; }
    [field: SerializeField]public float DamageModifier { get;private set; }
    [field: SerializeField]public float WeakpointModifier { get; private set; }
    [field: SerializeField]public float AnimationSpeed { get; private set; }
    public void Modifier(float damageModifier, float animationSpeed) //Used for Effect Statuses
    {
        DamageModifier = damageModifier / 100;
        AnimationSpeed = animationSpeed / 100;
    }


    [field: Header("Float Variables")]//Float variable
    [field: SerializeField][Required] public FloatVariable F_Stamina { get; private set; }
    [field: SerializeField][Required] public FloatVariable F_RageMeter { get; private set; }
    [field: SerializeField][Required] public FloatVariable F_StunMeter { get; private set; }
    [field: SerializeField][Required] public FloatVariable F_PoisonMeter { get; private set; }
#if UNITY_EDITOR //Editor Variable
  
    [SerializeField] private float _staminaMeter;
    [SerializeField] private float _rageMeter;
    [SerializeField] private float _stunMeter;
    [SerializeField] private float _poisonMeter;
#endif


    [field: Header("========DEBUG AREA READ ONLY========")]
    [field: Header("Animation")] //AnimatorComponent needs
    [ShowInInspector][ReadOnly]public int _animationHash;
    public delegate void OnAnimChange();
    public event OnAnimChange NotifyAnimChange;
    public bool NoFlip { get; private set; }
    public void SetNoFlip(bool noFlip) //Used for Substate or AnimatorComponent Only
    {
        this.NoFlip = noFlip;
    }
    public void SetAnimationHashAndNotify(int hash) //Used for States, Substate or AnimatorComponent Only
    {
        _animationHash = hash;
        NotifyAnimChange?.Invoke();
    }
    public int GetAnimationHashFromStatus() //Used for AnimatorComponent Only
    {
        return _animationHash;
    }
    public int GetAnimationHashFromSubstate()
    {
        return Substate.GetAnimation();
    }
    [field: Header("States READ ONLY")] //Enemy Behaviour needs
    [field: SerializeField][InlineEditor] public SO_Enemy_States States { get; private set;}
    [field: SerializeField][InlineEditor] public SO_Enemy_Substate Substate { get; private set; }
    [field: SerializeField][InlineEditor] public SO_Enemy_States PreviousState { get; private set; }

    public void SetPreviousState(SO_Enemy_States state) //Used for Behaviour Only
    {
        PreviousState = state;
    }
    public SO_Enemy_States GetPreviousState(List<FixedState> states) //Used for Behaviour Only 
    {  
        if(B_Enraged.value == true) 
        {
            foreach (FixedState condition in states)
            {
                if (condition.GetName() == EnemyStates.Raging)
                {
                    PreviousState = condition.state;
                }
            }
            return PreviousState;
        }
        else
        {
            foreach (FixedState condition in states)
            {
                if (condition.GetName() == EnemyStates.Normal)
                {
                    PreviousState = condition.state;
                }
            }
            return PreviousState;
        }
    }
    public SO_Enemy_States GetState() //Used for Behaviour Only
    {
        return States;
    }
    public void SetStateAndSubstate(SO_Enemy_States state, int index) //Used for Behaviour Only
    {
        States = state;
        Substate = state._subStates[index];
    }

    [Button("Reset to Default")]
    public override void OnSpawn() //Initialization
    {
        base.OnSpawn();
        DefaultModifier();
        IsMoving = false;
        IsAttacking = false;
        IsNextAttackReady = false;
        B_StatusBuildUp.value = true;
        B_Stunned.value = false;
        B_Enraged.value = false;
        B_Break.value = false;
        B_Poisoned.value = false;
        F_Stamina.value = baseStamina;
        F_RageMeter.value = 0; 
        F_StunMeter.value = 0; 
        F_PoisonMeter.value = 0;
        NoFlip = false;
        IsHalved = false;
    }
    public void DefaultModifier()
    {
        
        DamageModifier = BaseDamageModifier;
        WeakpointModifier = BaseWeakpointModifier;
        AnimationSpeed = BaseAnimationSpeed;
        B_Enraged.value = false;
        B_Break.value = false;
    }
#if UNITY_EDITOR //On Validate
    private void OnValidate()
    {
        _enraged = B_Enraged.value;
        _poisoned = B_Poisoned.value;
        _stunned = B_Stunned.value;
        _break = B_Break.value;
        _statusBuildUp = B_StatusBuildUp.value;
        _staminaMeter = F_Stamina.value;
        _rageMeter = F_RageMeter.value;
        _stunMeter = F_StunMeter.value;
        _poisonMeter = F_PoisonMeter.value;

    }
#endif

}

