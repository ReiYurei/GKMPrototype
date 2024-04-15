using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class DialogueUIController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
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
    [field: SerializeField] public SO_VoidGameEvent DialogueEndEvent { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }


    [field: Header("Other")]
    [SerializeField] private InputActionAsset _input;
    [SerializeField] private CutsceneState _cutsceneState;
    [SerializeField] private HubState _hubState;
    [SerializeField] private ExterminateState _exterminateState;




    private float _speed = 20;
    private string _tmText;
    [SerializeField]private bool _skippable;
    [SerializeField]private bool _textRevealed;
    private int _dialogueIndex = 0;
    public void ReadText()
    {
        _speed = TextSpeed;
        _tmText = "";
        var data = DialogueData.dialogue[_dialogueIndex];
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
                        yield return new WaitForSeconds(1f / _speed);
                        
                    }
                    visibleCounter = 0;
                }
                subCounter++;
                if (!data.AutoSkipAtEnd)
                {
                    _skippable = true;
                }
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
                        //Invoke Camera Shake Event
                        return null;
                    }
                    else if (tag.StartsWith("event="))
                    {
                        var lowercase = tag.Split('=')[1].Substring(1, tag.Split('=')[1].Length - 1).ToLower(); // return appy
                        var uppercase = tag.Split('=')[1].Substring(0, 1).ToUpper(); //return H
                        var value = uppercase + lowercase; //H+appy
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
            case EndEventBehaviour.DefaultEvent:
                DialogueEndEvent.Raise();
                ChangeStateEvent.Raise(_hubState);
                break;
            case EndEventBehaviour.CustomEvent:
                DialogueData.CustomEndEvent.Raise();
                break;
            case EndEventBehaviour.None_ToExterminate:
                ChangeStateEvent.Raise(_exterminateState);
                break;
            case EndEventBehaviour.None_ToHub:
                ChangeStateEvent.Raise(_hubState);
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
        ActiveSpeaker();
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
        ShowPotrait();
        ActiveSpeaker();
    }
    public void OnSkipText()
    {
        //if (gameState != GameState.Cutscene) return;
        if (!_skippable) return;
        if (_textRevealed)
        {
            CheckIndex();
            ConfirmIcon.gameObject.SetActive(false);
            _skippable = false;
            _textRevealed = false;
            //Next Line
            return;
        }
        _speed = 100;
        ConfirmIcon.gameObject.SetActive(true);
        SpeechText.maxVisibleCharacters = SpeechText.text.Length;
        _textRevealed = true;
        StopAllCoroutines();
    }
    private void OnSkipText(InputAction.CallbackContext context)
    {
        if (!_skippable) return;
        if (_textRevealed)
        {
            CheckIndex();
            ConfirmIcon.gameObject.SetActive(false);
            _skippable = false;
            _textRevealed = false;
            //Next Line
            return;
        }
        _speed = 100;
        ConfirmIcon.gameObject.SetActive(true);
        SpeechText.maxVisibleCharacters = SpeechText.text.Length;
        _textRevealed = true;
        StopAllCoroutines();
    }

    public void AutoSkip()
    {
        CheckIndex();
        ConfirmIcon.gameObject.SetActive(false);
        _skippable = false;
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
            SkipSlider.value = 0;
            while (context.phase.IsInProgress() && SkipSlider.value <= SkipSlider.maxValue)
            {
                SkipSlider.value += Time.deltaTime;
                yield return null;
            }
            SkipUI.SetActive(false);
        }
        if (context.canceled)
        {
            SkipUI.SetActive(false);
            return;
        }
        SkipUI.SetActive(false);
        SkipSlider.value = 0;
        _dialogueIndex = DialogueData.dialogue.Count - 1;
        ShowPotrait();
        ReadText();
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