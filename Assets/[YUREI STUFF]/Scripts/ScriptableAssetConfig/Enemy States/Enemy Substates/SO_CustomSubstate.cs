using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "Substates_Custom", menuName = "Enemy/Enemy Behaviour/Enemy Substate/Custom")]
public class SO_CustomSubstate : SO_Enemy_Substate
{
    [SerializeField]private int parameterName;
    
    [SerializeField]private List<CustomType> data;


    public override IEnumerator Execute(Enemy enemy)
    {
        yield break;
    }
    public override int GetAnimation()
    {
        return parameterName;
    }


    public string GetStringValue(int index)
    {
        return data[index].GetStringValue();
    }
    public int GetIntValue(int index)
    {
        return data[index].GetIntValue();
    }
    public float GetFloatValue(int index)
    {
        return data[index].GetFloatValue();
    }
    public bool GetBooleanValue(int index)
    {
        return data[index].GetBooleanValue();
    }
}
[System.Serializable]
public class CustomType
{
   public enum Type { String,Float,Int,Boolean}

    public string _stringValue;
    public int _intValue;
    public float _floatValue;
    public bool _boolValue;
    public string GetStringValue()
    {
        return _stringValue;
    }
    public float GetFloatValue()
    {
        return _floatValue;
    }
    public int GetIntValue()
    {
        return _intValue;
    }
    public bool GetBooleanValue()
    {
        return _boolValue;
    }

}
