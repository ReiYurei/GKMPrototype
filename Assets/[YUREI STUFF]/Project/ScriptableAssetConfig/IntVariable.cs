using UnityEngine;

[CreateAssetMenu(fileName = "Integer Variable", menuName = "Variable/Integer")]
public class IntVariable : CustomVariable
{
    public new int value;
    public float GetValue()
    {
        return value;
    }
}
