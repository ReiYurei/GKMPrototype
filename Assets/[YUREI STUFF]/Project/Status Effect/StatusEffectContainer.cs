using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TriInspector;
using NUnit.Framework.Internal.Commands;
public class StatusEffectContainer : MonoBehaviour
{
    [ShowInInspector] public HashSet<BaseStatusEffect> appliedStatuses;
    public Enemy_Status status;

    [InlineEditor]
    [Header("Enemy Self-inflicting Status")]
    [Required][SerializeField] SO_Rage rage;
    [Required][SerializeField] SO_Stun stun;
    [Required][SerializeField] SO_Poison poison;
    [Required][SerializeField] SO_Break breakStatus;

    private void Start()
    {
        appliedStatuses = new HashSet<BaseStatusEffect>();

    }

    private void OnEnable()
    {
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
        Inflict(rage);
    }
    public void OnStun()
    {
        Inflict(stun);
    }
    public void OnPoison()
    {
        Inflict(poison);

    }
    public void OnBreak()
    {
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

