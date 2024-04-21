using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventListenerComponent))]
public class TitleScreenComponent : MonoBehaviour, IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent StartGameEvent { get; private set; }
    [field: SerializeField]public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public GameObject FirstSelected { get; private set; }


    [SerializeField] private TitleScreenState _titleScreenState;
    [SerializeField] private LoadingScreenState _disableInputState;

    [SerializeField] private GameObject _pressStartPrompt;
    [SerializeField] private GameObject _menuButtons;
    [SerializeField] private GameObject _credits;

    [SerializeField] private InputActionAsset _input;
    private EventSystem _eventSystem;
    private void Start()
    {
        _eventSystem = EventSystem.current;
        _input.FindActionMap("UI").FindAction("Cancel").performed += Cancel;

    }
    private void OnDisable()
    {
        _input.FindActionMap("UI").FindAction("Cancel").performed -= Cancel;
    }
    public void OnLoadComplete() //Used by Events
    {
        _eventSystem = EventSystem.current;
        _input.FindActionMap("UI").FindAction("Confirm").performed += TitleStart;
        ChangeStateEvent.Raise(_titleScreenState);
    }
    private void TitleStart(InputAction.CallbackContext context)
    {
        _input.FindActionMap("UI").FindAction("Confirm").performed -= TitleStart;
        _pressStartPrompt.SetActive(false);
        _menuButtons.SetActive(true);
        _eventSystem.SetSelectedGameObject(FirstSelected);

    }
    public void StartGame()
    {
        StartGameEvent.Raise();
    }
    public void ShowCredit()
    {
        _credits.SetActive(true);
        ChangeStateEvent.Raise(_disableInputState);
        _input.FindActionMap("UI").FindAction("Cancel").Enable();
    }
    public void Cancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(_credits.activeInHierarchy) _credits.SetActive(false);
        ChangeStateEvent.Raise(_titleScreenState);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
