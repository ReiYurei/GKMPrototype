using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using TriInspector;
using System.Collections;
#if UNITY_EDITOR
#endif

public enum PatternTypes
{
    Spread,
}

public class ProjectileEngine : MonoBehaviour
{

    [InlineEditor]public SO_Projectile_Data data;
    float _minAngle = -90f;
    float _maxAngle = 90f;
    float _defaultTargetAngle;
    public float targetAngle;

    float _defaultAngleRange;
    public float angleRange;
    [Space(15)]
    float _totalLifeTime;
    public float _loopDuration;
    bool _isShooting;
    public bool canAim;
    [ShowIf(nameof(canAim), true)] public SO_PlayerInfo playerInfo;
    [ShowIf(nameof(canAim), true)] public Vector2 offset;

    public Transform projectile;
    GameObject container;
    [SerializeField] List<Transform> _projectiles;
    [Space(15)]

    public Transform origin;

    public GameObject particle;
    GameObject particleContainer;
    public int particlePoolCount;
    [SerializeField] List<GameObject> _particles;

    public bool endOfJob;

    ProjectileJobSingle _jobProjectile;
    JobHandle _jobHandleProjectile;

    RotationJob _jobRotation;
    JobHandle _jobHandleRotation;
    NativeArray<float> _rotationResult;
    NativeArray<float> _rotationDuration;
    NativeArray<bool> _reversingRotation;

    ChangeAngleRangeJob _jobAngleRange;
    JobHandle _jobHandleAngleRange;
    NativeArray<float> _angleRangeResult;
    NativeArray<float> _angleChangeDuration;
    NativeArray<bool> _reversingAngle;

    float3 _originPos;
    NativeArray<float3> _projectilePos;
    NativeArray<float3> _directionPos;
    NativeArray<float> _projectDelayTime;
    float[] _angle;
    NativeArray<bool> _setActiveInfo;
    NativeArray<bool> _hitPlayer;
    NativeArray<float> _lifeTime;
    NativeArray<float3> _direction;


    private void Awake()
    {
        float halfAngleRange = angleRange / 2f;
        _minAngle = targetAngle - halfAngleRange;
        _maxAngle = targetAngle + halfAngleRange;
        if (_minAngle > _maxAngle)
        {
            float temp = _minAngle;
            _minAngle = _maxAngle;
            _maxAngle = temp;
        }
        _defaultTargetAngle = targetAngle;
        _defaultAngleRange = angleRange;
        particleContainer = new GameObject("Particle Pool");
        container = new GameObject("Projectile Container");
        container.transform.localPosition = Vector3.zero;

        for (int i = 0; i < data.numberOfShoot; i++)
        {
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                var x = Instantiate(projectile, container.transform);
                _projectiles.Add(x);
                x.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < particlePoolCount; i++)
        {
            var y = Instantiate(particle, particleContainer.transform);
            _particles.Add(y);
            y.gameObject.SetActive(false);
        }
    }

    IEnumerator JobCoroutine()
    {
        while (_isShooting == true)
        {
            if (_totalLifeTime <= 0 && data.isLooping == false) break;
            if (_loopDuration <= 0 && data.isLooping == true) break;
            _loopDuration -= Time.deltaTime;
            _jobProjectile.duration = _loopDuration;
            _totalLifeTime -= Time.deltaTime;
            _jobProjectile.deltaTime = Time.deltaTime;
            _jobProjectile.originPos = (float3)origin.position;
            BulletAngle();
            BulletRotation();
            BulletDirection();
            _jobHandleProjectile = _jobProjectile.Schedule(_projectiles.Count, 64);
            _jobHandleProjectile.Complete();
            BulletPattern();
            if (_loopDuration < data.loopDuration * data.delayBetweenShot)
            {
                endOfJob = true;
            }
            yield return null;
        }
        ResetToDefault();
        JobEnd();
        yield break;
    }

    void ResetToDefault()
    {
        targetAngle = _defaultTargetAngle;
        angleRange = _defaultAngleRange;
    }

    void BulletRotation()
    {
        if (!data.enableAutoRotation)
        {
            return;
        }
        _jobRotation.deltaTime = Time.deltaTime;
        _jobHandleRotation = _jobRotation.Schedule();
        _jobHandleRotation.Complete();
        targetAngle = _rotationResult[0];
    }
    void BulletAngle()
    {
        if (!data.enableAutoAngle)
        {
            return;
        }
        _jobAngleRange.deltaTime = Time.deltaTime;
        _jobHandleAngleRange = _jobAngleRange.Schedule();
        _jobHandleAngleRange.Complete();
        angleRange = _angleRangeResult[0];

    }
    void BulletDirection()
    {

        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (data.numberOfSegments - 1);
        for (int j = 0; j < data.numberOfSegments; j++)
        {
            float angle = _minAngle + j * angleIncrement;
            if (data.numberOfSegments == 1)
            {
                targetAngle = (_minAngle + _maxAngle) / 2f;
                angle = targetAngle;
            }
            if (canAim)
            {
                angle += Mathf.Atan2(playerInfo.position.y - origin.position.y + offset.y, playerInfo.position.x - origin.position.x + offset.x) * Mathf.Rad2Deg;
            }
            float3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            _jobProjectile.direction[j] = direction;

        }
    }
    void BulletPattern()
    {
        for (int i = 0; i < _projectiles.Count; i++)
        {
            if (_setActiveInfo[i] == false) continue;               
            if (_hitPlayer[i] == true) continue;                    
            if (_lifeTime[i] <= 0 || _lifeTime[i] > _loopDuration)                                  
            {                                                       
                _setActiveInfo[i] = false;                          
                _projectiles[i].gameObject.SetActive(false);
                continue;                                           
            }                                                       
            _projectiles[i].gameObject.SetActive(_setActiveInfo[i]);
            _projectiles[i].position = (Vector3)_projectilePos[i];
            _projectiles[i].rotation = Quaternion.Euler(0f, 0f, _angle[i]);

            Collider2D x = Physics2D.OverlapCircle((Vector3)_projectilePos[i], 0.15f);
            if (x != null && (x.CompareTag("Player") || x.gameObject.layer == 7))
            {
                _setActiveInfo[i] = false;
                _hitPlayer[i] = true;
                _projectiles[i].gameObject.SetActive(false);
                ParticlePool(i);
                continue;
            }
        }
    }
    
    void ParticlePool(int index)
    {
        foreach (GameObject obj in _particles)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = (Vector3)(_projectilePos[index]);
                obj.SetActive(true);
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
        if (data.enableAutoRotation)
        {
            _rotationResult.Dispose();
            _rotationDuration.Dispose();
            _reversingRotation.Dispose();
            _rotationResult = default;
            _rotationDuration = default;
            _reversingRotation = default;
            Debug.Log("AutoRotationDisposed");
        }
        if (data.enableAutoAngle)
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
            ) return;

        _isShooting = false;
        _projectilePos.Dispose();
        _directionPos.Dispose();
        _projectDelayTime.Dispose();
        _lifeTime.Dispose();
        _setActiveInfo.Dispose();
        _hitPlayer.Dispose();
        _direction.Dispose();
        _projectilePos = default;
        _directionPos = default;
        _projectDelayTime = default;
        _lifeTime = default;
        _setActiveInfo = default;
        _hitPlayer = default;
        _direction = default;
        Debug.Log("Disposed");


    }
    public void InitializeData()
    {
        float totalAngleRange = _maxAngle - _minAngle;
        float angleIncrement = totalAngleRange / (data.numberOfSegments - 1);

        float delayedTime = 0f;
        float lifeTimespan = data.projectileLifeTime;

        for (int i = 0; i < data.numberOfShoot; i++)
        {
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                float angle = _minAngle + j * angleIncrement;
                if (data.numberOfSegments == 1)
                {
                    targetAngle = (_minAngle + _maxAngle) / 2f;
                    angle = targetAngle;
                }
                _angle[i * data.numberOfSegments + j] = angle;
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                if (i * data.numberOfSegments + j < _projectiles.Count)
                {
                    _projectilePos[i * data.numberOfSegments + j] = origin.position;
                    _directionPos[i * data.numberOfSegments + j] = direction;
                    _projectDelayTime[i * data.numberOfSegments + j] = delayedTime;
                    _lifeTime[i * data.numberOfSegments + j] = lifeTimespan;
                }
            }
            delayedTime += data.delayBetweenShot;
        }
    }

    
    public IEnumerator Shoot()
    {
        endOfJob = false;
        if (_isShooting) yield break;
        for (int i = 0; i < _projectiles.Count; i++)
        {
            _projectiles[i].transform.position = origin.position;
        }
        _projectilePos = new NativeArray<float3>(_projectiles.Count, Allocator.Persistent);
        _directionPos = new NativeArray<float3>(_projectiles.Count, Allocator.Persistent);
        _projectDelayTime = new NativeArray<float>(_projectiles.Count, Allocator.Persistent);
        _lifeTime = new NativeArray<float>(_projectiles.Count, Allocator.Persistent);
        _setActiveInfo = new NativeArray<bool>(_projectiles.Count, Allocator.Persistent);
        _hitPlayer = new NativeArray<bool>(_projectiles.Count, Allocator.Persistent);
        _direction = new NativeArray<float3>(data.numberOfSegments, Allocator.Persistent);
        _angle = new float[_projectiles.Count];
        InitializeData();
        if (data.enableAutoRotation)
        {
            _rotationResult = new NativeArray<float>(1, Allocator.Persistent);
            _rotationDuration = new NativeArray<float>(1, Allocator.Persistent);
            _reversingRotation = new NativeArray<bool>(1, Allocator.Persistent);
            _rotationDuration[0] = data.rotationDuration;
            _rotationResult[0] = targetAngle;
            _jobRotation = new RotationJob()
            {
                targetAngle = targetAngle,
                rotationSpeed = data.rotationSpeed,
                rotationDegree = data.rotationDegree,
                reversingRotation = _reversingRotation,
                rotationDuration = _rotationDuration,
                targetAngleResult = _rotationResult,
                minRotationChange = data.minRotationChange,
                maxRotationChange = data.maxRotationChange,
                canReverseRotation = data.canReverseRotation,
                fixedDeltaTime = 0.02f,

            };
        }
        if (data.enableAutoAngle)
        {
      
            _angleRangeResult = new NativeArray<float>(1, Allocator.Persistent);
            _reversingAngle = new NativeArray<bool>(1, Allocator.Persistent);
            _angleChangeDuration = new NativeArray<float>(1, Allocator.Persistent);
            _angleChangeDuration[0] = data.angleChangeDuration;
            _jobAngleRange = new ChangeAngleRangeJob()
            {
                angleRange = angleRange,
                angleDegree = data.angleDegree,
                angleChangeSpeed = data.angleChangeSpeed,
                angleChangeDuration = _angleChangeDuration,
                angleRangeResult = _angleRangeResult,
                maxAngleChange = data.maxAngleChange,
                minAngleChange = data.minAngleChange,
                canReverseAngle = data.canReverseAngle,
                reversingAngle =_reversingAngle,
                fixedDeltaTime = 0.02f,

            };
        }
        _jobProjectile = new ProjectileJobSingle
        {
            originPos = _originPos,
            direction = _direction,
            hitPlayer = _hitPlayer,
            position = _projectilePos,
            directionPos = _directionPos,
            delayTime = _projectDelayTime,
            setActive = _setActiveInfo,
            speed = data.projectileSpeed,
            duration = data.loopDuration,
            isLooping = data.isLooping,
            defaultDelayTime = data.delayBetweenShot,
            lifeTime = _lifeTime,
            defaultLifeTime = data.projectileLifeTime,
            segment = data.numberOfSegments,
            fixedDeltaTime = 0.02f,
    
        };
        _totalLifeTime = data.numberOfShoot * data.delayBetweenShot + data.projectileLifeTime;
        _loopDuration = data.loopDuration;
        _isShooting = true;
        yield return StartCoroutine(JobCoroutine());
    }


   #if UNITY_EDITOR
       void OnValidate()
       {
           // Calculate the half angle range from the target angle
           float halfAngleRange = angleRange / 2f;
   
           // Adjust the min and max angles based on the target angle and half angle range
           _minAngle = targetAngle - halfAngleRange;
           _maxAngle = targetAngle + halfAngleRange;
   
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
        // Calculate the half angle range from the target angle
        float halfAngleRange = angleRange / 2f;
        // Adjust the min and max angles based on the target angle and half angle range
        _minAngle = targetAngle - halfAngleRange;
        _maxAngle = targetAngle + halfAngleRange;

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
        float angleIncrement = totalAngleRange / (data.numberOfSegments - 1);
        for (int i = 0; i < data.numberOfSegments; i++)
        {
            float angle = _minAngle + i * angleIncrement;
            if (data.numberOfSegments == 1)
            {
                targetAngle = (_minAngle + _maxAngle) / 2f;
                angle = targetAngle;
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

        for (int i = 0; i < _projectiles.Count; i++)
        {
            Gizmos.DrawWireSphere((Vector3)_projectilePos[i], 0.15f);
        }
    }
}

