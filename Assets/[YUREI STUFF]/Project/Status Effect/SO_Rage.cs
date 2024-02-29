using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Rage", menuName = "Status Ailment/Rage")]
public class SO_Rage : BaseStatusEffect
{
    [SerializeField] private float rageModifier;
    [SerializeField] private float rageAnimSpeed;
    [SerializeField] private float rageWaitTime;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, Enemy_Status status)
    {
        while (status._enraged.value == true)
        {
            status.Modifier(rageModifier, rageAnimSpeed, rageWaitTime);
            yield return null;
        }
        container.appliedStatuses.Remove(this);
    }

}
