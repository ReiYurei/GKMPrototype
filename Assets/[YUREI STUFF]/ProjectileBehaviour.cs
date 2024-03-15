using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;


[BurstCompile(CompileSynchronously = false)]

public class ProjectileBehaviour : MonoBehaviour
{
    public List<Transform> transforms;
    public ParticleSystem ps;

    public Transform origin;
    float3 originPos;
    public NativeArray<float3> projectilePos;
    public NativeArray<float3> directionPos;
    public NativeArray<float> projectDelayTime;
    public NativeArray<bool> setActiveInfo;
    public NativeArray<bool> hitPlayer;
    public NativeArray<float> lifeTime;

    public delegate void OnJobEndHandler(JobHandle handle); 
    public event OnJobEndHandler OnJobEnd;

    public float speed;
    public float totalLifeTime;
    public int numOfShot;
    public int chunkSize;
    public float delay;
    public bool isDisabled;

    ProjectileJobSingle job;
    public JobHandle jobHandle;

    private void Update()
    {
         if (isDisabled == false)
         {
            if (totalLifeTime > 0)
            {
                job.deltaTime = Time.deltaTime;
                totalLifeTime -=Time.deltaTime;
                job.originPos = (float3)origin.position;
                jobHandle = job.Schedule(transforms.Count, 64);
                jobHandle.Complete();
                for (int i = 0; i < transforms.Count; i++)
                {
                    transforms[i].position = (Vector3)projectilePos[i];
                    if (lifeTime[i] <= 0)
                    {
                        transforms[i].gameObject.SetActive(false);
                    }

                    if (setActiveInfo[i] == true)
                    {
                        var x = Physics2D.OverlapCircle((Vector3)projectilePos[i], 0.15f);
                        if (x != null && (x.tag == "Player" || x.gameObject.layer == 7))
                        {
                            transforms[i].gameObject.SetActive(false);
                            hitPlayer[i] = true;     
                            
                            //launch particle at te position
                        }
                    }
                }
                return;

            }
            JobEnd();
        }
    }
 
    private void OnDrawGizmos()
    {
        if (isDisabled)
        {
            return;
        }
        UnityEngine.Color color = UnityEngine.Color.blue;
        Gizmos.color = color;
        
        for (int i = 0; i < transforms.Count; i++)
        {
            Gizmos.DrawWireSphere((Vector3)projectilePos[i], 0.15f);
        }
    }
    public void JobStart()
    {
        totalLifeTime = numOfShot * delay + lifeTime[lifeTime.Length -1];
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
        isDisabled = false;
    }
  
    void JobEnd()
    {
        isDisabled = true;

        OnJobEnd?.Invoke(jobHandle);        

        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].gameObject.SetActive(false);
        }

    }
    private void OnEnable()
    {
        isDisabled = true;
    }
    private void OnDisable()
    {
        isDisabled = true;
        transforms.Clear(); 
    }

}


