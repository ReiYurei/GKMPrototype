using System.Collections;
using TriInspector;
using UnityEngine;

public class HurtHandler : MonoBehaviour, IDamageable,IStatusInflictable
{
    [field: SerializeField] public Collider2D Collider { get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }
    public GameObject particlePrefab;
    GameObject[] particlePool;
    public float hitEffectDuration;
    public Color hitColor;
    public Color stunColor;
    public Color poisonColor;

    [SerializeField] private SpriteRenderer _parentRenderer;
    private SpriteRenderer _renderer;

    [Button("Debug : Hurt")]
    public void OnDamage(float damage, bool isGuardable = true)
    {
        Enemy.StatusData.AffectRage(damage);
        Enemy.StatusData.SetHealth(Enemy.StatusData.GetHealth() - (damage * Enemy.StatusData.WeakpointModifier));
        StopAllCoroutines();
        StartCoroutine(Hurt(hitColor));
    }
    public void OnStatusInflicted(float value, BaseStatusEffect effect)
    {
        switch (effect)
        {
            case SO_Stun:
                Enemy.StatusData.AffectStun(value);
                StopAllCoroutines();
                StartCoroutine(Hurt(stunColor));
                return;
            case SO_Poison:
                Enemy.StatusData.AffectPoison(value);
                StopAllCoroutines();
                StartCoroutine(Hurt(poisonColor));
                return;
            default:
                Enemy.StatusEffectContainerComponent.Inflict(effect);
                return;
        }
    }
    private void Start()
    {
        Collider = GetComponent<Collider2D>();
        Enemy = GetComponentInParent<Enemy>();
        _renderer = GetComponentInParent<SpriteRenderer>();
    }
    IEnumerator Hurt(Color color)
    {
        _renderer.enabled = true;
        _renderer.material.SetColor("_Color", color);
        float time = 0f;
        float speed;
        Color _color;
        AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        while(time < hitEffectDuration)
        {
            _renderer.sprite = _parentRenderer.sprite;
            time += Time.deltaTime;
            speed = curve.Evaluate(time / hitEffectDuration);
            _color = Color.Lerp(color, Color.clear, speed);
            _renderer.material.SetColor("_Color", _color);
            yield return null;
        }
        _renderer.enabled = false;

    }
}