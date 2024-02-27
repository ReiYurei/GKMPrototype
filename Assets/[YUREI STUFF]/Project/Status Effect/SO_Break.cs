using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Break", menuName = "Status Ailment/Break")]
public class SO_Break : BaseStatusEffect
{
    [SerializeField] private float breakModifier;
    [SerializeField] private float breakAnimSpeed;
    [SerializeField] private float duration;
    private float time;
    public override IEnumerator ApplyEffect(StatusEffectContainer container, Enemy_Status status)
    {
        time = duration;
        time -= Time.deltaTime;
        while (time > 0)
        {
            Debug.Log(time);
            status._break.value = true;
            status.RageModifier(breakModifier, breakAnimSpeed);
            time -= Time.deltaTime;
            yield return null;

        }
        status._break.value = false;
        status.DefaultModifier();
        container.appliedStatuses.Remove(this);
    }

}
