using UnityEngine;

[CreateAssetMenu(fileName = "String Variable", menuName = "Variable/String")]
public class StringVariable : CustomVariable
{
    public new string value;
    public string GetValue()
    {
        return value;
    }
}
