using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class DialogueUIController : MonoBehaviour
{
    public GameState gameState; //CHANGE TO SOMETHING LIKE SINGLETON
    [field: SerializeField]public SO_StoryDialogue DialogueData { get;private set; }
    [field: SerializeField]public GameObject DialogueCanvas { get; private set; }

    [field: SerializeField]public Image LeftCharacter { get; private set; }
    [field: SerializeField]public Image RightCharacter { get; private set; }
    [field: SerializeField]public Image ConfirmIcon { get; private set; }

    [field: SerializeField]public TMPro.TextMeshProUGUI SpeechText { get; private set; }
    [field: SerializeField]public TMPro.TextMeshProUGUI NameText { get; private set; }
    [field: SerializeField]public SO_VoidGameEvent DialogueEndEvent { get; private set; }
    [field: SerializeField] public float TextSpeed { get; private set; }

    private float speed = 20;
    private string tmText;
    [SerializeField]private bool skippable;
    [SerializeField]private bool textRevealed;
    [SerializeField]private int dialogueIndex = 0;


    public void ReadText()
    {
        speed = TextSpeed;
        tmText = "";
        var data = DialogueData.dialogue[dialogueIndex];
        string[] subText = data.SpeechText.Split('<', '>');
#if UNITY_EDITOR //Foreach Debug
       // foreach (string sub in subText)
       // {
       //     Debug.Log($"<color=yellow>{sub}</color> , <color=magenta>{sub.Length}</color>");
       // }
#endif
        
        for (int i = 0; i < subText.Length; i++)
        {
            if (i % 2 == 0)
            {
                tmText += subText[i];
            }
            else if (!isCustomTag(subText[i].Replace(" ", "")))
            {
                tmText += $"<{subText[i]}>";
            }
        }
        bool isCustomTag(string tag)
        {
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") || tag.StartsWith("concurrent") 
                || tag.StartsWith("shake") || tag.StartsWith("event=");
        }
        SpeechText.text = tmText;
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
                        yield return new WaitForSeconds(1f / speed);
                        
                    }
                    visibleCounter = 0;
                }
                subCounter++;
                if (!data.AutoSkipAtEnd)
                {
                    skippable = true;
                }
                yield return null;

            }
            if (data.AutoSkipAtEnd)
            {

                textRevealed = true;
                ForceSkipText();
                yield break;

            }
            textRevealed = true;
            ConfirmIcon.gameObject.SetActive(true);


            //Text End
            WaitForSeconds EvaluateTag(string tag)
            {
                if (tag.Length > 0)
                {
                    if (tag.StartsWith("speed="))
                    {
                        speed = float.Parse(tag.Split('=')[1]);
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
        gameState = GameState.Gameplay;
        DialogueEndEvent.Raise();
        DialogueCanvas.SetActive(false);
    }
    private void ResetProperty()
    {
        dialogueIndex = 0;
    }
    private void CheckIndex()
    {
        if (dialogueIndex >= DialogueData.dialogue.Count - 1)
        {
            DialogueEnd();
            ResetProperty();
            //Invoke Cutscene End, change Gamestate
            return;

        }
        dialogueIndex++;
        ShowPotrait();
        ReadText();
        ShowName();
        ActiveSpeaker();
    }
    private void ShowName()
    {
        var data = DialogueData.dialogue[dialogueIndex];
        NameText.text = data.SpeakerName;
        
    }
    private void ShowPotrait()
    {
        var data = DialogueData.dialogue[dialogueIndex];
        var pathLeft = ResourcePath.GetSpritePath(data.CharacterLeft, data.InitialExpressionLeft);
        var pathRight = ResourcePath.GetSpritePath(data.CharacterRight, data.InitialExpressionRight);

        LeftCharacter.sprite = ResourcePath.GetSprite(pathLeft);
        RightCharacter.sprite = ResourcePath.GetSprite(pathRight);

        LeftCharacter.preserveAspect = true;
        RightCharacter.preserveAspect = true;
    }
    private void ShowPotrait(ExpressionID expression)
    {
        var data = DialogueData.dialogue[dialogueIndex];
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
        var data = DialogueData.dialogue[dialogueIndex];
        var gray = new Color(0.5f,0.5f,0.5f,1f);
        var white = Color.white;
        ///switch(data.activeTalker)
        ///{
        ///    case ActiveTalker.None:
        ///        while (LeftCharacter.color == gray || RightCharacter.color == gray)
        ///        {
        ///            LeftCharacter.color = Vector4.Lerp(LeftCharacter.color, gray, lerpValue * Time.deltaTime);
        ///            RightCharacter.color = Vector4.Lerp(RightCharacter.color, gray, lerpValue * Time.deltaTime);
        ///            yield return null;
        ///        }
        ///        yield break;
        ///    case ActiveTalker.Left:
        ///        while (LeftCharacter.color == white || RightCharacter.color == gray)
        ///        {
        ///            LeftCharacter.color = Vector4.Lerp(LeftCharacter.color, white, lerpValue * Time.deltaTime);
        ///            RightCharacter.color = Vector4.Lerp(RightCharacter.color, gray, lerpValue * Time.deltaTime);
        ///            yield return null;
        ///        }
        ///
        ///        yield break;
        ///    case ActiveTalker.Right:
        ///        while (LeftCharacter.color == gray || RightCharacter.color == white)
        ///        {
        ///            LeftCharacter.color = Vector4.Lerp(LeftCharacter.color, gray, lerpValue * Time.deltaTime);
        ///            RightCharacter.color = Vector4.Lerp(RightCharacter.color, white, lerpValue * Time.deltaTime);
        ///            yield return null;
        ///        }
        ///
        ///        yield break;
        ///    case ActiveTalker.Both:
        ///        while (LeftCharacter.color == white || RightCharacter.color == white)
        ///        {
        ///            LeftCharacter.color = Vector4.Lerp(LeftCharacter.color, white, lerpValue * Time.deltaTime);
        ///            RightCharacter.color = Vector4.Lerp(RightCharacter.color, white, lerpValue * Time.deltaTime);
        ///            yield return null;
        ///        }
        ///
        ///        yield break;
        ///}
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
        var dialogueData = data as SO_StoryDialogue;
        gameState = GameState.Cutscene;//For Debugging
        DialogueCanvas.SetActive(true);
        ConfirmIcon.gameObject.SetActive(false);
        DialogueData = dialogueData;
        dialogueIndex = 0;
        ReadText();
        ShowName();
        ShowPotrait();
        ActiveSpeaker();
    }
    public void OnSkipText()
    {
        if (gameState != GameState.Cutscene) return;
        if (!skippable) return;
        if (textRevealed)
        {
            CheckIndex();
            ConfirmIcon.gameObject.SetActive(false);
            skippable = false;
            textRevealed = false;
            //Next Line
            return;
        }
        speed = 100;
        ConfirmIcon.gameObject.SetActive(true);
        SpeechText.maxVisibleCharacters = SpeechText.text.Length;
        textRevealed = true;
        StopAllCoroutines();
    }
    public void ForceSkipText()
    {
        CheckIndex();
        ConfirmIcon.gameObject.SetActive(false);
        skippable = false;
        textRevealed = false;
        //Next Line

    }
}