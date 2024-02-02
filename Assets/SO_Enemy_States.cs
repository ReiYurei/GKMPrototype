using System.Collections;
using System.Collections.Generic;
using TriInspector;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "States_Default", menuName = "Enemy/Enemy Behaviour/Enemy State")]
public class SO_Enemy_States : ScriptableObject
{
    [InlineEditor]
    public List<SO_Enemy_Substate> _subStates;
    public void UseState(int subState)
    {
        _subStates[subState].Execute();

    }
}
