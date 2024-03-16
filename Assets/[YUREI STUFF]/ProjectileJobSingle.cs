using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct ProjectileJobSingle : IJobParallelFor
{
    [ReadOnly] public float3 originPos;
    public NativeArray<float3> position;
    [ReadOnly] public NativeArray<float3> direction;
    public NativeArray<float> delayTime;
    public NativeArray<float> lifeTime;
    public NativeArray<bool> setActive;
    public NativeArray<bool> hitPlayer;
    public float speed;
    [ReadOnly]public float deltaTime;
    public void Execute(int index)
    {
        if (delayTime[index] >= 0)
        {
            delayTime[index] -= deltaTime;
            setActive[index] = false;
            position[index] = originPos;
            return;
        }
        else if (lifeTime[index] >= 0 && hitPlayer[index] == false)
        {
            setActive[index] = true;
            lifeTime[index] -= deltaTime;
            float3 movement = math.normalize(direction[index]) * speed * deltaTime;
            position[index] += movement;
            return;
        }

        setActive[index] = false;

    }
}