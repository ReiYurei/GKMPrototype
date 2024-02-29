using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TriInspector;
using NUnit.Framework.Internal.Commands;
using System.ComponentModel;
public class StatusEffectContainer : MonoBehaviour
{
    [ShowInInspector] public HashSet<BaseStatusEffect> appliedStatuses;
    Enemy_Status status;

    [Header("Enemy Self-inflicting Status")]
    [InlineEditor][Required][SerializeField] SO_Rage rage;
    [InlineEditor][Required][SerializeField] SO_Stun stun;
    [InlineEditor][Required][SerializeField] SO_Poison poison;
    [InlineEditor][Required][SerializeField] SO_Break breakStatus;

    private void Start()
    {
        appliedStatuses = new HashSet<BaseStatusEffect>();

    }

    private void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            status = component._status;

        }
        status.InitiateEnrage += OnRage;
        status.InitiateBreak += OnBreak;
        status.InitiatePoison += OnPoison;
        status.InitiateStun += OnStun;
    }
    private void OnDisable()
    {
        status.InitiateEnrage -= OnRage;
        status.InitiateBreak -= OnBreak;
        status.InitiatePoison -= OnPoison;
        status.InitiateStun -= OnStun;
    }

    public void OnRage()
    {
        if (rage == null) return;
        Inflict(rage);
    }
    public void OnStun()
    {
        if (stun == null) return;
        Inflict(stun);
    }
    public void OnPoison()
    {
        if (poison == null) return;
        Inflict(poison);
    }
    public void OnBreak()
    {
        if (breakStatus == null) return;
        Inflict(breakStatus);
    }
    public void Inflict(BaseStatusEffect effect)
    {
        if (appliedStatuses.Contains(effect))
        {
            return;
        }
        appliedStatuses.Add(effect);
        StartCoroutine(effect.ApplyEffect(this, status));
    }
    public void NullifyAll()
    {
        StopAllCoroutines();
        appliedStatuses.Clear();
    }
    
}

