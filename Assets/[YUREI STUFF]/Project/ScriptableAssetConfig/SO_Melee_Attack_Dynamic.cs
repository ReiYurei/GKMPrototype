using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
#if UNITY_EDITOR

#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Moveset_Melee_Dynamic", menuName = "Enemy/Moveset/Melee/Dynamic Attack")]
public class SO_Melee_Attack_Dynamic : SO_Enemy_Substate
{
    public List<DynamicAttack> potentialMove;
    int[] priority;
    public override IEnumerator Execute(Enemy enemy)
    {
        priority = new int[potentialMove.Count];
        for (int i = 0; i < potentialMove.Count; i++)
        {
            if (potentialMove[i].CheckCondition() == true) 
            {
                yield return potentialMove[i].attack.Execute(enemy);
                break;
            }
        }
        yield break;
    }

    public override int GetAnimation()
    {
        for (int i = 0; i < potentialMove.Count; i++)
        {
            if (potentialMove[i].CheckCondition() == true)
            {
                return potentialMove[i].attack.GetAnimation();
            }
        }
        return AnimationHash.Enemy_Idle;

    }
}

[System.Serializable]
public class DynamicAttack
{

    [InlineEditor] public SO_Base_Attack_Fixed attack;
    [ValidateInput(nameof(ValidateVariable))]
    public List<AttackCondition<CustomVariable, CustomVariable>> conditions;
    bool[] isFulfilled;
    public bool CheckCondition()
    {
        isFulfilled = new bool[conditions.Count];
        for (int i = 0; i < conditions.Count; i++)
        {
            isFulfilled[i] = conditions[i].CheckFullfilment();
        }
        for (int i = 0; i < isFulfilled.Length; i++)
        {
            if (isFulfilled[i] == false)
            {
                return false; //Condition is not Met
            }

        }
        return true; //All Condition Met

    }

    TriValidationResult ValidateVariable()
    {
        if (conditions == null) return TriValidationResult.Valid;


        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].variable1 == null || conditions[i].variable2 == null)
            {
                return TriValidationResult.Valid;
            }
            if (conditions[i].variable1.GetType() != conditions[i].variable2.GetType())
            {
                return TriValidationResult.Error("ERROR : Types between two condition must be the SAME TYPE");
            }
        }
        return TriValidationResult.Valid;
    }
}
[System.Serializable]
public class AttackCondition<T, U> where T : CustomVariable where U : CustomVariable
{
    [ValidateInput(nameof(ValidateVariable))]
    public ComparatorType comparatorType;
    public T variable1;
    public U variable2;
    public bool CheckFullfilment()
    {
        return ConditionCheck(variable1, variable2);
    }
    private bool ConditionCheck(T value1, U value2)
    {
        switch (comparatorType)
        {
            case ComparatorType.Equal:
                if (value1 is IBoolVariable && value2 is IBoolVariable)
                {
                    return ((IBoolVariable)value1).GetValue() == ((IBoolVariable)value2).GetValue();
                }
                else if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() == ((INumericVariable)value2).GetValue();
                }
                break;
            case ComparatorType.Inequal:
                if (value1 is IBoolVariable && value2 is IBoolVariable)
                {
                    return ((IBoolVariable)value1).GetValue() != ((IBoolVariable)value2).GetValue();
                }
                else if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() != ((INumericVariable)value2).GetValue();
                }
                break;

            case ComparatorType.GreaterThan:
                if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() > ((INumericVariable)value2).GetValue();
                }
                break;
            case ComparatorType.GreaterThanOrEqual:
                if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() >= ((INumericVariable)value2).GetValue();
                }
                break;
            case ComparatorType.LessThan:
                if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() < ((INumericVariable)value2).GetValue();
                }
                break;
            case ComparatorType.LessThanOrEqual:
                if (value1 is INumericVariable && value2 is INumericVariable)
                {
                    return ((INumericVariable)value1).GetValue() <= ((INumericVariable)value2).GetValue();
                }
                break;
        }
        return false;

    }
    TriValidationResult ValidateVariable()
    {
        if (variable1 is IBoolVariable && variable2 is IBoolVariable)
        {
            if (comparatorType == ComparatorType.Equal || comparatorType == ComparatorType.Inequal)
            {
                return TriValidationResult.Valid;
            }
            return TriValidationResult.Error("ERROR : Boolean Operation can only compared as EQUAL or INEQUAL");

        }
        return TriValidationResult.Valid;

    }

}