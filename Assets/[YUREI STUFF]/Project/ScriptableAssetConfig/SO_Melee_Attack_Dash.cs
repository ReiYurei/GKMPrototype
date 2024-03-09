using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Dash", menuName = "Enemy/Moveset/Melee/Dash")]
public class SO_Melee_Attack_Dash : SO_Base_Attack_Fixed
{
    [SerializeField] float travelDistance;
    [SerializeField] float travelSpeed = 1;
   
    public override IEnumerator Execute(Enemy enemy)
    {
        if (playerInfo.position.x < enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance) * -1;
        }
        else if (playerInfo.position.x > enemy.transform.position.x)
        {
            travelDistance = Mathf.Abs(travelDistance);
        }
        float time = enemy._status.WaitTime;
        Vector3 dashPosition = new Vector3(enemy.transform.position.x + travelDistance, enemy.transform.position.y, enemy.transform.position.z);
        while(time > 0)
        {
            time -= Time.deltaTime;
            while (enemy.transform.position != dashPosition)
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, dashPosition, travelSpeed * Time.deltaTime);
                yield return null;
            }
            yield return null;

        }
        yield break;
    }

}
