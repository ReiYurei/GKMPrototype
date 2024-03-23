using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
public class StatusEffectContainer : MonoBehaviour
{
    [ShowInInspector] public HashSet<BaseStatusEffect> appliedStatuses;
    [Header("Ignore if Enemy component exist in object")]
    [ReadOnly] public Enemy _enemy;
    [SerializeField] public EnemyStatus _status;
    [Header("Main Field")]
    [Header("Enemy Self-inflicting Status")]
    [InlineEditor][Required][SerializeField] SO_Rage _rage;
    [InlineEditor][Required][SerializeField] SO_Stun _stun;
    [InlineEditor][Required][SerializeField] SO_Poison _poison;
    [InlineEditor][Required][SerializeField] SO_Break _breakStatus;
    public EventListener eventListener;

    private void Start()
    {
        appliedStatuses = new HashSet<BaseStatusEffect>();

    }

    private void OnEnable()
    {
        if (_status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            if (component == null)
            {
                Debug.LogError($"{this.GetType()} : Component type of {typeof(Enemy)} not found! " +
                    $"Please atleast provide a component type of  {typeof(Enemy)} or fill the status data with {typeof(EnemyStatus)}");
                return;
            }
            _enemy = component;
            _status = _enemy.status;
        }
        _status.InitiateEnrage += OnRage;
        _status.InitiateBreak += OnBreak;
        _status.InitiatePoison += OnPoison;
        _status.InitiateStun += OnStun;
    }
    private void OnDisable()
    {
        _status.InitiateEnrage -= OnRage;
        _status.InitiateBreak -= OnBreak;
        _status.InitiatePoison -= OnPoison;
        _status.InitiateStun -= OnStun;
    }

    public void OnRage()
    {
        if (_rage == null) return;
        Inflict(_rage);
    }
    public void OnStun()
    {
        if (_stun == null) return;
        Inflict(_stun);
    }
    public void OnPoison()
    {
        if (_poison == null) return;
        Inflict(_poison);
    }
    public void OnBreak()
    {
        if (_breakStatus == null) return;
        Inflict(_breakStatus);
    }
    public void Inflict(BaseStatusEffect effect)
    {
        if (appliedStatuses.Contains(effect))
        {
            return;
        }
        appliedStatuses.Add(effect);
        StartCoroutine(effect.ApplyEffect(this, _status));
    }
    public void NullifyAll()
    {
        StopAllCoroutines();
        appliedStatuses.Clear();
    }
    
}

