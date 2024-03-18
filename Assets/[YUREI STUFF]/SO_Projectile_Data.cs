using UnityEngine;

#if UNITY_EDITOR
#endif
[CreateAssetMenu(fileName = "Projectile", menuName = "Enemy/Projectile/Projectile Data")]
public class SO_Projectile_Data : ScriptableObject
{
    public PatternTypes types;
    [Range(1, 90)] public int numberOfSegments;
    public float projectileSpeed;
    public int numberOfShoot;
    public float delayBetweenShot;
    public float projectileLifeTime;
}

      




