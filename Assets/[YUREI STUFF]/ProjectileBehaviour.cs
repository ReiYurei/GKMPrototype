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
    public List<GameObject> ps;

    public Transform origin;
    float3 originPos;
    public NativeArray<float3> projectilePos;
    public NativeArray<float3> directionPos;
    public NativeArray<float>  projectDelayTime;
    public NativeArray<bool>   setActiveInfo;
    public NativeArray<bool>   hitPlayer;
    public NativeArray<float>  lifeTime;

    public delegate void OnJobEndHandler(); 
    public event OnJobEndHandler OnJobEnd;

    public float speed;
    public float totalLifeTime;
    public int numOfShot;
    public int chunkSize;
    public float delay;
    public bool isDisabled;

    ProjectileJobSingle job;
    public JobHandle jobHandle;

    private void LateUpdate()
    {
       //  if (isDisabled == false)
       //  {
       //     if (totalLifeTime > 0)
       //     {
       //         job.deltaTime = Time.deltaTime;
       //         totalLifeTime -=Time.deltaTime;
       //         job.originPos = (float3)origin.position;
       //         jobHandle = job.Schedule(transforms.Count, 64);
       //         jobHandle.Complete();
       //         for (int i = 0; i < transforms.Count; i++)
       //         {
       //             transforms[i].position = (Vector3)projectilePos[i];
       //             if (_lifeTime[i] <= 0)
       //             {
       //                 transforms[i].gameObject.SetActive(false);
       //             }
       //
       //             if (_setActiveInfo[i] == true && _hitPlayer[i] == false)
       //             {
       //                // Collider2D x = Physics2D.OverlapCircle((Vector3)projectilePos[i], 0.15f);
       //               //  if (x != null && (x.tag == "Player" || x.gameObject.layer == 7))
       //               //  {
       //               //      _setActiveInfo[i] = false;
       //               //      _hitPlayer[i] = true;
       //               //      transforms[i].gameObject.SetActive(false);
       //               //      ParticlePool(i);
       //               //      continue;
       //               //      //launch particle at te position
       //               //  }
       //             }
       //         }
       //         return;
       //     }
       //     JobEnd();
       // }
    }
   
    public void Dispose()
    {
        Debug.Log(jobHandle.IsCompleted) ;

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
        StartCoroutine(JobCoroutine());
    }
    void JobEnd()
    {
        isDisabled = true;

        OnJobEnd?.Invoke();

        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].gameObject.SetActive(false);
        }

    }
    IEnumerator JobCoroutine()
    {
        while (isDisabled == false)
        {
            if (totalLifeTime <= 0) break;
            job.deltaTime = Time.deltaTime;
            totalLifeTime -= Time.deltaTime;
            job.originPos = (float3)origin.position;
            jobHandle = job.Schedule(transforms.Count, 64);
            jobHandle.Complete();
            for (int i = 0; i < transforms.Count; i++)
            {
                if (setActiveInfo[i] == false) continue;
                if (hitPlayer[i] == true) continue;
                if (lifeTime[i] <= 0)
                {
                    setActiveInfo[i] = false;
                    transforms[i].gameObject.SetActive(false);
                    continue;
                }
                transforms[i].gameObject.SetActive(setActiveInfo[i]);
                transforms[i].position = (Vector3)projectilePos[i];
                Collider2D x = Physics2D.OverlapCircle((Vector3)projectilePos[i], 0.15f);
                if (x != null && (x.CompareTag("Player") || x.gameObject.layer == 7))
                {
                   setActiveInfo[i] = false;
                   hitPlayer[i] = true;
                   transforms[i].gameObject.SetActive(false);
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
        foreach (GameObject obj in ps)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = (Vector3)(projectilePos[index]);
                obj.SetActive(true);
                return;
            }
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


