using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueUIController : MonoBehaviour
{
    public GameState gameState; //CHANGE TO SOMETHING LIKE SINGLETON
    [field: SerializeField]public SO_Story_Dialogue DialogueData { get;private set; }
    [field: SerializeField]public GameObject DialogueCanvas { get; private set; }

    [field: SerializeField]public Image LeftCharacter { get; private set; }
    [field: SerializeField]public Image RightCharacter { get; private set; }
    [field: SerializeField]public Image ConfirmIcon { get; private set; }

    [field: SerializeField]public TMPro.TextMeshProUGUI SpeechText { get; private set; }
    [field: SerializeField]public TMPro.TextMeshProUGUI NameText { get; private set; }

    private float speed = 20;
    private string tmText;
    [SerializeField]private bool skippable;
    [SerializeField]private bool textRevealed;
    [SerializeField]private int dialogueIndex = 0;


    public void ReadText()
    {
        textRevealed = false;
        skippable = false;
        tmText = "";
        string[] subText = DialogueData.dialogue[dialogueIndex].SpeechText.Split('<', '>');
#if UNITY_EDITOR //Foreach Debug
        foreach (string sub in subText)
        {
            Debug.Log($"<color=yellow>{sub}</color> , <color=magenta>{sub.Length}</color>");
        }
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
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") || tag.StartsWith("concurrent");
        }
        SpeechText.text = tmText;
        SpeechText.maxVisibleCharacters = 0;
        StartCoroutine(Read());
   
        IEnumerator Read()
        {
            skippable = true;
            int subCounter = 0;
            int visibleCounter = 0;
            while (subCounter < subText.Length)
            {
                // if 
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
                yield return null;
            }
            skippable = true;
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
                        ShowPotrait((ExpressionID)Enum.Parse(typeof(ExpressionID), tag.Split('=')[1]));
                        return null;
                    }
                    else if (tag.StartsWith("concurrent"))
                    {
                        return null;
                    }
                }
                return null;
            }
       
        }
        
    }
    private void CutsceneEnd()
    {
        gameState = GameState.Gameplay;
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
            CutsceneEnd();
            ResetProperty();
            //Invoke Cutscene End, change Gamestate
            return;

        }
        dialogueIndex++;
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
    private void ShowPotrait(ExpressionID expressionLeft, ExpressionID expressionRight)
    {
        var data = DialogueData.dialogue[dialogueIndex];
        var pathLeft = ResourcePath.GetSpritePath(data.CharacterLeft, expressionLeft);
        var pathRight = ResourcePath.GetSpritePath(data.CharacterRight, expressionRight);
        var defaultPath = ResourcePath.GetSpritePath(CharacterID.None, ExpressionID.Default);

        switch (data.ActiveSpeaker)
        {
            case ActiveTalker.None:
                LeftCharacter.overrideSprite = ResourcePath.GetSprite(defaultPath);
                RightCharacter.overrideSprite = ResourcePath.GetSprite(defaultPath);
                break;
            case ActiveTalker.Left:
                LeftCharacter.overrideSprite = ResourcePath.GetSprite(pathLeft);
                break;
            case ActiveTalker.Right:
                RightCharacter.overrideSprite = ResourcePath.GetSprite(pathRight);
                break;
            case ActiveTalker.Both:
                LeftCharacter.overrideSprite = ResourcePath.GetSprite(pathLeft);
                RightCharacter.overrideSprite = ResourcePath.GetSprite(pathRight);
                break;
        }
    }
    private void ActiveSpeaker()
    {
        var data = DialogueData.dialogue[dialogueIndex];
        switch(data.ActiveSpeaker)
        {
            case ActiveTalker.None: 
                break;
            case ActiveTalker.Left:
                break;
            case ActiveTalker.Right:
                break;
            case ActiveTalker.Both:
                break;
        }
    }

    public void OnCutsceneStart(ScriptableObject data)
    {
        var dialogueData = data as SO_Story_Dialogue;
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

            //Next Line
            return;
        }
        speed = 100;
        ConfirmIcon.gameObject.SetActive(true);
        SpeechText.maxVisibleCharacters = SpeechText.text.Length;
        textRevealed = true;
    }
}