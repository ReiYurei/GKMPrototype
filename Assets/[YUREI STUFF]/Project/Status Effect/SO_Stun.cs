using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun",menuName ="Status Ailment/Stun")]
public class SO_Stun : BaseStatusEffect
{
    [SerializeField] private float duration;
    private float time;

    public override IEnumerator ApplyEffect(StatusEffectContainer container, Enemy_Status status)
    { 
        time = duration;
        time -= Time.deltaTime;
        while (time > 0)
        {
            status._stunned.value = true;
            status._statusBuildUp.value = false;
            time -= Time.deltaTime;
            yield return null;

        }
        status._statusBuildUp.value = true;
        status._stunned.value = false;
        status.NotifyEndOfStatus(this);
        container.appliedStatuses.Remove(this);

    }
}