using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
[BurstCompile]
public struct RotationJob : IJob
{
    public NativeArray<float> targetAngleResult;
    public float targetAngle;
    [ReadOnly]public float rotationSpeed;
    [ReadOnly]public float rotationDegree;
    public NativeArray<float> rotationDuration;
    public NativeArray<bool> reversingRotation;
    [ReadOnly] public float maxRotationChange;
    [ReadOnly] public float minRotationChange;
    [ReadOnly]public bool canReverseRotation;
    [ReadOnly]public float deltaTime;
    [ReadOnly] public float fixedDeltaTime;
    
    public void Execute()
    {
        if (rotationDuration[0] <= 0)
        {
            return;
        }
        rotationDuration[0] -= deltaTime;
        if (canReverseRotation)
        {
            if (targetAngleResult[0] >= maxRotationChange)
            {
                reversingRotation[0] = true;
            }
            if (targetAngleResult[0] <= minRotationChange)
            {
                reversingRotation[0] = false;
            }
            if (reversingRotation[0])
            {

                targetAngleResult[0] -= rotationSpeed * rotationDegree * deltaTime * 0.55f;
                return;
            }
            else
            {
                targetAngleResult[0] += rotationSpeed * rotationDegree * deltaTime * 0.55f;
                return;
            }

        }
        if (targetAngleResult[0] > 360f)
        {
            targetAngleResult[0] = 0;
        }
        targetAngleResult[0] += rotationSpeed * rotationDegree * deltaTime * 0.55f;
    }
}
[BurstCompile]
public struct ChangeAngleRangeJob : IJob
{
    public NativeArray<float> angleRangeResult;
    public float angleRange;
    [ReadOnly] public float angleChangeSpeed;
    [ReadOnly] public float angleDegree;
    [ReadOnly] public float maxAngleChange;
    [ReadOnly] public float minAngleChange;
    [ReadOnly] public bool canReverseAngle;
    public NativeArray<float> angleChangeDuration;
    public NativeArray<bool> reversingAngle;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float fixedDeltaTime;
    public void Execute()
    {
        if (angleChangeDuration[0] <= 0)
        {
            return;
        }
        angleChangeDuration[0] -= deltaTime;
        if (canReverseAngle)
        {
            if (angleRangeResult[0] >= maxAngleChange)
            {
                reversingAngle[0] = true;
            }
            if (angleRangeResult[0] <= minAngleChange)
            {
                reversingAngle[0] = false;
            }
            if (reversingAngle[0]) 
            {
                angleRangeResult[0] -= angleChangeSpeed * angleDegree * deltaTime * 0.55f;
                return;
            }
            else
            {
                angleRangeResult[0] += angleChangeSpeed * angleDegree * deltaTime * 0.55f;
                return;
            }

        }    
       if (angleRangeResult[0] > maxAngleChange)
       {
           angleRangeResult[0] = 0;
       }
       angleRangeResult[0] += angleChangeSpeed * angleDegree * deltaTime * 0.55f;
    }
}
[BurstCompile]
public struct ProjectileJobSingle : IJobParallelFor
{
    [ReadOnly] public float3 originPos;
    [ReadOnly] public NativeArray <float3> direction;
    public NativeArray<float3> position;
    public NativeArray<float3> directionPos;
    public NativeArray<float> delayTime;
    public NativeArray<float> lifeTime;
    public NativeArray<bool> setActive;
    public NativeArray<bool> hitPlayer;

    public float duration;
    public float defaultDelayTime;
    public float defaultLifeTime;
    public bool isLooping;
    public float speed;
    public int segment;
    [ReadOnly]public float deltaTime;
    [ReadOnly]public float fixedDeltaTime;

    public void Execute(int index)
    {
        if (isLooping)
        {
            if (lifeTime[index] >= duration)
            {
                setActive[index] = false;
                return;
            }
            if (delayTime[index] > 0)
            {
                delayTime[index] -= deltaTime;
                setActive[index] = false;
                position[index] = originPos;
                directionPos[index] = direction[index % segment];
                return;
            }
      
            if (lifeTime[index] >= 0 && hitPlayer[index] == false && lifeTime[index] < duration)
            {
                setActive[index] = true;
                lifeTime[index] -= deltaTime;
                float3 movement = math.normalize(directionPos[index]) * speed * deltaTime * 0.4999f;
                position[index] += movement;
                return;
            }
            if (lifeTime[index] >= 0)
            {
                lifeTime[index] -= deltaTime;
                return;
            }
          
            position[index] = originPos;
            delayTime[index] = 0;
            lifeTime[index] = defaultLifeTime;
            directionPos[index] = direction[index % segment];
            hitPlayer[index] = false;
            setActive[index] = true;
            return;
        }
        if (delayTime[index] > 0)
        {
            delayTime[index] -= deltaTime;
            setActive[index] = false;
            position[index] = originPos;
            directionPos[index] = direction[index % segment];
            return;
        }
        else if (lifeTime[index] >= 0 && hitPlayer[index] == false)
        {
            setActive[index] = true;
            lifeTime[index] -= deltaTime;
            float3 movement = math.normalize(directionPos[index]) * speed * deltaTime * 0.4999f;
            position[index] += movement;
            return;
        }

        setActive[index] = false;

    }
}