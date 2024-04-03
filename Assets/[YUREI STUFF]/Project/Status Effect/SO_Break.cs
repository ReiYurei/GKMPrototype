using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Break", menuName = "Status Ailment/Break")]
public class SO_Break : BaseStatusEffect
{
    [SerializeField] private float breakModifier;
    [SerializeField] private float breakAnimSpeed;
    [SerializeField] private float duration;
    private float time;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, SO_EnemyStatus status)
    {
        time = duration;
        time -= Time.deltaTime;
        while (time > 0)
        {
            status.B_Break.value = true;
            status.Modifier(breakModifier, breakAnimSpeed);
            time -= Time.deltaTime;
            yield return null;

        }
        status.B_Break.value = false;
        status.DefaultModifier();
        container.appliedStatuses.Remove(this);
    }

}
