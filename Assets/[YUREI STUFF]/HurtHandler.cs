using System.Collections;
using TriInspector;
using UnityEngine;
using System.Collections.Generic;

public class HurtHandler : MonoBehaviour, IDamageable,IStatusInflictable , IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public Collider2D Collider { get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }


    public ParticleSystem particle;
    public GameObject particleContainer;
    private List<ParticleSystem> _particlePool;
    public int poolCount;
    public float hitEffectDuration;
    public Vector3 offset;
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
        AudioCollection.Play_OneShot("Hurt");
        Particle();
    }
    public void OnStatusInflicted(float value, BaseStatusEffect effect)
    {
        switch (effect)
        {
            case SO_Stun:
                Enemy.StatusData.AffectStun(value);
                StopAllCoroutines();
                StartCoroutine(Hurt(stunColor));
                AudioManager.Instance.GenericSoundCollection.Play_OneShot("Stun");
                return;
            case SO_Poison:
                Enemy.StatusData.AffectPoison(value);
                StopAllCoroutines();
                StartCoroutine(Hurt(poisonColor));
                AudioManager.Instance.GenericSoundCollection.Play_OneShot("Poison");
                return;
            default:
                Enemy.StatusEffectContainerComponent.Inflict(effect);
                return;
        }
    }
    private void Start()
    {
        if (AudioCollection != null) AudioCollection.InitializeStartData();
        Collider = GetComponent<Collider2D>();
        Enemy = GetComponentInParent<Enemy>();
        _renderer = GetComponentInParent<SpriteRenderer>();
        particleContainer = Instantiate(particleContainer);
        for(int i = 0; i < poolCount; i++)
        {
            _particlePool ??= new List<ParticleSystem>();
            var obj = Instantiate(particle, particleContainer.transform);
            obj.gameObject.SetActive(false);
            _particlePool.Add(obj);
        }
    }
    private void Particle()
    {
        for(int i = 0; i < _particlePool.Count; i++)
        {
      
            if (_particlePool[i].gameObject.activeInHierarchy) continue;
            _particlePool[i].transform.position = transform.position + offset;
            _particlePool[i].gameObject.SetActive(true);
            return;
        }
        var obj = Instantiate(particle, particleContainer.transform);
        obj.gameObject.SetActive(true);
        _particlePool.Add(obj);

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