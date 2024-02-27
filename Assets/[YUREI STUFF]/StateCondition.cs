using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[System.Serializable]
public class StateCondition 
{
    [SerializeField] string name;
    [SerializeField] bool isBoolean;
    [ShowIf(nameof(isBoolean), false)] public ComparatorType condition;
    [ShowIf(nameof(isBoolean),false)]public FloatVariable metric1;
    [ShowIf(nameof(isBoolean), false)]public FloatVariable metric2;

    [SerializeField] bool useConstant;
    [ShowIf(nameof(isBoolean), true)] public BooleanComparatorType boolCondition;
    [ShowIf(nameof(isBoolean), true)] public BooleanVariable boolMetric1;
    [ShowIf(nameof(isBoolean), true)] public BooleanVariable boolMetric2;
    public SO_Enemy_States state;
    
    public bool CheckValue()
    {
        if (isBoolean)
        {
            return BooleanCheck();
        }
        else
        {
            return ConditionCheck();
        }
    }

    private bool BooleanCheck()
    {
        switch (boolCondition)
        {
            case BooleanComparatorType.Equal:
                if (boolMetric1.value == boolMetric2.value)
                {

                    return true;
                }
                else return false;
            case BooleanComparatorType.Inequal:
                if (boolMetric1.value != boolMetric2.value)
                {

                    return true;
                }
                else return false;

            default: return false;
        }
    }
    private bool ConditionCheck()
    {
           switch (condition)
           {
               case ComparatorType.Equal:
                    if (metric1.value == metric2.value)
                    {

                        return true;
                    }
                    else return false;
               case ComparatorType.Inequal:
                    if (metric1.value != metric2.value)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.GreaterThan:
                    if (metric1.value > metric2.value)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.GreaterThanOrEqual:
                    if (metric1.value >= metric2.value)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.LessThan:
                    if (metric1.value < metric2.value)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.LessThanOrEqual:
                    if (metric1.value <= metric2.value)
                    {

                        return true;
                    }
                    else return false;
                default : return false;
           }
    }
}

