using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using TriInspector;
using Unity.Burst;
#if UNITY_EDITOR
#endif
public class ProjectileLauncherTest : MonoBehaviour
{
    [SerializeField][InlineEditor] public SO_Projectile_Data data;
    float minAngle = -90f;
    float maxAngle = 90f;
    public float targetAngle = 0f;
    public float rotation = 0f;
    public float angleRange = 90f;
    public Transform projectile;
    public Transform container;
    [SerializeField] List<Transform> projectiles;
    [Space(15)]
    public GameObject particle;
    public Transform particleContainer;
    [SerializeField] List<GameObject> particles;

    public Transform origin;

    float3 originPos;
    public NativeArray<float3> projectilePos;
    public NativeArray<float3> directionPos;
    public NativeArray<float> projectDelayTime;
    public NativeArray<bool> setActiveInfo;
    public NativeArray<bool> hitPlayer;
    public NativeArray<float> lifeTime;

    public delegate void OnJobEndHandler();
    public event OnJobEndHandler OnJobEnd;

    public float speed;
    public float totalLifeTime;

    ProjectileJobSingle job;

    private void Start()
    {
        container.transform.localPosition = Vector3.zero;
        for (int i = 0; i < data.numberOfShoot; i++)
            for (int j = 0; j < data.numberOfSegments; j++)
            {
                var x = Instantiate(projectile, container.transform);
                var y = Instantiate(particle, particleContainer.transform);

                projectiles.Add(x);
                particles.Add(y);
                x.gameObject.SetActive(false);
                y.gameObject.SetActive(false);
            }
        totalLifeTime -= Time.deltaTime;
     //   projectilePos = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
     //   directionPos = new NativeArray<float3>(projectiles.Count, Allocator.Persistent);
     //   _projectDelayTime = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
     //   _lifeTime = new NativeArray<float>(projectiles.Count, Allocator.Persistent);
     //   _setActiveInfo = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);
     //   _hitPlayer = new NativeArray<bool>(projectiles.Count, Allocator.Persistent);
    }
    JobHandle jobHandle;
    private void Update()
    {
            if (totalLifeTime > 0)
            {
             
          
            jobHandle = job.Schedule(projectiles.Count, 128);
            jobHandle.Complete();
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].position = (Vector3)projectilePos[i];

                if (setActiveInfo[i] == true)
                {
                    var x = Physics2D.OverlapCircle((Vector3)projectilePos[i], 0.15f);
                    if (x != null && (x.tag == "Player" || x.gameObject.layer == 7))
                    {
                        projectiles[i].gameObject.SetActive(false);
                        hitPlayer[i] = true;
                        particles[i].transform.position = (Vector3)projectilePos[i];
                        particles[i].gameObject.SetActive(true);
                    }
                }

            }

        }

    }
    private void LateUpdate()
    {
        
    }
    public void DisposeAll()
    {
        projectilePos.Dispose();
        directionPos.Dispose();
        projectDelayTime.Dispose();
        lifeTime.Dispose();
        setActiveInfo.Dispose();
        hitPlayer.Dispose();
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
                if (i * data.numberOfSegments + j < projectiles.Count)
                {
                    projectilePos[i * data.numberOfSegments + j] = projectiles[i * data.numberOfSegments + j].transform.position;
                    directionPos[i * data.numberOfSegments + j] = direction;
                    projectDelayTime[i * data.numberOfSegments + j] = delayedTime;
                    lifeTime[i * data.numberOfSegments + j] = lifeTimespan;
                }
            }
            delayedTime += data.delayBetweenShot;
        }


        //JobAssign();
    }

    public void Shoot()
    {
        InitializeData();

        ActivateProjectiles();

        job = new ProjectileJobSingle
        {
            originPos = originPos,
            hitPlayer = hitPlayer,
            position = projectilePos,
            direction = directionPos,
            delayTime = projectDelayTime,
            setActive = setActiveInfo,
            speed = speed,
            lifeTime = lifeTime,
            deltaTime = Time.deltaTime
        };
        totalLifeTime = data.numberOfShoot * data.delayBetweenShot + data.delayBetweenShot;

    }

    private void ActivateProjectiles()
    {
        for (int i = 0; i < data.numberOfShoot; i++)
        {
            for (int j = 0; j < data.numberOfSegments; j++)
            {

                if (i * data.numberOfSegments + j < projectiles.Count)
                {
                    var projectileArray = projectiles[i * data.numberOfSegments + j];
                    projectileArray.position = origin.position;
                    projectileArray.gameObject.SetActive(true);
                }
            }
        }
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
