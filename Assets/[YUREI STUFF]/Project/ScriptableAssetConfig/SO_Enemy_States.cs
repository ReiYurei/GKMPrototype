using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
#if UNITY_EDITOR

#endif

[System.Serializable]
[CreateAssetMenu(fileName = "States_Default", menuName = "Enemy/Enemy Behaviour/Enemy State")]
public class SO_Enemy_States : ScriptableObject
{
    [InlineEditor]
    public List<SO_Enemy_Substate> _subStates;
    public IEnumerator Execute(Enemy enemy ,int subState)
    {       
        SetAnimation(enemy._status, subState);
        yield return enemy._enemyBehaviour.StartCoroutine(_subStates[subState].Execute(enemy)); ;

    }
    public void SetAnimation(Enemy_Status status, int subState)
    {
        status.SetAnimationHash(GetAnimation(subState));
    }
    public int GetAnimation(int subState)
    {
        return _subStates[subState].GetAnimation();
    }
}
