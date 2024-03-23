using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStatusEffect : ScriptableObject
{
    public abstract IEnumerator ApplyEffect(StatusEffectContainer container, EnemyStatus status);

}
