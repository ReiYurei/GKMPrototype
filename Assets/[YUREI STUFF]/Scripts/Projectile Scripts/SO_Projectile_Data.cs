using UnityEngine;
using TriInspector;
#if UNITY_EDITOR
#endif
[CreateAssetMenu(fileName = "Projectile", menuName = "Enemy/Projectile/Projectile Data")]
public class SO_Projectile_Data : ScriptableObject
{
    public PatternTypes types;

    [Tooltip("If Looping toogled on, it'll mostly use the loop duration than a total number of shoot")]
    public int damage;
    public bool isLooping;
    [HideIf(nameof(isLooping), false)] public float loopDuration;

    [Range(1, 72)] public int numberOfSegments;
    public int numberOfShoot;
    public float projectileSpeed;
    public float delayBetweenShot;
    public float projectileLifeTime;

    [Header("Rotation Properties")]
    public bool enableAutoRotation;
    [ShowIf(nameof(enableAutoRotation), true)] public float rotationDegree;
    [ShowIf(nameof(enableAutoRotation), true)] public float rotationSpeed;
    [ShowIf(nameof(enableAutoRotation), true)] public float rotationDuration;
    [ShowIf(nameof(enableAutoRotation), true)] public float maxRotationChange;
    [ShowIf(nameof(enableAutoRotation), true)] public float minRotationChange;
    [ShowIf(nameof(enableAutoRotation), true)] public bool canReverseRotation;


    [Header("Angle Properties")]
    public bool enableAutoAngle;
    [ShowIf(nameof(enableAutoAngle), true)] public float angleDegree;
    [ShowIf(nameof(enableAutoAngle), true)] public float angleChangeSpeed;
    [ShowIf(nameof(enableAutoAngle), true)] public float angleChangeDuration;
    [ShowIf(nameof(enableAutoAngle), true)] public float maxAngleChange;
    [ShowIf(nameof(enableAutoAngle), true)] public float minAngleChange;
    [ShowIf(nameof(enableAutoAngle), true)] public bool canReverseAngle;

}






