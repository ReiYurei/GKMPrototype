using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[System.Serializable]
public class FixedState 
{
    [SerializeField] EnemyStates name;
    public EnemyStates GetName()
    {
        return name;
    }

    [Required][InlineEditor]public SO_Enemy_States state;
    
}

[System.Serializable]
[CreateAssetMenu(fileName = "Dynamic State", menuName = "Enemy/Enemy Behaviour/Enemy Dynamic State")]
public class SO_DynamicState : ScriptableObject
{
    //public GameplayType gameplayType;
    [InlineEditor] public SO_Enemy_States states;
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
    public void Execute(Enemy enemy,int subStateIndex)
    {
        states._subStates[subStateIndex].Execute(enemy);
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
public enum GameplayType
{
    Side_Scroll,Bullet_Hell
}