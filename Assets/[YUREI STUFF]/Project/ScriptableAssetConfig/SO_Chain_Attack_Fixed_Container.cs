using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chain Moveset", menuName = "Enemy/Moveset/Chain Moveset Container")]
public class SO_Chain_Attack_Fixed_Container : SO_Enemy_Substate
{
    public List<SO_Base_Attack_Fixed> chainMoveset;
    int index = 0;
    [SerializeField] bool isHalved;

    private void OnEnable()
    {
        index = 0;
    }
    public override IEnumerator Execute(Enemy enemy)
    {
        if (isHalved == true)
        {
            enemy.status.SetHalvedAnim(isHalved);
        }
        index = 0;
        for (int i = 0; i < chainMoveset.Count; i++)
        {
            if (i == chainMoveset.Count)
            {
                enemy.status.SetHalvedAnim(false);
                enemy.status.SetAnimationHashAndNotify(GetAnimation());
                enemy.status.NotifyAttacking(false);
                break;
            }
            enemy.status.SetAnimationHashAndNotify(GetAnimation());
            yield return enemy.enemyBehaviour.StartCoroutine(chainMoveset[i].Execute(enemy));
            index++;
        }

        index = 0;
        enemy.status.SetHalvedAnim(false);
        yield break;
    }

    public override int GetAnimation()
    {
        return chainMoveset[index].GetAnimation();
    }
}
