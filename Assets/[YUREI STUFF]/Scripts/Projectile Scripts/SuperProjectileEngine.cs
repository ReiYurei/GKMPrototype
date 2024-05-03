using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperProjectileEngine : MonoBehaviour, IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }

    [SerializeField] private List<ProjectileEngine> _engines;
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _curveHandle;
    private Vector3 _curveHandlePos;
    [SerializeField] private bool _useRandomHandle;
    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0f,0f,1f,1f);
    [SerializeField] private float _timeToReachPoint = 2f;
    private float _moveSpeed;
    private float _time;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private bool _consecutiveShoot;
    [SerializeField] private bool _canShootWhileMoving;
    [SerializeField] private bool _returnAfterShootEnd;
    [Tooltip("The Super Projectile Engine can damage player")]
    [SerializeField] private bool _dealingDamage;
    [Tooltip("Only Used if dealing damage is on")]
    [SerializeField] private float _damage;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Start()
    {
        _collider = TryGetComponent(out Collider2D collider) ? _collider = collider : null;
        _spriteRenderer = TryGetComponent(out SpriteRenderer spriteRenderer) ? _spriteRenderer = spriteRenderer : null;
        var child = GetComponentsInChildren<ProjectileEngine>();
        for (int i = 0; i < child.Length; i++)
        {
            _engines.Add(child[i]);
        }
        _spriteRenderer.enabled = false;
        _collider.enabled = false;

    }
    public void DeactiveAllProjectileEngine()
    {
        foreach (var engine in _engines)
        {
            engine.DeactiveAllParticle();
        }
    }
    void CurveHandleEnd()
    {
        if (!_useRandomHandle)
        {
            _curveHandlePos = _curveHandle.position;
            return;
        }
        _curveHandlePos = new Vector3(Random.Range(transform.position.x - 4, Mathf.Abs(transform.position.x + 4)), Random.Range(transform.position.y - 3, (Vector3.Distance(transform.position, _origin.position) * 0.35f)), 0f);
        if (_curveHandle != null) _curveHandle.position = _curveHandlePos;

    }
    void CurveHandleStart()
    {
        if (!_useRandomHandle)
        {
            _curveHandlePos = _curveHandle.position;
            return;
        }
        _curveHandlePos = new Vector3(Random.Range(_origin.localPosition.x - 4, Mathf.Abs(_origin.localPosition.x + 4)), Random.Range(_origin.localPosition.y - 3, (Vector3.Distance(_origin.position, _target.position) * 0.35f)), 0f);
        if (_curveHandle != null) _curveHandle.position = _curveHandlePos;
    }

    public void OnProjectileInitiate()
    {
        CurveHandleStart();
        transform.position = _origin.position + _offset;
        if (_spriteRenderer != null) _spriteRenderer.enabled = true;
        if(_dealingDamage) _collider.enabled = true;
        StartCoroutine(EngineCoroutine());
    }
    IEnumerator EngineCoroutine()
    {
        if (_target != null && !_canShootWhileMoving)
        {
            yield return StartCoroutine(EngineMove());
            StartCoroutine(EngineStart());
            yield break;
        }
        StartCoroutine(EngineMove());
        StartCoroutine(EngineStart());


    }
    IEnumerator EngineMove()
    {
        _time = 0f;
        Vector3 originTempPos = _origin.position + _offset;
        while (transform.position != _target.position)
        {
            _time += Time.deltaTime;
            _moveSpeed = _speedCurve.Evaluate(_time / _timeToReachPoint);
            Vector3 ab = Vector3.Lerp(originTempPos, _curveHandlePos, _moveSpeed);
            Vector3 bc = Vector3.Lerp(_curveHandlePos, _target.position, _moveSpeed);
            transform.position = Vector3.Lerp(ab, bc, _moveSpeed);
            yield return null;
        }
        yield break;
    }
    IEnumerator EngineReturn() 
    {
        _time = 0f;
        Vector3 originTempPos = transform.position;
        if (!_returnAfterShootEnd)
        {
            if (_dealingDamage) _collider.enabled = false;
            _spriteRenderer.enabled = false;
            yield break;
        }
        while (transform.position != _origin.position + _offset)
        {
            _time += Time.deltaTime;
            _moveSpeed = _speedCurve.Evaluate(_time / _timeToReachPoint);
            Vector3 ab = Vector3.Lerp(originTempPos, _curveHandlePos, _moveSpeed);
            Vector3 bc = Vector3.Lerp(_curveHandlePos, _origin.position + _offset, _moveSpeed);
            transform.position = Vector3.Lerp(ab, bc, _moveSpeed);
            yield return null;
        }
        if (_dealingDamage) _collider.enabled = false;
        _spriteRenderer.enabled = false;
        yield break;
    }
    IEnumerator EngineStart()
    {
        if(_consecutiveShoot)
        {
            for (int i = 0;i < _engines.Count;i++)
            {    
                StartCoroutine(_engines[i].Shoot());
                if (i == _engines.Count - 1)
                {
                    CurveHandleEnd();
                    yield return new WaitUntil(() => _engines[i].EndOfJob);
                    _enemy.StatusData.NotifyShooting(false);
                    StartCoroutine(EngineReturn());
                    yield break;
                }
                continue;
            }
            yield break;
        }
        for (int i = 0; i < _engines.Count; i++)
        {
            yield return StartCoroutine(_engines[i].Shoot());
            if (i == _engines.Count - 1)
            {
                yield return new WaitUntil(() => _engines[i].EndOfJob);
                _enemy.StatusData.NotifyShooting(false);
                StartCoroutine(EngineReturn());
                yield break;
            }
        }
        yield break;
    }

}
