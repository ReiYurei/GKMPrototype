using System.Collections;
using System.Collections.Generic;
using TriInspector;
#if UNITY_EDITOR

#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Projectile_Standing", menuName = "Enemy/Moveset/Projectile/Standing")]
public class SO_Projectile_Attack_Base : SO_Base_Attack_Fixed
{
    [InlineEditor]public List<SO_VoidGameEvent> projectileEvents;
    public float delay;
    public bool waitUntilShootingDone;
    public bool hasAnimation;
    [ShowIf(nameof(hasAnimation), true)]public ProjectileSlot projectile;


    public override IEnumerator Execute(Enemy enemy)
    {
        if (waitUntilShootingDone)
        {
            enemy.StatusData.NotifyShooting(true);
            enemy.StartCoroutine(Shoot());
            yield return new WaitUntil(() => enemy.StatusData.IsShooting == false);        //Notify Attack false at Super Projectile Engine
        }
        else enemy.StartCoroutine(Shoot());
    }
    IEnumerator Shoot()
    {
        float time;

        for(int i = 0; i < projectileEvents.Count; i++)
        {
            time = 0f;
            projectileEvents[i].Raise();
            while (time < delay)
            {
                time += Time.deltaTime;
                yield return null;
            }

        }
    }
    public override int GetAnimation()
    {
        if(hasAnimation) return AnimationHash.Enemy_Projectile((int)projectile);
        return AnimationHash.Enemy_Idle;
    }

}
