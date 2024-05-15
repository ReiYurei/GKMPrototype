using TriInspector;
[System.Serializable]
public class Condition<T, U> where T : CustomVariable where U : CustomVariable //Used for statemachine
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