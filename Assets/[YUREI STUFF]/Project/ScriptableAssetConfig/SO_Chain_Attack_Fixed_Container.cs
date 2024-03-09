using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chain Moveset", menuName = "Enemy/Moveset/Chain Moveset Container")]
public class SO_Chain_Attack_Fixed_Container : SO_Enemy_Substate
{
    public List<SO_Base_Attack_Fixed> chainMoveset;
    int index = 0;
    private void OnEnable()
    {
        index = 0;
    }
    public override IEnumerator Execute(Enemy enemy)
    {
        index = 0;
        for (int i = 0; i < chainMoveset.Count; i++)
        {
            enemy._status.SetAnimationHash(GetAnimation());
            yield return enemy._enemyBehaviour.StartCoroutine(chainMoveset[i].Execute(enemy));
            index++;

        }
        index = 0;
        yield break;
    }

    public override int GetAnimation()
    {
        return chainMoveset[index].GetAnimation();
    }
}
