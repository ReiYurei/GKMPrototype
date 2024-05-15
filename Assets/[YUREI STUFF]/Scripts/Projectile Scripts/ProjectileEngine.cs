using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using TriInspector;
using System.Collections;
using System;
#if UNITY_EDITOR
#endif

public enum PatternTypes
{
    Spread,
}

public class ProjectileEngine : MonoBehaviour
{
    [SerializeField] private string _projectileSound;

    [SerializeField][InlineEditor] private SO_Projectile_Data _data;
    private float _minAngle = -90f;
    private float _maxAngle = 90f;

    private float _defaultTargetAngle;
    [SerializeField] private float _targetAngle;

    private float _defaultAngleRange;
    [SerializeField] private float _angleRange;

    [Space(15)]
    private float _totalLifeTime;
    private float _loopDuration;
    private bool _isShooting;
    private bool _produceBullet;

    [SerializeField] private bool _canAim;
    [SerializeField] private SO_PlayerInfo _playerInfo;
    [ShowIf(nameof(_canAim), true)][SerializeField] private Vector2 _offset;

    [SerializeField] private Transform _projectileObj;
    private GameObject _container;
    [SerializeField] private List<Transform> _projectiles;
    [Space(15)]

    [SerializeField] private Transform _origin;

    [SerializeField] private ParticleSystem _particle;
    private GameObject _particleContainer;
    [SerializeField] private int particlePoolCount;
    [SerializeField] List<ParticleSystem> _particles;
    [Tooltip("Distance to player, to check whether or not the collission should start in shape or not to reduce" +
        " GC Allocation")]
    [SerializeField] private float _collissionDistance = 3f;
    public bool EndOfJob { get; private set; }

    private ProjectileJobSingle _jobProjectile;
    private JobHandle _jobHandleProjectile;

    private RotationJob _jobRotation;
    private JobHandle _jobHandleRotation;
    private NativeArray<float> _rotationResult;
    private NativeArray<float> _rotationDuration;
    private NativeArray<bool> _reversingRotation;

    private ChangeAngleRangeJob _jobAngleRange;
    private JobHandle _jobHandleAngleRange;
    private NativeArray<float> _angleRangeResult;
    private NativeArray<float> _angleChangeDuration;
    private NativeArray<bool> _reversingAngle;

    private float3 _originPos;
    private float3 _playerPos;

    private NativeArray<float> _distanceToPlayer;
    private NativeArray<float3> _originRadius;
    private NativeArray<float3> _projectilePos;
    private NativeArray<float3> _directionPos;
    private NativeArray<float> _projectDelayTime;
    private float[] _angle;
    private NativeArray<bool> _setActiveInfo;
    private NativeArray<bool> _hitPlayer;
    private NativeArray<float> _lifeTime;
    private NativeArray<float3> _direction;


    private void Awake()
    {
        float halfAngleRange = _angleRange / 2f;
        _minAngle = _targetAngle - halfAngleRange;
        _maxAngle = _targetAngle + halfAngleRange;
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }
        _defaultTargetAngle = _targetAngle;
        _defaultAngleRange = _angleRange;
        _particleContainer = new GameObject("Particle Pool");
        _container = new GameObject("Projectile Container");
        _container.transform.localPosition = Vector3.zero;

        if (_data.isLooping)
        {
            for (int i = 0; i < _data.numberOfShoot; i++)
            {
                for (int j = 0; j < _data.numberOfSegments; j++)
                {
                    var x = Instantiate(_projectileObj, _container.transform);
                    _projectiles.Add(x);
                    x.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < _data.numberOfShoot; i++)
            {
                for (int j = 0; j < _data.numberOfSegments; j++)
                {
                    var x = Instantiate(_projectileObj, _container.transform);
                    _projectiles.Add(x);
                    x.gameObject.SetActive(false);
                }
            }
        }

        if (_particle == null) return;
        for (int i = 0; i < particlePoolCount; i++)
        {
            var y = Instantiate(_particle, _particleContainer.transform);
            _particles.Add(y);
            y.gameObject.SetActive(false);
        }
    }
    public void InitializeData()
    {


        float delayedTime = 0f;
        float lifeTimespan = _data.projectileLifeTime;
        if (_canAim)
        {
            _targetAngle = Mathf.Atan2(_playerInfo.position.y - (_origin.position.y + _offset.y), _playerInfo.position.x - (_origin.position.x + _offset.x)) * Mathf.Rad2Deg;
        }
        TargetAngleRange();
        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (_data.numberOfSegments);
        for (int i = 0; i < _data.numberOfShoot; i++)
        {
            for (int j = 0; j < _data.numberOfSegments; j++)
            {
                float angle = _minAngle + j * angleIncrement;
                if (_data.numberOfSegments == 1)
                {
                    angle = _targetAngle;
                }
                _angle[i * _data.numberOfSegments + j] = angle;
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                if (i * _data.numberOfSegments + j < _projectiles.Count)
                {
                    _projectilePos[i * _data.numberOfSegments + j] = _origin.position;
                    _directionPos[i * _data.numberOfSegments + j] = direction;
                    _projectDelayTime[i * _data.numberOfSegments + j] = delayedTime;
                    _lifeTime[i * _data.numberOfSegments + j] = lifeTimespan;
                }
            }
            delayedTime += _data.delayBetweenShot;
        }
    }

    IEnumerator JobCoroutine()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        _produceBullet = true;
        float delay = 0;
        AudioManager.Instance.ProjectileCollection.Play_OneShot(_projectileSound);
        while (_isShooting == true)
        {
            if (_totalLifeTime <= 0 && _data.isLooping == false) break;
            if (_loopDuration <= 0 && _data.isLooping == true) break;
            if(_loopDuration <= _data.delayBetweenShot + _data.projectileLifeTime) _produceBullet = false;
            if (delay <= 0 && _produceBullet) 
            {
                AudioManager.Instance.ProjectileCollection.Play_OneShot(_projectileSound);
                delay = _data.delayBetweenShot;
            }
            delay -= Time.deltaTime;
            _loopDuration -= Time.deltaTime;
            _jobProjectile.duration = _loopDuration;
    

            _totalLifeTime -= Time.deltaTime;
            _jobProjectile.deltaTime = Time.deltaTime;
            _jobProjectile.originPos = (float3)_origin.position;
            _jobProjectile.playerPos = (float3)_playerInfo.position;
            BulletDirection();
            BulletAngle();
            BulletRotation();
            BulletPattern();
            if (_loopDuration < _data.loopDuration * _data.delayBetweenShot)
            {
                EndOfJob = true;
            }
            yield return null;
        }
        EndOfJob = true;
        ResetToDefault();
        JobEnd();
        yield break;
    }

    void ResetToDefault()
    {
        _targetAngle = _defaultTargetAngle;
        _angleRange = _defaultAngleRange;
    }

    void BulletRotation()
    {
        if (!_data.enableAutoRotation) return;
        _jobRotation.deltaTime = Time.deltaTime;
        _jobHandleRotation = _jobRotation.Schedule();
        _jobHandleRotation.Complete();
        _targetAngle = _rotationResult[0];
    }
    void BulletAngle()
    {
        if (!_data.enableAutoAngle) return;
        _jobAngleRange.deltaTime = Time.deltaTime;
        _jobHandleAngleRange = _jobAngleRange.Schedule();
        _jobHandleAngleRange.Complete();
        _angleRange = _angleRangeResult[0];

    }
    void BulletDirection()
    {

        float totalAngleRange;
        float angleIncrement;
        float angle;
        if (_canAim)
        {
            _targetAngle = Mathf.Atan2(_playerInfo.position.y - (_origin.position.y + _offset.y), _playerInfo.position.x - (_origin.position.x + _offset.x)) * Mathf.Rad2Deg;
        }
        for (int j = 0; j < _data.numberOfSegments; j++)
        {
            totalAngleRange = _maxAngle - _minAngle;
            angleIncrement = totalAngleRange / (_data.numberOfSegments);
            angle = _minAngle + j * angleIncrement;
 
            if (_data.numberOfSegments == 1)
            {
                angle = _targetAngle;
            }
            float3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            _jobProjectile.direction[j] = direction;

        }
    }
    Collider2D x;

    void BulletPattern()
    {
        _jobHandleProjectile = _jobProjectile.Schedule(_projectiles.Count, 64);
        _jobHandleProjectile.Complete();
        for (int i = 0; i < _projectiles.Count; i++)
        {
            if (_setActiveInfo[i] == false) continue;
            if (_hitPlayer[i] == true) continue;
            if (_lifeTime[i] >= _loopDuration)
            {
                _setActiveInfo[i] = false;
                _projectiles[i].gameObject.SetActive(false);
                continue;
            }
            if (_lifeTime[i] <= 0)
            {
                _setActiveInfo[i] = false;
                _projectiles[i].gameObject.SetActive(false);
                continue;
            }
    

            _projectiles[i].gameObject.SetActive(_setActiveInfo[i]);
            _projectiles[i].position = (Vector3)_projectilePos[i];//The first batch of position didnt get separated from the second batch
            _projectiles[i].rotation = Quaternion.Euler(0f, 0f, _angle[i]);
            if (_projectiles[i].position.x < CameraBorder.Instance.defaultLeftWallAnchor.x |
            _projectiles[i].position.x > CameraBorder.Instance.defaultRightWallAnchor.x |
            _projectiles[i].position.y > CameraBorder.Instance.defaultUpperWallAnchor.y |
            _projectiles[i].position.y < CameraBorder.Instance.defaultBottomWallAnchor.y)
            {
                _setActiveInfo[i] = false;
                _hitPlayer[i] = true;
                _projectiles[i].gameObject.SetActive(false);

                continue;
            }
            if (_distanceToPlayer[i] > _collissionDistance) continue;
            x = Physics2D.OverlapCircle((Vector3)_projectilePos[i], 0.15f, LayerMask.GetMask("Player")); //This Creates GC Alloc
            if (x != null)
            {
                _setActiveInfo[i] = false;
                _hitPlayer[i] = true;
                _projectiles[i].gameObject.SetActive(false);
                ParticlePool(i);
                if (x.TryGetComponent(out IDamageable damaging))
                {
                    damaging.OnDamage(_data.damage, _data.isGuardable);
                }
            }
        }
    }
    public void DeactiveAllParticle()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            if (!_projectiles[i].gameObject.activeInHierarchy) continue;
            ParticlePool(i);
            _projectiles[i].gameObject.SetActive(false);

        }
        _loopDuration = 0;
        _totalLifeTime = 0;
        JobEnd();
        EndOfJob = true;
        _isShooting = false;
        StopAllCoroutines();

    }
    public void DeactiveTemporary()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            if (!_projectiles[i].gameObject.activeInHierarchy) continue;
            if (_setActiveInfo[i] == false) continue;
            if (_hitPlayer[i] == true) continue;
            _setActiveInfo[i] = false;
            _hitPlayer[i] = true;
            _projectiles[i].gameObject.SetActive(false);
            ParticlePool(i);
        }

    }
    void ParticlePool(int index)
    {
        if (_particle == null || _particles == null || _particles.Count <= 0) return;
        foreach (ParticleSystem particle in _particles)
        {
            if (!particle.gameObject.activeInHierarchy)
            {
                particle.gameObject.transform.position = (Vector3)(_projectilePos[index]);
                particle.gameObject.SetActive(true);
                return;
            }
        }
    }
    void JobEnd()
    {
        _isShooting = false;
        DisposeAll();
        for (int i = 0; i < _projectiles.Count; i++)
        {
            _projectiles[i].gameObject.SetActive(false);
        }
    }

    public void DisposeAll()
    {
        if (_data.enableAutoRotation)
        {
            _rotationResult.Dispose();
            _rotationDuration.Dispose();
            _reversingRotation.Dispose();
            _rotationResult = default;
            _rotationDuration = default;
            _reversingRotation = default;
            Debug.Log("AutoRotationDisposed");
        }
        if (_data.enableAutoAngle)
        {
            _angleRangeResult.Dispose();
            _angleChangeDuration.Dispose();
            _reversingAngle.Dispose();
            _angleRangeResult = default;
            _angleChangeDuration = default;
            _reversingAngle = default;
            Debug.Log("AutoAngleDisposed");

        }
        if (_projectilePos.IsCreated != true || _directionPos.IsCreated != true || _projectDelayTime.IsCreated != true
            || _lifeTime.IsCreated != true || _setActiveInfo.IsCreated != true || _hitPlayer.IsCreated != true || _direction.IsCreated != true
            || _originRadius.IsCreated != true || _distanceToPlayer.IsCreated != true
            ) return;

        _isShooting = false;
        _distanceToPlayer.Dispose();
        _originRadius.Dispose();
        _projectilePos.Dispose();
        _directionPos.Dispose();
        _projectDelayTime.Dispose();
        _lifeTime.Dispose();
        _setActiveInfo.Dispose();
        _hitPlayer.Dispose();
        _direction.Dispose();
        _distanceToPlayer = default;
        _originRadius = default;
        _projectilePos = default;
        _directionPos = default;
        _projectDelayTime = default;
        _lifeTime = default;
        _setActiveInfo = default;
        _hitPlayer = default;
        _direction = default;
        Debug.Log("Disposed");


    }
    public IEnumerator Shoot()
    {
        EndOfJob = false;
        if (_isShooting) yield break;
        for (int i = 0; i < _projectiles.Count; i++)
        {
            _projectiles[i].transform.position = _origin.position;
        }
        _distanceToPlayer = new NativeArray<float>(_projectiles.Count, Allocator.Persistent);
        _originRadius = new NativeArray<float3>(_projectiles.Count, Allocator.Persistent);
        _projectilePos = new NativeArray<float3>(_projectiles.Count, Allocator.Persistent);
        _directionPos = new NativeArray<float3>(_projectiles.Count, Allocator.Persistent);
        _projectDelayTime = new NativeArray<float>(_projectiles.Count, Allocator.Persistent);
        _lifeTime = new NativeArray<float>(_projectiles.Count, Allocator.Persistent);
        _setActiveInfo = new NativeArray<bool>(_projectiles.Count, Allocator.Persistent);
        _hitPlayer = new NativeArray<bool>(_projectiles.Count, Allocator.Persistent);
        _direction = new NativeArray<float3>(_data.numberOfSegments, Allocator.Persistent);
        _angle = new float[_projectiles.Count];
        InitializeData();
        if (_data.enableAutoRotation)
        {
            _rotationResult = new NativeArray<float>(1, Allocator.Persistent);
            _rotationDuration = new NativeArray<float>(1, Allocator.Persistent);
            _reversingRotation = new NativeArray<bool>(1, Allocator.Persistent);
            _rotationDuration[0] = _data.rotationDuration;
            _rotationResult[0] = _targetAngle;
            _jobRotation = new RotationJob()
            {
                targetAngle = _targetAngle,
                rotationSpeed = _data.rotationSpeed,
                rotationDegree = _data.rotationDegree,
                reversingRotation = _reversingRotation,
                rotationDuration = _rotationDuration,
                targetAngleResult = _rotationResult,
                minRotationChange = _data.minRotationChange,
                maxRotationChange = _data.maxRotationChange,
                canReverseRotation = _data.canReverseRotation,
                fixedDeltaTime = 0.02f,

            };
        }
        if (_data.enableAutoAngle)
        {

            _angleRangeResult = new NativeArray<float>(1, Allocator.Persistent);
            _reversingAngle = new NativeArray<bool>(1, Allocator.Persistent);
            _angleChangeDuration = new NativeArray<float>(1, Allocator.Persistent);
            _angleChangeDuration[0] = _data.angleChangeDuration;
            _jobAngleRange = new ChangeAngleRangeJob()
            {
                angleRange = _angleRange,
                angleDegree = _data.angleDegree,
                angleChangeSpeed = _data.angleChangeSpeed,
                angleChangeDuration = _angleChangeDuration,
                angleRangeResult = _angleRangeResult,
                maxAngleChange = _data.maxAngleChange,
                minAngleChange = _data.minAngleChange,
                canReverseAngle = _data.canReverseAngle,
                reversingAngle = _reversingAngle,
                fixedDeltaTime = 0.02f,

            };
        }
        _jobProjectile = new ProjectileJobSingle
        {
            distanceToPlayer = _distanceToPlayer,
            playerPos = _playerPos,
            originPos = _originPos,
            originRadius = _originRadius,
            direction = _direction,
            hitPlayer = _hitPlayer,
            position = _projectilePos,
            directionPos = _directionPos,
            delayTime = _projectDelayTime,
            setActive = _setActiveInfo,
            speed = _data.projectileSpeed,
            duration = _data.loopDuration,
            spawnRadius = _data.spawnRadius,
            isLooping = _data.isLooping,
            defaultDelayTime = _data.delayBetweenShot,
            lifeTime = _lifeTime,
            defaultLifeTime = _data.projectileLifeTime,
            segment = _data.numberOfSegments,
            fixedDeltaTime = 0.02f,

        };
        _totalLifeTime = _data.numberOfShoot * _data.delayBetweenShot + _data.projectileLifeTime;
        _loopDuration = _data.loopDuration;
        _isShooting = true;
        yield return StartCoroutine(JobCoroutine());
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        // Calculate the half angle range from the _target angle
        float halfAngleRange = _angleRange / 2f;

        // Adjust the min and max angles based on the _target angle and half angle range
        _minAngle = _targetAngle - halfAngleRange;
        _maxAngle = _targetAngle + halfAngleRange;

        // Calculate the angle increment for each segment

        // Ensure _minAngle is less than _maxAngle
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }

    }



    private void OnDisable()
    {
        DisposeAll();
    }

    void OnDrawGizmos()
    {
        DivideAngle();
    }
#endif
    private void Update()
    {
        if (!_isShooting) return;
        // Calculate the half angle range from the _target angle
        TargetAngleRange();

    }

    private void TargetAngleRange()
    {
        float halfAngleRange = _angleRange / 2f;
        // Adjust the min and max angles based on the _target angle and half angle range
        _minAngle = _targetAngle - halfAngleRange;
        _maxAngle = _targetAngle + halfAngleRange;

        // Calculate the angle increment for each segment

        // Ensure _minAngle is less than _maxAngle
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }
    }

    void DivideAngle()
    {

        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (_data.numberOfSegments);
        for (int i = 0; i < _data.numberOfSegments; i++)
        {
            float angle = _minAngle + i * angleIncrement;
            if (_data.numberOfSegments == 1)
            {
                _targetAngle = (_minAngle + _maxAngle) / 2f;
                angle = _targetAngle;
            }
            Vector3 start = Quaternion.Euler(0, 0, angle) * Vector3.right * 2f;
            Vector3 end = Quaternion.Euler(0, 0, angle) * Vector3.right * 3f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + start, transform.position + end);
        }
        if (!_isShooting)
        {
            return;
        }
        UnityEngine.Color color = UnityEngine.Color.blue;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(_playerInfo.position, _collissionDistance);

        for (int i = 0; i < _projectiles.Count; i++)
        {
            if (_distanceToPlayer[i] > _collissionDistance) continue;
            Gizmos.DrawWireSphere((Vector3)_projectilePos[i], 0.15f);
        }
    }
}

