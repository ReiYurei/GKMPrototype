using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class SO_Enemy_Substate : ScriptableObject
{
    public abstract void Execute();
    public abstract string GetName();
}
