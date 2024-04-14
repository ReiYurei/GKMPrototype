using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EventListenerComponent))]
public class InputManager : MonoBehaviour
{
    public InputActionAsset input;
    public void OnStateChange(ScriptableObject data)
    {
        Debug.Log("CALLED");
 
        var state = data as BaseGameState;
        switch (state)
        {
            case HubState:
                DisableInput();
                input.FindActionMap("Hub").Enable();
                Debug.Log("Hub Mapping Enabled");

                break;
            case CutsceneState:
                DisableInput();
                input.FindActionMap("Cutscene").Enable();
                Debug.Log("Cutscene Mapping Enabled");

                break;
            case ExterminateState:
                DisableInput();
                input.FindActionMap("Exterminate").Enable();
                Debug.Log("Exterminate Mapping Enabled");

                break;
            case ListingState:
                DisableInput();
                input.FindActionMap("Listing").Enable();
                Debug.Log("Listing Mapping Enabled");
                break;
        }
        void DisableInput()
        {
            foreach (var inputMap in input.actionMaps)
            {
                inputMap.Disable();
            }
        }
    }
}
