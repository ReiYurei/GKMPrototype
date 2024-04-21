using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EventListenerComponent))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance {  get; private set; } 
    public InputActionAsset input;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void OnEnable()
    {
        foreach (var inputMap in input.actionMaps)
        {
            inputMap.Disable();
        }
        input.FindActionMap("UI").Enable();
    }
    public void OnStateChange(ScriptableObject data)
    { 
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
            case ListingState:
                DisableInput();
                input.FindActionMap("Listing").Enable();
                Debug.Log("Listing Mapping Enabled");
                break;
            case BulletHellGameplayState:
                DisableInput();

                break;
            case RegularGameplayState:
                DisableInput();
                
                break;
            case PauseState:
                DisableInput();
                input.FindActionMap("UI").Enable();
                Debug.Log("UI Mapping Enabled");
                break;
            case GenericUIState:
                DisableInput();
                input.FindActionMap("UI").Enable();
                Debug.Log("UI Mapping Enabled");
                break;
            case LoadingScreenState:
                DisableInput();
                Debug.Log("Disabled All Input");
                return;
            case TitleScreenState:
                DisableInput();
                input.FindActionMap("UI").Enable();
                input.FindActionMap("UI").FindAction("Pause").Disable();
                return;
        }
        input.FindActionMap("UI").Enable();


        void DisableInput()
        {
            foreach (var inputMap in input.actionMaps)
            {
                inputMap.Disable();
            }
        }

    }
}
