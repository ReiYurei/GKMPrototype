using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun",menuName ="Status Ailment/Stun")]
public class SO_Stun : BaseStatusEffect
{
    [SerializeField] private float duration;
    private float time;

    public override IEnumerator ApplyEffect(StatusEffectContainer container, EnemyStatus status)
    { 
        time = duration;
        time -= Time.deltaTime;
        while (time > 0)
        {
            status.B_Stunned.value = true;
            status.B_StatusBuildUp.value = false;
            time -= Time.deltaTime;
            yield return null;

        }
        status.B_StatusBuildUp.value = true;
        status.B_Stunned.value = false;
        status.NotifyEndOfStatus(this);
        container.appliedStatuses.Remove(this);

    }
}