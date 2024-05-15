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
    public List<Condition<CustomVariable, CustomVariable>> conditions;
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
