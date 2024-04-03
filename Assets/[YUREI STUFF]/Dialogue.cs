using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    [field: Header("Left Talker")]
    [field: SerializeField]public CharacterID CharacterLeft { get; private set; }
    [field: SerializeField] public ExpressionID InitialExpressionLeft { get; private set; }

    [field: Header("Right Talker")]
    [field: SerializeField] public CharacterID CharacterRight { get; private set; }
    [field: SerializeField] public ExpressionID InitialExpressionRight { get; private set; }

    [field: Header("Active Talker")]
    [field: SerializeField] public ActiveTalker ActiveSpeaker { get; private set; }

    [field: SerializeField][field: Space(15)] public string SpeakerName { get; private set; }
    [field: SerializeField][field: TextArea(3, 15)] public string SpeechText { get; private set; }



    public void SetDialogue(CharacterID charLeft, ExpressionID expressionLeft, CharacterID charRight, ExpressionID expressionRight,ActiveTalker activeSpeaker, string name, string speech)
    {
        CharacterLeft = charLeft; 
        CharacterRight = charRight;
        InitialExpressionLeft = expressionLeft;
        InitialExpressionRight = expressionRight;
        ActiveSpeaker = activeSpeaker;
        SpeakerName = name;
        SpeechText = speech;
    }
}


public static class ResourcePath
{
    public static string GetSpritePath(CharacterID character, ExpressionID expression)
    {
        if (character == CharacterID.None)
        {
            string defaultPath = $"Sprite/Potrait/Default/Default";
            return defaultPath;
        }
        string path = $"Sprite/Potrait/{character.ToString()}/{character.ToString()}_{expression.ToString()}";
        return path;
    }
    public static Sprite GetSprite(string path)
    {
        var sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            return sprite;
        }
        Debug.LogWarning("SPRITE NOT FOUND");
        return null;
    }
}

