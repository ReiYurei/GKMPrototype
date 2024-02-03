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
    private void Start()
    {
        appliedStatuses = new HashSet<BaseStatusEffect>();

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

