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

    [SerializeField] private bool _limitFramerate;
    [SerializeField] private int _frameRate;
    [SerializeField] private TitleScreenState _titleScreenState;
    [SerializeField] private LoadingScreenState _disableInputState;

    [SerializeField] private GameObject _pressStartPrompt;
    [SerializeField] private GameObject _menuButtons;
    [SerializeField] private GameObject _credits;

    [SerializeField] private InputActionAsset _input;
    private EventSystem _eventSystem;
    private void Start()
    {
        if (_limitFramerate) Application.targetFrameRate = _frameRate;
        _eventSystem = EventSystem.current;
        AudioCollection.InitializeStartData();
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
        AudioManager.Instance.MusicCollection.Play("Title Screen");
        ChangeStateEvent.Raise(_titleScreenState);
    }
    private void TitleStart(InputAction.CallbackContext context)
    {
        _input.FindActionMap("UI").FindAction("Confirm").performed -= TitleStart;
        AudioCollection.Play_OneShot("Title");
        _pressStartPrompt.SetActive(false);
        _menuButtons.SetActive(true);
        _eventSystem.SetSelectedGameObject(FirstSelected);

    }
    public void StartGame()
    {
        StartGameEvent.Raise();
        AudioCollection.Play_OneShot("Start");
        AudioManager.Instance.MusicCollection.StopInstance("Title Screen", "Volume", 0,1,2.5f);

    }
    public void ShowCredit()
    {
        _credits.SetActive(true);
        ChangeStateEvent.Raise(_disableInputState);
        AudioCollection.Play_OneShot("Confirm");
        _input.FindActionMap("UI").FindAction("Cancel").Enable();
    }
    public void Cancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(_credits.activeInHierarchy) _credits.SetActive(false);
        AudioCollection.Play_OneShot("Cancel");

        ChangeStateEvent.Raise(_titleScreenState);
    }
    public void Quit()
    {
        AudioCollection.Play_OneShot("Confirm");
        Application.Quit();
    }
}
