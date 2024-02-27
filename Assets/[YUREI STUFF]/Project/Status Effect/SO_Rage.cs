using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Rage", menuName = "Status Ailment/Rage")]
public class SO_Rage : BaseStatusEffect
{
    [SerializeField] private float rageModifier;
    [SerializeField] private float rageAnimSpeed;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, Enemy_Status status)
    {
        while (status._enraged.value == true)
        {
            status.RageModifier(rageModifier, rageAnimSpeed);
            yield return null;
        }
        Debug.Log("nomore ngamok");
        container.appliedStatuses.Remove(this);
    }

}
