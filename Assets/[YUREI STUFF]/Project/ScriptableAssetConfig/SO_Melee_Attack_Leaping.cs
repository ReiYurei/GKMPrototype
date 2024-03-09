using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Leaping", menuName = "Enemy/Moveset/Melee/Leaping")]
public class SO_Melee_Attack_Leaping : SO_Base_Attack_Fixed
{
    public float height;
    float time;

    public float maxJumpLength;
    float targetXposition;
    float targetYposition;
    float distance;
    float initialVelocity;
    public override IEnumerator Execute(Enemy enemy)
    {
        target = playerInfo.position;
        targetXposition = target.x;
        targetYposition = target.y;
        yield break;
        
    }

    public IEnumerator ExecuteChain(Enemy enemy)
    {
        time = enemy._status.WaitTime;
        while (enemy.transform.position.x != targetXposition)
        {

            initialVelocity = initialVelocity / time;

            float launchAngle = Mathf.Atan((height * Mathf.PI) / (targetXposition * 2f));

            launchAngle = launchAngle * Mathf.Rad2Deg;

            float initialVelocityX = initialVelocity * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
            float initialVelocityY = initialVelocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad);
            enemy._rb.velocity = new Vector3(initialVelocityX, initialVelocityY, 0f);
            yield break;
        }
    }
}
