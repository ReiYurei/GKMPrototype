using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "Boolean Variable", menuName = "Variable/Boolean")]
public class BooleanVariable : CustomVariable
{
    public new bool value;
}


public abstract class CustomVariable : ScriptableObject
{
    public object value;
}
