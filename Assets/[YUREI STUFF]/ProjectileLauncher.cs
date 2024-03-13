using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using System.ComponentModel.Design;
using Unity.Jobs;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
using TriInspector;

#endif



[BurstCompile(CompileSynchronously =false)]
public class ProjectileLauncher : MonoBehaviour
{

    float minAngle = -90f;
    float maxAngle = 90f;
    [Range(1, 90)] public int numberOfSegments;
    public float projectileSpeed;
    public int numberOfShoot;
    public float delayBetweenShot;
    public float projectileLifeTime;
    public float targetAngle = 0f;
    public float rotation = 0f;
    public float angleRange = 90f;
    bool isShooting = false;
    public Transform container;
    public Transform projectile;
    [SerializeField] List<Transform> projectiles;

    NativeArray<float3> positionArray;
    NativeArray<float3> directionArray;
    NativeArray<float>  projectileDelayedTime;
    NativeArray<float>  lifeTimeArray;
    NativeArray<bool>   setActiveArray;
    NativeArray<bool>   hitPlayer;

    public ProjectileBehaviour behaviour;

 
    private void Start()
    {
        behaviour.OnJobEnd += DisposeAll;
        container.transform.localPosition = Vector3.zero;
        for (int i = 0; i < numberOfShoot; i++)
            for (int j = 0; j < numberOfSegments; j++)
            {
                var x = Instantiate(projectile, container.transform);
                projectiles.Add(x);
                x.gameObject.SetActive(false);
            }
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

    public void ShootNon()
    {
        StopAllCoroutines();
        InitiateJob();
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
        float angleIncrement = totalAngleRange / (numberOfSegments - 1);

        positionArray = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
        directionArray = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
        projectileDelayedTime = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
        lifeTimeArray = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
        setActiveArray = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);
        hitPlayer = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);

        float delayedTime = delayBetweenShot;
        float lifeTimespan = projectileLifeTime;

        for (int i = 0; i < numberOfShoot; i++)
        {
            for (int j = 0; j < numberOfSegments; j++)
            {
                 float angle = minAngle + j * angleIncrement;
                 if (numberOfSegments == 1)
                 {
                     targetAngle = (minAngle + maxAngle) / 2f;
                     angle = targetAngle;
                 }
                 Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                 if ( i * numberOfSegments+ j < projectiles.Count)
                 {
                     var projectileArray = projectiles[j];
                     positionArray[i * numberOfSegments + j] = projectileArray.transform.position;
                     directionArray[i * numberOfSegments + j] = direction;
                     projectileDelayedTime[i * numberOfSegments + j] = delayedTime;
                     lifeTimeArray[i * numberOfSegments + j] = lifeTimespan;
                 }
            }
            delayedTime += delayBetweenShot;
        }         
    }

    public IEnumerator ShootProjectile()
    {
        float totalAngleRange = maxAngle - minAngle;
        float angleIncrement = totalAngleRange / (numberOfSegments - 1);

        for (int i = 0; i < numberOfShoot; i++)
        {
            for (int j = 0; j < numberOfSegments; j++)
            {
                float angle = minAngle + j * angleIncrement;
                if (numberOfSegments == 1)
                {
                    targetAngle = (minAngle + maxAngle) / 2f;
                    angle = targetAngle;
                }
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                if (i * numberOfSegments + j < projectiles.Count)
                {
                    var projectileArray = projectiles[i * numberOfSegments + j];
                    projectiles[i * numberOfSegments + j].gameObject.transform.position = container.transform.position;

                    projectiles[i * numberOfSegments + j].TryGetComponent<ProjectileBehaviour>(out ProjectileBehaviour component);
                    if (component != null)
                    {
                        projectiles[i * numberOfSegments + j].gameObject.transform.position = container.transform.position;
                        projectiles[i * numberOfSegments + j].gameObject.SetActive(true);

                        StartCoroutine(component.Launch(projectileSpeed / 100, projectileLifeTime, direction));
                    }
                }
            }
            yield return new WaitForSeconds(delayBetweenShot);        
        }
        isShooting = false;
        yield break;

    } //deprecated


    public void InitiateJob()
    {

        for (int i = 0; i < projectiles.Count; i++)
        {
            projectiles[i].transform.position = container.position;
        }
        InitializeData();
        behaviour.transforms = projectiles;
        behaviour.projectilePos = positionArray;
        behaviour.directionPos = directionArray;
        behaviour.projectDelayTime = projectileDelayedTime;
        behaviour.speed = projectileSpeed;
        behaviour.lifeTime = lifeTimeArray;
        behaviour.numOfShot = numberOfShoot;
        behaviour.setActiveInfo = setActiveArray;
        behaviour.delay = delayBetweenShot;
        behaviour.hitPlayer = hitPlayer;
        for (int i = 0; i < numberOfShoot; i++)
        {
            for (int j = 0; j < numberOfSegments; j++)
            {
                
                if (i * numberOfSegments + j < projectiles.Count)
                {
                    var projectileArray = projectiles[i * numberOfSegments + j];
                    projectileArray.gameObject.SetActive(true);

                }
            }
        }
        behaviour.JobStart();    
        isShooting = false;
    }
    void DisposeAll()
    {
       positionArray.Dispose();
       directionArray.Dispose();
       projectileDelayedTime.Dispose();
       lifeTimeArray.Dispose();
       setActiveArray.Dispose();
    }
    void DivideAngle()
       {

           float totalAngleRange = maxAngle - minAngle;
           float angleIncrement = totalAngleRange / (numberOfSegments - 1);
           for (int i = 0; i < numberOfSegments; i++)
           {
               float angle = minAngle + i * angleIncrement;
               if (numberOfSegments == 1)
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

      




