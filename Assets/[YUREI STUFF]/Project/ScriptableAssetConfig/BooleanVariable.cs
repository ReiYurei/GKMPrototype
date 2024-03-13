using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "Boolean Variable", menuName = "Variable/Boolean")]
public class BooleanVariable : CustomVariable, IBoolVariable
{
    public new bool value;

    public bool GetValue()
    {
        return value;
    }
}


public abstract class CustomVariable : ScriptableObject
{
    public object value;
}
