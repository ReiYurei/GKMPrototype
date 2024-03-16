using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using TriInspector;
using Unity.Collections.LowLevel.Unsafe;


#if UNITY_EDITOR
using UnityEditor;

#endif



[BurstCompile(CompileSynchronously =false)]
public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField][InlineEditor]public SO_Projectile_Data data;
    float _minAngle = -90f;
    float _maxAngle = 90f;
    public float targetAngle = 0f;
    public float rotation = 0f;
    public float angleRange = 90f;
    bool _isShooting = false;
    public Transform projectile;
    public Transform container;
    [SerializeField] List<Transform> _projectiles;
    [Space(15)]
    public GameObject particle;
    public Transform particleContainer;
    public int particlePoolCount;

    [SerializeField] List<GameObject> _particles;

    public Transform origin;

    NativeArray<float3> _projectilePos;
    NativeArray<float3> _directionPos;
    NativeArray<float>  _projectDelayTime;
    NativeArray<float>  _lifeTime;
    NativeArray<bool>   _setActiveInfo;
    NativeArray<bool>   _hitPlayer;


    public bool isDisabled;
    public ProjectileBehaviour behaviour;
    public float totalLifeTime;

    private void Awake()
    {
        behaviour.OnJobEnd += DisposeAll;
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
    private void OnDisable()
    {
        _projectiles.Clear();
        _particles.Clear();
        DisposeAll();
        behaviour.OnJobEnd -= DisposeAll;


    }
    private void Update()
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


    
    }
    private void LateUpdate()
    {
       
     //   _jobHandle.Complete();
     //   if (isShooting == false && totalLifeTime > 0)
     //   {
     //       JobUpdate();
     //
     //   }
     //   DisposeAll();
    }
    public void Shoot()
    {
        InitiateJob();
        _isShooting = true;
    }

    public void Dispose()
    {
        //DisposeAll(behaviour._jobHandle);
        Debug.Log(_projectilePos.IsCreated);

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
    public IEnumerator ChangeAngle()
    {
        while (_isShooting == true)
        {
            targetAngle += rotation * Time.fixedDeltaTime;
            yield return null;
        }
        targetAngle = 0;
        yield break;
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
                 if ( i * data.numberOfSegments + j < _projectiles.Count)
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



    public void InitiateJob()
    {
        if (_isShooting) return;
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
        JobAssign();
        behaviour.JobStart();

        isDisabled = false;
    }
    private void JobAssign()
    {
        behaviour.transforms = _projectiles;
        behaviour.ps = _particles;
        behaviour.projectilePos = _projectilePos;
        behaviour.directionPos = _directionPos;
        behaviour.projectDelayTime = _projectDelayTime;
        behaviour.speed = data.projectileSpeed;
        behaviour.lifeTime = _lifeTime;
        behaviour.numOfShot = data.numberOfShoot;
        behaviour.setActiveInfo = _setActiveInfo;
        behaviour.delay = data.delayBetweenShot;
        behaviour.hitPlayer = _hitPlayer;
    }
    public void DisposeAll()
    {
#if UNITY_EDITOR
    //    Debug.Log(_projectilePos.IsCreated);
    //    Debug.Log(_directionPos.IsCreated);
    //    Debug.Log(_projectDelayTime.IsCreated);
    //    Debug.Log(_lifeTime.IsCreated);
    //    Debug.Log(_setActiveInfo.IsCreated);
    //    Debug.Log(_hitPlayer.IsCreated);
#endif

        if (_projectilePos.IsCreated != true || _directionPos.IsCreated != true || _projectDelayTime.IsCreated != true ||
             _lifeTime.IsCreated != true || _setActiveInfo.IsCreated != true || _hitPlayer.IsCreated != true) return;

        _isShooting = false;
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
