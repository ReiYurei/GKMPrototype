using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class DialogueUIController : MonoBehaviour, IAudioSource 
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public StateObserver CurrentState { get; private set; }
    private string _previousMusic = "";
    public static DialogueUIController Instance { get; private set; }
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
    private void Start()
    {
        AudioCollection.InitializeStartData();
        AudioManager.Instance.MusicCollection.InitializeStartData();
        _previousMusic = "";
    }
    private void OnEnable()
    {
        _input.FindActionMap(_inputName).FindAction("Confirm").performed += OnSkipText;
        _input.FindActionMap(_inputName).FindAction("Skip").performed += ForceSkipAll;
        _input.FindActionMap(_inputName).FindAction("Skip").started += ForceSkipAll;
        _input.FindActionMap(_inputName).FindAction("Skip").canceled += ForceSkipAll;



    }


    private void OnDisable()
    {
        _input.FindActionMap(_inputName).FindAction("Confirm").performed -= OnSkipText;
        _input.FindActionMap(_inputName).FindAction("Skip").performed -= ForceSkipAll;
        _input.FindActionMap(_inputName).FindAction("Skip").started -= ForceSkipAll;
        _input.FindActionMap(_inputName).FindAction("Skip").canceled -= ForceSkipAll;

    }

    [field: SerializeField]public SO_Dialogue DialogueData { get;private set; }
    [field: Header("Canvas")]
    [field: SerializeField]public GameObject DialogueCanvas { get; private set; }
    [field: SerializeField]public Image LeftCharacter { get; private set; }
    [field: SerializeField]public Image RightCharacter { get; private set; }
    [field: SerializeField]public Image ConfirmIcon { get; private set; }

    [field: SerializeField]public TMPro.TextMeshProUGUI SpeechText { get; private set; }
    [field: SerializeField] public TMPro.TextMeshProUGUI NameText { get; private set; }
    [field: SerializeField] public GameObject SkipUI { get; private set; }
    [field: SerializeField] public Slider SkipSlider { get; private set; }
    [field: SerializeField] public float TextSpeed { get; private set; }
    [SerializeField] private string _inputName = "Cutscene";

    [field: Header("Event")]
    [field: SerializeField] public SO_VoidGameEvent SkipTextEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent SkipAllEvent { get; private set; }

    [field: SerializeField] public SO_VoidGameEvent DialogueEnd_DefaultHubEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent DialogueEnd_DefaultExterminateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ShakeEvent { get; private set; }


    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }


    [field: Header("Other")]
    [SerializeField] private InputActionAsset _input;
    [SerializeField] private CutsceneState _cutsceneState;
    [SerializeField] private HubState _hubState;
    [SerializeField] private ExterminateState _exterminateState;




    private float _speed = 20;
    private string _tmText;
    private bool _skipping;
    [SerializeField]private bool _skippable;
    [SerializeField]private bool _textRevealed;
    private int _dialogueIndex = 0;
    public void OnReturnToTitle()
    {
        _skipping = false;
        _skippable = false;
        _textRevealed = false;
        DialogueData = null;
        DialogueCanvas.SetActive(false);
    }
    public void ReadText()
    {
        StopAllCoroutines();
        _skipping = false;

        _speed = TextSpeed;
        _tmText = "";
        var data = DialogueData.dialogue[_dialogueIndex];
        if (data.AutoSkipAtEnd) _skippable = false;
        else _skippable = true;


        string[] subText = data.SpeechText.Split('<', '>');
        
        for (int i = 0; i < subText.Length; i++)
        {
            if (i % 2 == 0)
            {
                _tmText += subText[i];
            }
            else if (!isCustomTag(subText[i].Replace(" ", "")))
            {
                _tmText += $"<{subText[i]}>";
            }
        }
        bool isCustomTag(string tag)
        {
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") || tag.StartsWith("concurrent") 
                || tag.StartsWith("shake") || tag.StartsWith("event=");
        }
        SpeechText.text = _tmText;
        SpeechText.maxVisibleCharacters = 0;
        StartCoroutine(Read());
   
        IEnumerator Read()
        {
            int subCounter = 0;
            int visibleCounter = 0;
            yield return new WaitForSeconds(0.15f);
            while (subCounter < subText.Length)
            {
                if (subCounter % 2 == 1)
                {
                    yield return EvaluateTag(subText[subCounter].Replace(" ", ""));

                }
                else
                {
                    while (visibleCounter < subText[subCounter].Length)
                    {
                        visibleCounter++;
                        SpeechText.maxVisibleCharacters++;
                        AudioCollection.Play_OneShot("Blip", "Pitch", data.SpeechPitch);
                        yield return new WaitForSeconds(1f / _speed);


                    }
                    visibleCounter = 0;
                }
                subCounter++;
          
                yield return null;

            }
            if (data.AutoSkipAtEnd)
            {

                _textRevealed = true;
                AutoSkip();
                yield break;

            }
            _textRevealed = true;
            ConfirmIcon.gameObject.SetActive(true);


            //Text End
            WaitForSeconds EvaluateTag(string tag)
            {
                if (tag.Length > 0)
                {
                    if (tag.StartsWith("speed="))
                    {
                        _speed = float.Parse(tag.Split('=')[1]);
                    }
                    else if (tag.StartsWith("pause="))
                    {
                        if(_skipping) return new WaitForSeconds(float.Parse(tag.Split('=')[1])/_speed);
                        return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("emotion="))
                    {
                        //Example <emotion=happy>
                        var lowercase = tag.Split('=')[1].Substring(1, tag.Split('=')[1].Length -1).ToLower(); // return appy
                        var uppercase = tag.Split('=')[1].Substring(0,1).ToUpper(); //return H
                        var value = uppercase + lowercase; //H+appy
                        ShowPotrait((ExpressionID)Enum.Parse(typeof(ExpressionID), value));
                        return null;
                    }
                    else if (tag.StartsWith("concurrent"))
                    {
                        return null;
                    }
                    else if (tag.StartsWith("shake"))
                    {
                        ShakeEvent.Raise();
                        return null;
                    }
                    else if (tag.StartsWith("event="))
                    {
                        var key = tag.Split('=')[1];
                        if (data.VoidGameEvent.Key == key)
                        {
                            data.VoidGameEvent.Raise();
                        }
                        if(data.ParameterGameEvent.Key == key)
                        {
                            data.ParameterGameEvent.Raise();
                        }
                        return null;
                    }
                }
                return null;
            }
       
        }
        
    }
    private void DialogueEnd()
    {
        DialogueCanvas.SetActive(false);
        switch (DialogueData.endEventBehaviour)
        {
            case EndEventBehaviour.DefaultHubEvent:
                DialogueEnd_DefaultHubEvent.Raise();
                break;
            case EndEventBehaviour.DefaultExterminateEvent:
                DialogueEnd_DefaultExterminateEvent.Raise();
                break;
            case EndEventBehaviour.CustomEvent:
                DialogueData.CustomEndEvent.Raise();
                break;
            case EndEventBehaviour.None_ToExterminate:
                ChangeStateEvent.Raise(CurrentState.OverallState);
                break;
            case EndEventBehaviour.None_ToHub:
                ChangeStateEvent.Raise(CurrentState.OverallState);
                AudioManager.Instance.MusicCollection.Play("Hub");
                break;
        }
    }
    private void ResetProperty()
    {
        _dialogueIndex = 0;
    }
    private void CheckIndex()
    {
        if (_dialogueIndex >= DialogueData.dialogue.Count - 1)
        {
            DialogueEnd();
            ResetProperty();
            //Invoke CutsceneState End, change Gamestate
            return;

        }
        _dialogueIndex++;
        ShowPotrait();
        ReadText();
        ShowName();
        PlayMusic();
        ActiveSpeaker();
    }
    private void PlayMusic()
    {
        var data = DialogueData.dialogue[_dialogueIndex];
        if (data.Music == "" | data.Music == null) return;
        if (data.Music == _previousMusic) return;
        if (data.Music == "Stop")
        {
            if (AudioManager.Instance.MusicCollection.AudioEventsDict.Count == 0) return;
            AudioManager.Instance.MusicCollection.StopAllInstance("Volume", 0, 1, 2f);
            _previousMusic = "";
            return;
        }
        AudioManager.Instance.MusicCollection.StopInstance(_previousMusic);
        _previousMusic = data.Music;
        AudioManager.Instance.MusicCollection.Play(data.Music);
    }
    private void ShowName()
    {
        var data = DialogueData.dialogue[_dialogueIndex];
        NameText.text = data.SpeakerName;
        
    }
    private void ShowPotrait()
    {
        var data = DialogueData.dialogue[_dialogueIndex];
        var pathLeft = ResourcePath.GetSpritePath(data.CharacterLeft, data.InitialExpressionLeft);
        var pathRight = ResourcePath.GetSpritePath(data.CharacterRight, data.InitialExpressionRight);

        LeftCharacter.sprite = ResourcePath.GetSprite(pathLeft);
        RightCharacter.sprite = ResourcePath.GetSprite(pathRight);

        LeftCharacter.preserveAspect = true;
        RightCharacter.preserveAspect = true;
    }
    private void ShowPotrait(ExpressionID expression)
    {
        var data = DialogueData.dialogue[_dialogueIndex];
        var pathLeft = ResourcePath.GetSpritePath(data.CharacterLeft, expression);
        var pathRight = ResourcePath.GetSpritePath(data.CharacterRight, expression);
        var defaultPath = ResourcePath.GetSpritePath(CharacterID.None, ExpressionID.Default);

        switch (data.ActiveSpeaker)
        {
            case ActiveTalker.None:
                LeftCharacter.sprite = ResourcePath.GetSprite(defaultPath);
                RightCharacter.sprite = ResourcePath.GetSprite(defaultPath);
                break;
            case ActiveTalker.Left:
                LeftCharacter.sprite = ResourcePath.GetSprite(pathLeft);
                break;
            case ActiveTalker.Right:
                RightCharacter.sprite = ResourcePath.GetSprite(pathRight);
                break;
            case ActiveTalker.Both:
                LeftCharacter.sprite = ResourcePath.GetSprite(pathLeft);
                RightCharacter.sprite = ResourcePath.GetSprite(pathRight);
                break;
        }
    }
    private void ActiveSpeaker()
    {
        var data = DialogueData.dialogue[_dialogueIndex];
        var gray = new Color(0.5f,0.5f,0.5f,1f);
        var white = Color.white;
        switch (data.ActiveSpeaker)
        {
            case ActiveTalker.None:
                LeftCharacter.color = gray;
                RightCharacter.color =gray;
                break;
            case ActiveTalker.Left:
                LeftCharacter.color = white;
                RightCharacter.color = gray;
                break;
            case ActiveTalker.Right:
                LeftCharacter.color = gray;
                RightCharacter.color = white;
                break;
            case ActiveTalker.Both:
                LeftCharacter.color = white;
                RightCharacter.color = white;
                break;
        }
    }
    public void OnCutsceneStart(ScriptableObject data)
    {
        SkipUI.SetActive(false);
        Debug.Log("Cutscene Start");
        var dialogueData = data as SO_Dialogue;
        //gameState = GameState.Cutscene;//For Debugging
        ChangeStateEvent.Raise(_cutsceneState);
        DialogueCanvas.SetActive(true);
        ConfirmIcon.gameObject.SetActive(false);
        DialogueData = dialogueData;

        _dialogueIndex = 0;
        ReadText();
        ShowName();
        PlayMusic();
        ShowPotrait();
        ActiveSpeaker();
    }
    public void OnSkipText()
    {
        //if (gameState != GameState.Cutscene) return;
        if (!_skippable) return;
        if (_textRevealed)
        {
            AudioCollection.Play_OneShot("Skip Tap");
            CheckIndex();
            SkipTextEvent.Raise();
            ConfirmIcon.gameObject.SetActive(false);
            _textRevealed = false;
            //Next Line
            return;
        }
        _speed = 500;
        ConfirmIcon.gameObject.SetActive(true);
        _skipping = true;

        //SpeechText.maxVisibleCharacters = SpeechText.text.Length;

        //_textRevealed = true;
        //StopAllCoroutines();
    }
    private void OnSkipText(InputAction.CallbackContext context)
    {
        OnSkipText();
    }

    public void AutoSkip()
    {
        CheckIndex();
        ConfirmIcon.gameObject.SetActive(false);
        _textRevealed = false;
        //Next Line

    }
    public void ForceSkipAll(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SkipUI.SetActive(true);
            StartCoroutine(SkipTimer());
            return;
        }
        IEnumerator SkipTimer()
        {
            AudioCollection.Play("Skip Hold");
            SkipSlider.value = 0;
            float speed;
            float paramValue;
            float time = 0f;
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            while (context.phase.IsInProgress() && SkipSlider.value <= SkipSlider.maxValue)
            {

                SkipSlider.value += Time.deltaTime;
                time += Time.deltaTime;
                speed = curve.Evaluate(time / SkipSlider.maxValue);
                paramValue = Mathf.Lerp(0.5f,1, speed);
                AudioCollection.SetEventParameter("Skip Hold", "Volume", paramValue);
                yield return null;
            }
            SkipUI.SetActive(false);
        }
        if (context.canceled)
        {
            SkipUI.SetActive(false);
            AudioCollection.StopInstance("Skip Hold");
            return;
        }
        AudioCollection.StopInstance("Skip Hold");
        AudioCollection.Play_OneShot("Skip All");
        SkipAllEvent.Raise();
        SkipUI.SetActive(false);
        SkipSlider.value = 0;
        _dialogueIndex = DialogueData.dialogue.Count - 1;
        ShowPotrait();
        ReadText();
        var data = DialogueData.dialogue;
        for (int i = DialogueData.dialogue.Count -1; i >= 0; i--)
        {
            if (data[i].Music == "") continue;
            if (data[i].Music == "Stop")
            {
                AudioManager.Instance.MusicCollection.StopAllInstance();
                break; 
            }
            AudioManager.Instance.MusicCollection.Play(data[i].Music);   
            
        }
        ShowName();
        ActiveSpeaker();
        _speed = 100;
        ConfirmIcon.gameObject.SetActive(true);
        SpeechText.maxVisibleCharacters = SpeechText.text.Length;
        _textRevealed = true;
        _skippable = true;

        StopAllCoroutines();
    }
}