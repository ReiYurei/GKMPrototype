using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public CharacterID _characterID;
    public ExpressionID _expressionID;
    [Space(15)]

    public string _speakerName;
    [TextArea(3, 10)] public string _speechText;
    

    public Dialogue(string speakerName, string speechText, CharacterID characterID, ExpressionID expressionID)
    {
        _speakerName = speakerName;
        _speechText = speechText;
        _characterID = characterID;
        _expressionID = expressionID;
    }
}


public static class ResourcePath
{
    public static string GetSpritePath(CharacterID character, ExpressionID expression)
    {
        string path = $"Sprite/Potrait/{character.ToString()}/{character.ToString()}_{expression.ToString()}";
        return path;
    }
    public static Sprite GetSprite(string path)
    {
        var sprite = Resources.Load<Sprite>(path);
        return sprite;
    }
}

