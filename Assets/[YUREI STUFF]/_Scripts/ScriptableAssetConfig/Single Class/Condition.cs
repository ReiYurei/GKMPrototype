using TriInspector;
public enum ConditionType { Int, Float, Bool }
[System.Serializable]
public class Condition<T, U> where T : CustomVariable where U : CustomVariable //Used for statemachine
{
    [ValidateInput(nameof(ValidateVariable))]
    public bool useConstant;
    [ShowIf(nameof(useConstant), true)] public ConditionType conditionType;


    public ComparatorType comparatorType;
    public T variable1;
    [ShowIf(nameof(useConstant), true)][ShowIf(nameof(conditionType), ConditionType.Int)] public int intValue;
    [ShowIf(nameof(useConstant), true)][ShowIf(nameof(conditionType), ConditionType.Float)] public float floatValue;
    [ShowIf(nameof(useConstant), true)][ShowIf(nameof(conditionType), ConditionType.Bool)] public bool boolValue;
    [ShowIf(nameof(useConstant), false)] public U variable2;
    public bool CheckFullfilment()
    {
        if (useConstant) return CompareCondition(variable1);
        return CompareCondition(variable1, variable2);
    }

    private bool CompareCondition(T value1)
    {
        switch (comparatorType)
        {
            case ComparatorType.Equal:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() == intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() == floatValue;
                }
                if (conditionType == ConditionType.Bool)
                {
                    var value = value1 as BooleanVariable;
                    return value.GetValue() == boolValue;
                }

                break;
            case ComparatorType.Inequal:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() != intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() != floatValue;
                }
                if (conditionType == ConditionType.Bool)
                {
                    var value = value1 as BooleanVariable;
                    return value.GetValue() != boolValue;
                }

                break;

            case ComparatorType.GreaterThan:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() > intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() > floatValue;
                }

                break;
            case ComparatorType.GreaterThanOrEqual:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() >= intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() >= floatValue;
                }

                break;
            case ComparatorType.LessThan:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() < intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() < floatValue;
                }

                break;
            case ComparatorType.LessThanOrEqual:
                if (conditionType == ConditionType.Int)
                {
                    var value = value1 as IntVariable;
                    return value.GetValue() <= intValue;
                }
                if (conditionType == ConditionType.Float)
                {
                    var value = value1 as FloatVariable;
                    return value.GetValue() <= floatValue;
                }

                break;
        }
        return false;

    }
    private bool CompareCondition(T value1, U value2)
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
        if (useConstant)
        {
            if (variable1 == null) return TriValidationResult.Error("ERROR : Variable 1 is NULL ");
            switch (conditionType)
            {
                case ConditionType.Int:
                    if (variable1 is not IntVariable)
                    {
                        return TriValidationResult.Error("ERROR : Variable 1 Must be type of CUSTOM INTEGER ");
                    }
                    break;

                case ConditionType.Float:
                    if (variable1 is not FloatVariable)
                    {
                        return TriValidationResult.Error("ERROR : Variable 1 Must be type of CUSTOM FLOAT ");
                    }
                    break;
                case ConditionType.Bool:
                    if (variable1 is not BooleanVariable)
                    {

                        return TriValidationResult.Error("ERROR : Variable 1 Must be type of CUSTOM BOOLEAN ");
                    }
                    if (comparatorType == ComparatorType.Equal || comparatorType == ComparatorType.Inequal)
                    {
                        return TriValidationResult.Valid;
                    }
                    return TriValidationResult.Error("ERROR : Boolean Operation can only compared as EQUAL or INEQUAL");
            }
            return TriValidationResult.Valid;

        }
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