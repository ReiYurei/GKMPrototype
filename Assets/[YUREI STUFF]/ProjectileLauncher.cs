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
    float minAngle = -90f;
    float maxAngle = 90f;
    public float targetAngle = 0f;
    public float rotation = 0f;
    public float angleRange = 90f;
    bool isShooting = false;
    public Transform projectile;
    public Transform container;
    [SerializeField] List<Transform> projectiles;
    [Space(15)]
   // public GameObject particle;
   // public Transform particleContainer;
   // [SerializeField] List<GameObject> particles;

    public Transform origin;

    NativeArray<float3> projectilePos;
    NativeArray<float3> directionPos;
    NativeArray<float>  projectDelayTime;
    NativeArray<float>  lifeTime;
    NativeArray<bool>   setActiveInfo;
    NativeArray<bool>   hitPlayer;

    public bool isDisabled;
    public ProjectileBehaviour behaviour;
    public float totalLifeTime;

    private void Awake()
    {
        behaviour.OnJobEnd += DisposeAll;
        container.transform.localPosition = Vector3.zero;
        for (int i = 0; i < data.numberOfShoot; i++)
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                var x = Instantiate(projectile, container.transform);
               // var y = Instantiate(particle, particleContainer.transform);

                projectiles.Add(x);
             //   particles.Add(y);
                x.gameObject.SetActive(false);
            //    y.gameObject.SetActive(false);
            }
    }
    private void OnDisable()
    {
        projectiles.Clear();
        behaviour.OnJobEnd -= DisposeAll;


    }
    private void Update()
    {
        float halfAngleRange = angleRange / 2f;

        minAngle = targetAngle - halfAngleRange;
        maxAngle = targetAngle + halfAngleRange;

        if (minAngle > maxAngle)
        {
            float temp = minAngle;
            minAngle = maxAngle;
            maxAngle = temp;
        }


    
    }
    private void LateUpdate()
    {
       
     //   jobHandle.Complete();
     //   if (isDisabled == false && totalLifeTime > 0)
     //   {
     //       JobUpdate();
     //
     //   }
     //   DisposeAll();
    }
    public void Shoot()
    {
        // InitializeData();
        //
        // for (int i = 0; i < data.numberOfShoot; i++)
        // {
        //     for (int j = 0; j < data.numberOfSegments; j++)
        //     {
        //
        //         if (i * data.numberOfSegments + j < projectiles.Count)
        //         {
        //             var projectileArray = projectiles[i * data.numberOfSegments + j];
        //             projectileArray.gameObject.SetActive(true);
        //         }
        //     }
        // }
        // isDisabled = false;
        // TotalLifeTime();
       // DisposeAll(behaviour.jobHandle);
        InitiateJob();
        isShooting = true;
    }

    public void Dispose()
    {
        DisposeAll(behaviour.jobHandle);


    }
#if UNITY_EDITOR
    void OnValidate()
    {
        // Calculate the half angle range from the target angle
        float halfAngleRange = angleRange / 2f;

        // Adjust the min and max angles based on the target angle and half angle range
        minAngle = targetAngle - halfAngleRange;
        maxAngle = targetAngle + halfAngleRange;

        // Calculate the angle increment for each segment


        // Ensure minAngle is less than maxAngle
        if (minAngle > maxAngle)
        {
            float temp = minAngle;
            minAngle = maxAngle;
            maxAngle = temp;
        }
    }



    void OnDrawGizmos()
    {
        DivideAngle();
    }
#endif
    public IEnumerator ChangeAngle()
    {
        while (isShooting == true)
        {
            targetAngle += rotation * Time.fixedDeltaTime;
            yield return null;
        }
        targetAngle = 0;
        yield break;
    }
    public void InitializeData()
    {
        float totalAngleRange = maxAngle - minAngle;
        float angleIncrement = totalAngleRange / (data.numberOfSegments - 1);
 
        float delayedTime = 0f;
        float lifeTimespan = data.projectileLifeTime;

        for (int i = 0; i < data.numberOfShoot; i++)
        {
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                 float angle = minAngle + j * angleIncrement;
                 if (data.numberOfSegments == 1)
                 {
                     targetAngle = (minAngle + maxAngle) / 2f;
                     angle = targetAngle;
                 }
                 Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                 if ( i * data.numberOfSegments + j < projectiles.Count)
                 {
                    projectilePos[i * data.numberOfSegments + j] = origin.position;
                    directionPos[i * data.numberOfSegments + j] = direction;
                    projectDelayTime[i * data.numberOfSegments + j] = delayedTime;
                    lifeTime[i * data.numberOfSegments + j] = lifeTimespan;
                 }
            }
            delayedTime += data.delayBetweenShot;
        }
    }



    public void InitiateJob()
    {
        if (isShooting) return;
        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].transform.position = origin.position;
        }
        projectilePos = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
        directionPos = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
        projectDelayTime = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
        lifeTime = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
        setActiveInfo = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);
        hitPlayer = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);
        InitializeData();
        JobAssign();
        behaviour.JobStart();
        for (int i = 0; i < data.numberOfShoot; i++)
        {
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                
                if (i * data.numberOfSegments + j < projectiles.Count)
                {
                    var projectileArray = projectiles[i * data.numberOfSegments + j];
                    projectileArray.gameObject.SetActive(true);
                }
            }
        }
        isDisabled = false;
    }
    private void JobAssign()
    {
        behaviour.transforms = projectiles;
       // behaviour.particle = particles;
        behaviour.projectilePos = projectilePos;
        behaviour.directionPos = directionPos;
        behaviour.projectDelayTime = projectDelayTime;
        behaviour.speed = data.projectileSpeed;
        behaviour.lifeTime = lifeTime;
        behaviour.numOfShot = data.numberOfShoot;
        behaviour.setActiveInfo = setActiveInfo;
        behaviour.delay = data.delayBetweenShot;
        behaviour.hitPlayer = hitPlayer;
    }
    public void DisposeAll(JobHandle handle)
    {
        if (projectilePos.IsCreated == false || directionPos.IsCreated == false || projectDelayTime.IsCreated == false ||
             lifeTime.IsCreated == false || setActiveInfo.IsCreated == false || hitPlayer.IsCreated == false) return;
        unsafe
        {
            void* projectilePtr = projectilePos.GetUnsafePtr();
            void* directionPtr = directionPos.GetUnsafePtr();
            void* projectDelayPtr = projectDelayTime.GetUnsafePtr();
            void* lifePtr = lifeTime.GetUnsafePtr();
            void* setActiveInfoPtr = setActiveInfo.GetUnsafePtr();
            void* hitPtr = hitPlayer.GetUnsafePtr();
            UnsafeUtility.Free(projectilePtr, Allocator.Persistent);
            UnsafeUtility.Free(directionPtr, Allocator.Persistent);
            UnsafeUtility.Free(projectDelayPtr, Allocator.Persistent);
            UnsafeUtility.Free(lifePtr, Allocator.Persistent);
            UnsafeUtility.Free(setActiveInfoPtr, Allocator.Persistent);
            UnsafeUtility.Free(hitPtr, Allocator.Persistent);
            Debug.Log("Freed Memory");
      
        }

        isShooting = false;
     // projectilePos.Dispose();
     // directionPos.Dispose();
     // projectDelayTime.Dispose();
     // lifeTime.Dispose();
     // setActiveInfo.Dispose();
     // hitPlayer.Dispose();
        
        Debug.Log("Disposed");
    }
    void DivideAngle()
    {
           float totalAngleRange = maxAngle - minAngle;
           float angleIncrement = totalAngleRange / (data.numberOfSegments - 1);
           for (int i = 0; i < data.numberOfSegments; i++)
           {
               float angle = minAngle + i * angleIncrement;
               if (data.numberOfSegments == 1)
               {
                   targetAngle = (minAngle + maxAngle) / 2f;
                   angle = targetAngle;
               }
               Vector3 start = Quaternion.Euler(0, 0, angle) * Vector3.right * 2f;
               Vector3 end = Quaternion.Euler(0, 0, angle) * Vector3.right * 3f; 

               Gizmos.color = Color.red;
               Gizmos.DrawLine(transform.position + start, transform.position + end);
           }
    }
}
