using UnityEngine;
using TriInspector;
[CreateAssetMenu(fileName = "Current State Observer", menuName ="Game States/State Observer")]
[System.Serializable]
public class StateObserver : ScriptableObject
{
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public BaseGameState OverallState { get; private set; }
    [field: SerializeField] public BaseGameState State { get; private set; }
    [field: SerializeField] public BaseGameState PreviousState { get; private set; }

    [Button("Raise Event For Debugging")]
    public void Raise()
    {
        ChangeStateEvent.Raise(State);
    }
    public void SetCurrentState(BaseGameState state)
    {
        State = state;
        SetOverallState(state);
        Debug.Log("Current State : "+ State.name);
    }
    public void SetPreviousState(BaseGameState previousState)
    {
        PreviousState = previousState;
        Debug.Log("Previous State :" + PreviousState.name);
    }
    public void SetOverallState(BaseGameState state)
    {
        switch (state)
        {
            case HubState:
                OverallState = state;
                break;
            case ExterminateState:
                OverallState = state;
                break;
  
        }
    }
}