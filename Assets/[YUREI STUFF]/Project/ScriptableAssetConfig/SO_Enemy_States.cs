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
    int[] priority;

    public IEnumerator Execute(Enemy enemy ,int subState)
    {       
        SetAnimation(enemy.StatusData, subState);
        yield return enemy.EnemyBehaviourComponent.StartCoroutine(_subStates[subState].Execute(enemy));

    }
    public void SetAnimation(EnemyStatus status, int subState)
    {
        status.SetAnimationHashAndNotify(GetAnimation(subState));
    }
    public int GetAnimation(int subState)
    {
        return _subStates[subState].GetAnimation();
    }
}
