using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [field:SerializeField] public Collider2D Collider {  get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }
    private SO_EnemyStatus _status;
    private void Start()
    {
        Collider = GetComponent<Collider2D>();
        Enemy = GetComponentInParent<Enemy>();
        _status = Enemy.StatusData;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out IDamageable damageable);
        if (damageable != null && collision.CompareTag("Player"))
        {
            float damage = _status.RawPower * (_status.MotionValue / 100) * _status.DamageModifier;
            damageable.OnDamage(damage,_status.isGuardable);
        }
    }
}
