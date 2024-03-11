using JetBrains.Annotations;
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
    public override IEnumerator Execute(Enemy enemy)
    {
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
        throw new NotImplementedException();
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
                return TriValidationResult.Error("Types between two condition must be the SAME TYPE");
            }
        }
        return TriValidationResult.Valid;
    }
}
[System.Serializable]
public class AttackCondition<T, U> where T : CustomVariable where U : CustomVariable
{
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
                if (value1.value == value2.value)
                {
                    return true;
                }
                else return false;
            case ComparatorType.Inequal:
                if (value1.value != value2.value)
                {

                    return true;
                }
                else return false;
            case ComparatorType.GreaterThan when value1 is FloatVariable && value2 is FloatVariable:
                var value1Gthan = (FloatVariable)(object)value1;
                var value2Gthan = (FloatVariable)(object)value2;
                if (value1Gthan.value > value2Gthan.value)
                {

                    return true;
                }
                else return false;
            case ComparatorType.GreaterThanOrEqual when value1 is FloatVariable && value2 is FloatVariable:
                var value1GthanEq = (FloatVariable)(object)value1;
                var value2GthanEq = (FloatVariable)(object)value2;
                if (value1GthanEq.value >= value2GthanEq.value)
                {

                    return true;
                }
                else return false;

            case ComparatorType.LessThan when value1 is FloatVariable && value2 is FloatVariable:
                var value1Lthan = (FloatVariable)(object)value1;
                var value2Lthan = (FloatVariable)(object)value2;
                if (value1Lthan.value < value2Lthan.value)
                {

                    return true;
                }
                else return false;
            case ComparatorType.LessThanOrEqual when value1 is FloatVariable && value2 is FloatVariable:
                var value1LthanE = (FloatVariable)(object)value1;
                var value2LthanE = (FloatVariable)(object)value2;
                if (value1LthanE.value <= value2LthanE.value)
                {

                    return true;
                }
                else return false;
            default: return false;
        }
    }
}