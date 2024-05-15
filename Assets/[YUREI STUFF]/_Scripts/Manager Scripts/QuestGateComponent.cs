using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class QuestGateComponent : MonoBehaviour, IInteractable, IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public StateObserver StateObserver { get; private set; }
    [field: SerializeField] public SO_StoryData NoMissionTakenDialogue { get; private set; }
    [field: SerializeField] public GameObject DepartPrompt { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent DepartEvent { get; private set; }

    [SerializeField]private GenericUIState _uiState;
    [SerializeField]private HubState _hubState;

    [field: SerializeField] public GameObject FirstSelected { get; private set; }

    private EventSystem _eventSystem;
    [SerializeField] private InputActionAsset _input;
    [SerializeField] private string _inputName = "UI";

    private void Start()
    {
        _eventSystem = EventSystem.current;
        AudioCollection.InitializeStartData();
    }
    private void OnEnable()
    {
        _input.FindActionMap(_inputName).FindAction("Cancel").performed += Cancel;
    }
    private void OnDisable()
    {
        _input.FindActionMap(_inputName).FindAction("Cancel").performed -= Cancel;
    }
    public void ConfirmFunction()
    {
        if (!DepartPrompt.activeInHierarchy) return;
        AudioCollection.Play_OneShot("Start");
        DepartEvent.Raise(Observer.AssignedMission);
        DepartPrompt.SetActive(false);

    }
    private void Cancel(InputAction.CallbackContext context)
    {
        CancelFunction();
    }
    public void CancelFunction()
    {
        if (!DepartPrompt.activeInHierarchy) return;
        AudioCollection.Play_OneShot("Cancel");
        DepartPrompt.SetActive(false);
        ChangeStateEvent.Raise(_hubState);
    }
    public void OnResume()
    {
        if (DepartPrompt.activeInHierarchy)
        {
            _eventSystem.SetSelectedGameObject(FirstSelected);
        }
    }
    public void OnInteract()
    {
        if (Observer.AssignedMission == null)
        {
            NoMissionTakenDialogue.StartStoryDialogue();
            return;
        }
        if (!DepartPrompt.activeInHierarchy)
        {
            ChangeStateEvent.Raise(_uiState);
            DepartPrompt.SetActive(true);
            _eventSystem.SetSelectedGameObject(FirstSelected);
            AudioCollection.Play_OneShot("Confirm");

        }

    }
}

