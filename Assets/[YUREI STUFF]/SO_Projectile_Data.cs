using UnityEngine;

#if UNITY_EDITOR
#endif
[CreateAssetMenu(fileName = "Projectile", menuName = "Enemy/Projectile/Projectile Data")]
public class SO_Projectile_Data : ScriptableObject
{

    [Range(1, 90)] public int numberOfSegments;
    public float projectileSpeed;
    public int numberOfShoot;
    public float delayBetweenShot;
    public float projectileLifeTime;
}

      




