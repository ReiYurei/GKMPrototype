using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
#if UNITY_EDITOR

#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_Substate", menuName = "Enemy/Moveset/Condition Moveset")]
public class SO_Condition_Substate : SO_Enemy_Substate
{
    public List<DynamicAttack> potentialMove;
    private int[] _weight;
    private int _highestWeight;

    public override IEnumerator Execute(Enemy enemy)
    {
        _highestWeight = 0;
        _weight = new int[potentialMove.Count];
        for (int i = 0; i < potentialMove.Count; i++)
        {
            if (potentialMove[i].CheckCondition()) 
            {
                if (potentialMove[i].moveset is SO_Base_Attack_Fixed)
                {
                    var attack = potentialMove[i].moveset as SO_Base_Attack_Fixed;
                    _weight[i] = attack.motionValue + potentialMove[i].Weight;
                    if (_highestWeight < _weight[i]) _highestWeight = _weight[i];
                    continue;
                }
            }
            _weight[i] = potentialMove[i].Weight;
            if (_highestWeight < _weight[i]) _highestWeight = _weight[i];
        }
        for (int i = 0; i < _weight.Length; i++)
        {
            if (_weight[i] >= _highestWeight)
            {
                yield return potentialMove[i].moveset.Execute(enemy);
                break;
            }
        }
    }

    public override int GetAnimation()
    {
        _highestWeight = 0;
        _weight = new int[potentialMove.Count];
        for (int i = 0; i < potentialMove.Count; i++)
        {
            if (potentialMove[i].CheckCondition())
            {
                if (potentialMove[i].moveset is SO_Base_Attack_Fixed)
                {
                    var attack = potentialMove[i].moveset as SO_Base_Attack_Fixed;
                    _weight[i] = attack.motionValue + potentialMove[i].Weight;
                    if (_highestWeight < _weight[i]) _highestWeight = _weight[i];
                    continue;
                }
            }
            _weight[i] = potentialMove[i].Weight;
            if (_highestWeight < _weight[i]) _highestWeight = _weight[i];
        }
        return AnimationHash.Enemy_Idle;

    }
}

[System.Serializable]
public class DynamicAttack
{

    [InlineEditor] public SO_Enemy_Substate moveset;
    [ValidateInput(nameof(ValidateVariable))]
    public List<Condition<CustomVariable, CustomVariable>> conditions;
    private bool[] isFulfilled;
    public int Weight {  get; private set; }
    public bool CheckCondition()
    {
        if (conditions == null || conditions.Count <= 0) return true;
        isFulfilled = new bool[conditions.Count];
        Weight = 0;
        for (int i = 0; i < conditions.Count; i++)
        {
            isFulfilled[i] = conditions[i].CheckFullfilment();
            if (isFulfilled[i])
            {
                Weight = conditions.Count * 10;
                continue;
            }
            Weight = 0;
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
