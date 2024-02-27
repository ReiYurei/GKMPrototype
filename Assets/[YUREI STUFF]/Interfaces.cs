using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage);

}

public interface IStatusInflictable
{
    void OnStatusInflicted(float value, BaseStatusEffect effect);

}
