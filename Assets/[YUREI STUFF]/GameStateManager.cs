using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class GameStateManager : MonoBehaviour 
{
    [field: SerializeField]public SO_ParameterGameEvent StateChangeEvent {  get; private set; }
    [field: SerializeField]public BaseGameState CurrentState { get; private set; }

    public void SetGameState(BaseGameState state)
    {
        CurrentState = state;
        StateChangeEvent.Raise(state);
    }
}
