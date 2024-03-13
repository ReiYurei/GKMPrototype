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
    public Transform origin;
    float3 originPos;
    public NativeArray<float3> projectilePos;
    public NativeArray<float3> directionPos;
    public NativeArray<float> projectDelayTime;
    public NativeArray<bool> setActiveInfo;
    public NativeArray<bool> hitPlayer;
    public NativeArray<float> lifeTime;
    public float speed;

    public delegate void OnJobEndHandler(); 
    public event OnJobEndHandler OnJobEnd;

    public float totalLifeTime;
    public int numOfShot;
    public int chunkSize;
    public float delay;
    public bool isDisabled;

    ProjectileJobSingle job;

    private void LateUpdate()
    {
        if (isDisabled == false)
        {
            job.originPos = (float3)origin.position;
            if (totalLifeTime > 0)
            {
                totalLifeTime -=Time.deltaTime;
                
                job.deltaTime = Time.deltaTime;
                JobHandle jobHandle = job.Schedule(transforms.Count, 1000);
                jobHandle.Complete();
                for (int i = 0; i < transforms.Count; i++)
                {
                    transforms[i].position = (Vector3)projectilePos[i];
                    transforms[i].gameObject.SetActive((bool)setActiveInfo[i]);
                    if (setActiveInfo[i] == true)
                    {
                        var x = Physics2D.OverlapCircle((Vector3)projectilePos[i], 0.15f);
                        if (x != null && x.tag == "Player")
                        {
                            hitPlayer[i] = true;
                            Debug.Log(x.gameObject.name);

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
        job = new ProjectileJobSingle()
        {
            originPos = originPos,
            hitPlayer = hitPlayer,
            position = projectilePos,
            direction = directionPos,
            delayTime = projectDelayTime,
            setActive = setActiveInfo,
            speed = speed,
            lifeTime = lifeTime,
        };
        isDisabled = false;

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
    private void OnDisable()
    {
        isDisabled = true;
    }
    public IEnumerator Launch(float speed, float lifetime, Vector3 direction)
    {

        float elapsedTime = 0;

        while (elapsedTime < lifetime)
        {
            Vector3 movement = direction.normalized * speed * Time.fixedDeltaTime;

            transform.position += movement;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        yield break;
    } //deprecated
}



[BurstCompile(CompileSynchronously = false)]
public struct ProjectileJobSingle : IJobParallelFor
{
    public float3 originPos;
    public NativeArray<float3> position;
    public NativeArray<float3> direction;
    public NativeArray<float> delayTime;
    public NativeArray<float> lifeTime;
    public NativeArray<bool> setActive;
    public NativeArray<bool> hitPlayer;

    public float speed;
    public float deltaTime;

    public void Execute(int index)
    {
        if (delayTime[index] > 0)
        {
            delayTime[index] -= deltaTime;
            setActive[index] = false;
            position[index] = originPos;
            return;
        }
        else if (lifeTime[index] > 0 && hitPlayer[index] == false)
        {
            setActive[index] = true;
            lifeTime[index] -= deltaTime;
            float3 movement = math.normalize(direction[index]) * speed * deltaTime;
            position[index] +=  movement;
            return;
        }

        setActive[index] = false;
        
        
        

    }
}