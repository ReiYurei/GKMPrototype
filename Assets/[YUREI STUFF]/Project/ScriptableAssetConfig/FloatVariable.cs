using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[CreateAssetMenu(fileName = "Float Variable", menuName ="Variable/Float")]
public class FloatVariable : CustomVariable
{
    public new float value;
}


[System.Serializable]
public class FloatReference
{
    [SerializeField] private bool useConstant;
    [SerializeField][ShowIf(nameof(useConstant), false)] private FloatVariable _Variable;
    [SerializeField][ShowIf(nameof(useConstant), true)] private float _ConstantValue;
    public float Value
    {
        get { return useConstant ? _ConstantValue : _Variable.value; }
        set { if (useConstant) _ConstantValue = value; else _Variable.value = value; }
    }


}