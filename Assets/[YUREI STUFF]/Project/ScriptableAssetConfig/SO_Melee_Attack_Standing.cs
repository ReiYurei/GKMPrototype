using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Standing", menuName = "Enemy/Moveset/Melee/Standing")]
public class SO_Melee_Attack_Standing : SO_Base_Attack_Fixed
{
    public override IEnumerator Execute(Enemy enemy)
    {
        float time = enemy._status.WaitTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }


}
