using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using TriInspector;
using Unity.Burst;
using System.Collections;
#if UNITY_EDITOR
#endif
public class ProjectileEngine : MonoBehaviour
{
    [SerializeField][InlineEditor] public SO_Projectile_Data data;
    float _minAngle = -90f;
    float _maxAngle = 90f;
    public float targetAngle = 0f;
    public float rotation = 0f;
    public float angleRange = 90f;
    public float totalLifeTime;
    public bool isShooting;
    public Transform projectile;
    public Transform container;
    [SerializeField] List<Transform> _projectiles;
    [Space(15)]

    public Transform origin;

    public GameObject particle;
    public Transform particleContainer;
    public int particlePoolCount;
    [SerializeField] List<GameObject> _particles;


    ProjectileJobSingle _job;
    JobHandle _jobHandle;
    float3 _originPos;
    NativeArray<float3> _projectilePos;
    NativeArray<float3> _directionPos;
    NativeArray<float> _projectDelayTime;
    NativeArray<bool> _setActiveInfo;
    NativeArray<bool> _hitPlayer;
    NativeArray<float> _lifeTime;

    private void Awake()
    {
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
        while (isShooting == true)
        {
            if (totalLifeTime <= 0) break;
            _job.deltaTime = Time.deltaTime;
            totalLifeTime -= Time.deltaTime;
            _job.originPos = (float3)origin.position;
            _jobHandle = _job.Schedule(_projectiles.Count, 64);
            _jobHandle.Complete();
            for (int i = 0; i < _projectiles.Count; i++)
            {
                if (_setActiveInfo[i] == false) continue;
                if (_hitPlayer[i] == true) continue;
                if (_lifeTime[i] <= 0)
                {
                    _setActiveInfo[i] = false;
                    _projectiles[i].gameObject.SetActive(false);
                    continue;
                }
                _projectiles[i].gameObject.SetActive(_setActiveInfo[i]);
                _projectiles[i].position = (Vector3)_projectilePos[i];
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
            yield return null;
        }
        JobEnd();
        yield break;
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
        isShooting = false;
        DisposeAll();
        for (int i = 0; i < _projectiles.Count; i++)
        {
            _projectiles[i].gameObject.SetActive(false);
        }
    }
    public void DisposeAll()
    {

        if (_projectilePos.IsCreated != true || _directionPos.IsCreated != true || _projectDelayTime.IsCreated != true ||
             _lifeTime.IsCreated != true || _setActiveInfo.IsCreated != true || _hitPlayer.IsCreated != true) return;

        isShooting = false;
        _projectilePos.Dispose();
        _directionPos.Dispose();
        _projectDelayTime.Dispose();
        _lifeTime.Dispose();
        _setActiveInfo.Dispose();
        _hitPlayer.Dispose();
        _projectilePos = default;
        _directionPos = default;
        _projectDelayTime = default;
        _lifeTime = default;
        _setActiveInfo = default;
        _hitPlayer = default;
#if UNITY_EDITOR
        Debug.Log("Disposed");

#endif
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

    public void Shoot()
    {
        if (isShooting) return;
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
        InitializeData();

        _job = new ProjectileJobSingle
        {
            originPos = _originPos,
            hitPlayer = _hitPlayer,
            position = _projectilePos,
            direction = _directionPos,
            delayTime = _projectDelayTime,
            setActive = _setActiveInfo,
            speed = data.projectileSpeed,
            lifeTime = _lifeTime,
            deltaTime = Time.deltaTime
        };
        totalLifeTime = data.numberOfShoot * data.delayBetweenShot + data.projectileLifeTime;
        isShooting = true;
        StartCoroutine(JobCoroutine());
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


    void OnDrawGizmos()
    {
        DivideAngle();
    }
#endif
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
    }
}
