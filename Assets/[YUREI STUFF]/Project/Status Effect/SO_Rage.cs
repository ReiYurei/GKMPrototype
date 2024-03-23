using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Rage", menuName = "Status Ailment/Rage")]
public class SO_Rage : BaseStatusEffect
{
    [SerializeField] private float rageModifier;
    [SerializeField] private float rageAnimSpeed;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, EnemyStatus status)
    {
        while (status.b_Enraged.value == true)
        {
            status.Modifier(rageModifier, rageAnimSpeed);
            yield return null;
        }
        container.appliedStatuses.Remove(this);
    }

}
