using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Poison", menuName = "Status Ailment/Poison")]
public class SO_Poison : BaseStatusEffect
{
    [SerializeField] private float duration;
    [SerializeField] private float poisonPercentage;
    private float time;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, SO_EnemyStatus status)
    {
        time = duration;
        while (time > 0)
        {
            time -= Time.deltaTime;
            status.SetHealth(status.GetHealth() - poisonPercentage  / 100 * Time.deltaTime);
            yield return null;

        }
        status.NotifyEndOfStatus(this);
        container.appliedStatuses.Remove(this);
    }
    }