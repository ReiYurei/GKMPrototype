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
            time -= Time.deltaTime;
            Debug.Log($"Stunned for {time} seconds");
            yield return null;

        }
        status._stunned.value = false;
        container.appliedStatuses.Remove(this);

    }
}