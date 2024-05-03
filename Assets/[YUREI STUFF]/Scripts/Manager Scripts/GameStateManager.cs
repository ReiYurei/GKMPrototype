using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EventListenerComponent))]
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    [field: SerializeField] public StateObserver CurrentState { get; private set; }

    [field: Header("Events")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ResumeEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent MissionFailedEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ReturnToTitleEvent { get; private set; }

    [field: Header("Pause Canvas")]
    [field: SerializeField] public PauseState PauseState { get; private set; }
    [field: SerializeField] public GameObject PausePrompt { get; private set; }
    [field: SerializeField] public GameObject AbandonMissionOption { get; private set; }

    [field: Header("Other")]
    [field: SerializeField] public InputActionAsset Input { get; private set; }
    [field: SerializeField] public GameObject FirstSelected { get; private set; }
    private EventSystem _eventSystem;


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
    public void OnEnable()
    {
        _eventSystem = EventSystem.current;
        Input.FindActionMap("UI").FindAction("Pause").performed += PauseGame;
        Input.FindActionMap("UI").FindAction("Cancel").performed += ResumeGame;

    }
    public void OnDisable()
    {
        Input.FindActionMap("UI").FindAction("Pause").performed -= PauseGame;
        Input.FindActionMap("UI").FindAction("Cancel").performed -= ResumeGame;


    }
    private void Start()
    {
    }
    public void OnStateChange(ScriptableObject data) //Listen to Event
    {
        var state = data as BaseGameState;
        CurrentState.SetCurrentState(state);
    }
    public void OnOverallStateChange(ScriptableObject data)
    {
        var state = data as BaseGameState;
        CurrentState.SetOverallState(state);
    }
    public void PauseGameFunction()
    {
        PausePrompt.SetActive(true);
        if (CurrentState.OverallState is RegularGameplayState ||
            CurrentState.OverallState is BulletHellGameplayState) AbandonMissionOption.SetActive(true);   
        else AbandonMissionOption.SetActive(false);
 
        _eventSystem.SetSelectedGameObject(FirstSelected);
        CurrentState.SetPreviousState(CurrentState.State);
        ChangeStateEvent.Raise(PauseState);
        Time.timeScale = 0f;
    }
    public void PauseGame(InputAction.CallbackContext context)
    {
        if(PausePrompt.activeInHierarchy)
        {
            ResumeGameFunction();
            return;
        }
        PauseGameFunction();
    }
    public void ReturnToTitle()
    {
        PausePrompt.SetActive(false);
        ReturnToTitleEvent.Raise();
        Time.timeScale = 1f;

    }
    public void AbandonMission()
    {
        PausePrompt.SetActive(false);
        Time.timeScale = 1f;
        MissionFailedEvent.Raise();

    }
    public void ResumeGameFunction()
    {
        if (!PausePrompt.activeInHierarchy) return;
        PausePrompt.SetActive(false);
        ResumeEvent.Raise();
        ChangeStateEvent.Raise(CurrentState.PreviousState);
        Time.timeScale = 1f;
    }
    public void ResumeGame(InputAction.CallbackContext context)
    {
        ResumeGameFunction();
    }
}
