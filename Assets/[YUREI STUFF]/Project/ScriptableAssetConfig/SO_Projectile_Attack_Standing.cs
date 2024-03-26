using System.Collections;
using TriInspector;
#if UNITY_EDITOR

#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Projectile_Standing", menuName = "Enemy/Moveset/Projectile/Standing")]
public class SO_Projectile_Attack_Standing : SO_Base_Attack_Fixed
{
    [InlineEditor]public SO_Projectile_Data projectileInfo;
    public bool triggerShot;
    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.StatusData.NotifyShooting(true);
        yield return new WaitUntil(() => enemy.StatusData.IsShooting == false);
        //Notify Attack false at Super Projectile Engine
    }

    public override int GetAnimation()
    {
        return AnimationHash.Enemy_Projectile((int)projectile);
    }

}
