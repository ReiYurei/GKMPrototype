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
            enemy.StatusData.SetHalvedAnim(isHalved);
        }
        index = 0;
        for (int i = 0; i < chainMoveset.Count; i++)
        {
            if (i == chainMoveset.Count)
            {
                enemy.StatusData.SetHalvedAnim(false);
                enemy.StatusData.SetAnimationHashAndNotify(GetAnimation());
                enemy.StatusData.NotifyAttacking(false);
                break;
            }
            enemy.StatusData.SetAnimationHashAndNotify(GetAnimation());
            yield return enemy.EnemyBehaviourComponent.StartCoroutine(chainMoveset[i].Execute(enemy));
            index++;
        }

        index = 0;
        enemy.StatusData.SetHalvedAnim(false);
        yield break;
    }

    public override int GetAnimation()
    {
        return chainMoveset[index].GetAnimation();
    }
}
