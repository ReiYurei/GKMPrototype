using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[System.Serializable]
public class StateCondition 
{
    [SerializeField] EnemyStates name;
    public EnemyStates GetName()
    {
        return name;
    }

   // [SerializeField] bool isBoolean;
  //  [ShowIf(nameof(isBoolean), false)] public ComparatorType condition;
  //  [ShowIf(nameof(isBoolean),false)]public FloatVariable metric1;
  //  [ShowIf(nameof(isBoolean), false)]public float metric2;
  //
  //  [ShowIf(nameof(isBoolean), true)] public BooleanComparatorType boolCondition;
  //  [ShowIf(nameof(isBoolean), true)] public BooleanVariable boolMetric1;
  //  [ShowIf(nameof(isBoolean), true)] public bool boolMetric2;
    [Required][InlineEditor]public SO_Enemy_States state;
    
  /*  public bool CheckValue()
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
                if (boolMetric1.value == boolMetric2)
                {

                    return true;
                }
                else return false;
            case BooleanComparatorType.Inequal:
                if (boolMetric1.value != boolMetric2)
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
                    if (metric1.value == metric2)
                    {

                        return true;
                    }
                    else return false;
               case ComparatorType.Inequal:
                    if (metric1.value != metric2)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.GreaterThan:
                    if (metric1.value > metric2)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.GreaterThanOrEqual:
                    if (metric1.value >= metric2)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.LessThan:
                    if (metric1.value < metric2)
                    {

                        return true;
                    }
                    else return false;
                case ComparatorType.LessThanOrEqual:
                    if (metric1.value <= metric2)
                    {

                        return true;
                    }
                    else return false;
                default : return false;
           }
    }*/
}

