using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using static Codice.Client.Common.Connection.AskCredentialsToUser;



#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class DialogueEditor : EditorWindow
{
    Texture2D headerDebugTex;
    Texture2D previewDebugTex;
    Texture2D missingTex;
    Texture2D nameDebugTex;
    Texture2D charInactiveTex;
    Texture2D charPreviewTex;
    Texture2D speechTextDebugTex;


    Texture2D charLeftTex;
    Sprite charLeftSprite;
    Rect charLeftRect;
    Texture2D charRightTex;
    Sprite charRightSprite;
    Rect charRightRect;

    Rect headerSection;
    Rect previewSection;
    Rect nameSection;
    Rect charLeftSection;
    Rect charRightSection;
    Rect expressionLeftSection;
    Rect expressionRightSection;
    Rect speechTextSection;

    Rect editorOffset;
    Rect editorCharacterLeftSection;
    Rect editorCharacterRightSection;
    Rect editorNameSection;
    Rect editorSpeechTextSection;
    Rect editorActiveTalkerSection;
    Rect editorEventSection;

    Rect buttonOffset;
    Rect buttonNextSection;
    Rect buttonPrevSection;
    Rect pageAreaSection;

    Rect configButtonSection;
    
    Color headerColor = Color.blue;
    Color previewColor = new(20/255f,50f/255f,20f/255f,1f);
    Color nameColor =           new(255 / 255f, 255 / 255f, 150f / 255f, 1f);
    Color characterMissingColor =  new(255 / 255f, 0 / 255f, 255 / 255f, 1f);
    Color characterInactiveColor = new(25 / 255f, 25 / 255f, 25 / 255f, 0.5f);
    Color speechTextColor =     new(150f / 255f, 100f / 255f, 10f / 255f, 1f);

    CharacterID characterLeft = CharacterID.None;
    ExpressionID expressionLeft = ExpressionID.Neutral;
    
    CharacterID characterRight = CharacterID.None;
    ExpressionID expressionRight = ExpressionID.Neutral;

    ActiveTalker activeTalker = ActiveTalker.Left;

    string keyVoid;
    SO_VoidGameEvent voidEvent;

    string keyParam;
    SO_ParameterGameEvent paramEvent;
    ScriptableObject paramData;
    VoidGameEventWithKey<string> voidEventKey;
    ParameterGameEventWithKey<string> paramEventKey;
    List<Dialogue> localDialogues;
    string eventName;
    string speakerName;
    string speechText;
    string music;
    float pitch;
    bool autoSkipAtEnd;
    EndEventBehaviour endEventBehaviour;
    SO_VoidGameEvent endEvent;

    string tmText;
    float speed;
    string previewText;

    int subCounter;
    int visibleCounter;

    static bool autoLoadSO = false;
    static bool autoSaveToLocal = true;
    int index = 0;

    bool isPlaying = false;
    string path;


    TextMeshProUGUI tmp;
    SO_Dialogue dialogueData;

    [MenuItem("Tools/Dialogue Editor")]
    static void OpenWindow()
    {
        DialogueEditor window = (DialogueEditor)GetWindow(typeof(DialogueEditor));
        window.minSize = new Vector2(650, 900);
        window.Show();
    }
    private void OnEnable()
    {
        index = 0;
        localDialogues = new(1);
        InitTexture();
        InitProperty();
    }
    private void OnDisable()
    {
        ClearAll();
    }
    private void OnGUI()
    {
        InitTextureFromSprite();      
        DrawLayout();
        EditorLayout();
        EventLayout();
        ButtonLayout();
        ConfigButtonLayout();
        AutoloadSO();
    }

    private void InitTexture()
    {
        headerDebugTex = new Texture2D(1, 1);
        headerDebugTex.SetPixel(0, 0, headerColor);
        headerDebugTex.Apply();

        previewDebugTex = new Texture2D(1, 1);
        previewDebugTex.SetPixel(0, 0, previewColor);
        previewDebugTex.Apply();

        nameDebugTex = new Texture2D(1, 1);
        nameDebugTex.SetPixel(0, 0, nameColor);
        nameDebugTex.Apply();

        charInactiveTex = new Texture2D(1, 1);
        charInactiveTex.SetPixel(0, 0, characterInactiveColor);
        charInactiveTex.Apply();

        charPreviewTex = new Texture2D(1, 1);
        charPreviewTex.SetPixel(0, 0, previewColor);
        charPreviewTex.Apply();

        missingTex = new Texture2D(1, 1);
        missingTex.SetPixel(0, 0, characterMissingColor);
        missingTex.Apply();

        speechTextDebugTex = new Texture2D(1, 1);
        speechTextDebugTex.SetPixel(0, 0, speechTextColor);
        speechTextDebugTex.Apply();
    }
    private void InitProperty()
    {
        voidEventKey = new VoidGameEventWithKey<string>();
        paramEventKey = new ParameterGameEventWithKey<string>();

        
    }
    private void DrawLayout()
    {
        HeaderLayout();
        PreviewLayout();

    }
    private void EditorLayout()
    {
        editorOffset.x = 0;
        editorOffset.y = previewSection.height + previewSection.y;
        editorOffset.width = Screen.width;
        editorOffset.height = Screen.height - (previewSection.y + headerSection.y);
        //GUI.DrawTexture(editorOffset, headerDebugTex); //Remove after done
        GUILayout.BeginArea(editorOffset);
      
        EditorCharacterLayout();
        EditorSpeechTextLayout();
        EditorSpeakerNameLayout();
        EditorActiveTalkerLayout();
        GUILayout.EndArea();
    }
    private void EventLayout()
    {
        editorEventSection.x = 0;
        editorEventSection.y = 595;
        editorEventSection.width = Screen.width;
        editorEventSection.height = 45;

        var customStyle2 = new GUIStyle(EditorStyles.label);
        customStyle2.alignment = TextAnchor.UpperLeft;

        GUILayoutOption[] customLayout = new GUILayoutOption[]
        {
            GUILayout.Width(35),
        };
        GUILayoutOption[] customLayout2 = new GUILayoutOption[]
        {
              GUILayout.Width(50),
        };
        GUILayoutOption[] customLayout3 = new GUILayoutOption[]
     {
              GUILayout.Width(150),
     };
        GUILayout.BeginArea(editorEventSection);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Void Game Event", customStyle2);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key", customLayout);
        keyVoid = EditorGUILayout.TextField(keyVoid, customLayout2);
        voidEvent = (SO_VoidGameEvent)EditorGUILayout.ObjectField(voidEvent, typeof(SO_VoidGameEvent), true, customLayout3);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Param Game Event", customStyle2);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key", customLayout);
        keyParam = EditorGUILayout.TextField(keyParam, customLayout2);
        paramEvent = (SO_ParameterGameEvent)EditorGUILayout.ObjectField(paramEvent, typeof(SO_ParameterGameEvent), true, customLayout2);
        paramData = (ScriptableObject)EditorGUILayout.ObjectField(paramData, typeof(ScriptableObject), true, customLayout3);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void ButtonLayout()
    {
        buttonOffset.x = 0;
        buttonOffset.y = editorEventSection.y + editorEventSection.height;
        buttonOffset.width = Screen.width;
        buttonOffset.height = 50;
        //GUI.DrawTexture(buttonOffset, headerDebugTex);
        GUILayout.BeginArea(buttonOffset);
        NavigationButtonLayout();
        PageViewLayout();
        GUILayout.EndArea();


    }
    private void ConfigButtonLayout()
    {
        configButtonSection.x = 0;
        configButtonSection.y = buttonOffset.y + buttonOffset.height;
        configButtonSection.width = Screen.width;
        configButtonSection.height = 265;


        GUILayout.BeginArea(configButtonSection);

 
        //tmp = (TextMeshProUGUI)EditorGUILayout.ObjectField(tmp, typeof(TextMeshProUGUI), true);
        dialogueData = (SO_Dialogue)EditorGUILayout.ObjectField(dialogueData, typeof(SO_Dialogue), true);
        eventName = EditorGUILayout.TextField("Event Name", eventName);
        path = EditorGUILayout.TextField("Objects Path", path);

        GUILayout.BeginHorizontal();
        autoSaveToLocal = EditorGUILayout.Toggle("Auto-Save Local data", autoSaveToLocal);
        //autoLoadSO = EditorGUILayout.Toggle("Auto-Load SO data", autoLoadSO);
        GUILayout.EndHorizontal();

        //if (GUILayout.Button("Read Dialogue")) { Read(); GUI.FocusControl(null); }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Changes to SO")) { SaveEditToSO(); GUI.FocusControl(null); }
        if (GUILayout.Button("Save Changes to Local")) { SaveLocalEdit(); GUI.FocusControl(null); }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Load Data from SO")) { LoadData(); GUI.FocusControl(null); }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dialogue")) { AddDialogue(); GUI.FocusControl(null); }
        if (GUILayout.Button("Insert Dialogue")) { InsertPage(); GUI.FocusControl(null); }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Remove Dialogue")) { RemovePage(); GUI.FocusControl(null); }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Asset")) { CreateNew(); GUI.FocusControl(null); }
        if (GUILayout.Button("Refresh Asset Path")) {Refresh(); GUI.FocusControl(null); }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Clear All")) { ClearAll(); GUI.FocusControl(null); }


        GUILayout.EndArea();


    }
    private void ClearAll()
    {
        characterLeft = CharacterID.None; 
        characterRight = CharacterID.None;
        expressionLeft = ExpressionID.Neutral;
        expressionRight = ExpressionID.Neutral;
        activeTalker = ActiveTalker.Left;
        dialogueData = null;
        eventName = "";
        speechText = "";
        pitch = 0.5f;
        music = "";
        if (localDialogues.Count <= 0) return;
        for (int i = localDialogues.Count - 1; i > 0 ; i--)
        {
            localDialogues.RemoveAt(i);
        }
        index = 0;
    }
    private void Read()
    {
        if (tmp == null) {Debug.LogWarning("TextMeshPro Is null"); return; }
        isPlaying = true;
        previewText = "";
        tmText = "";
        string[] subText = speechText.Split('<', '>');
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
        tmp.text = tmText;
        tmp.maxVisibleCharacters = 0;
        this.StartCoroutine(Read());
        IEnumerator Read()
        {
            Debug.Log("Read");
            subCounter = 0;
            visibleCounter = 0;
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
                        tmp.maxVisibleCharacters++;
                        Debug.Log(tmp.maxVisibleCharacters);
                        yield return new EditorWaitForSeconds(1f / speed);                     
                    }
                    visibleCounter = 0;
                }
                subCounter++;
                yield return null;
                isPlaying = false;
                previewText = tmText;
                Debug.Log("Done");
            }

            EditorWaitForSeconds EvaluateTag(string tag)
            {
                if (tag.Length > 0)
                {
                    if (tag.StartsWith("speed="))
                    {
                        speed = float.Parse(tag.Split('=')[1]);
                    }
                    else if (tag.StartsWith("pause="))
                    {
                        return new EditorWaitForSeconds(float.Parse(tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("emotion="))
                    {
                        DrawSprite((ExpressionID)Enum.Parse(typeof(ExpressionID), tag.Split('=')[1]));
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
    private string TrimTag()
    {
        if (isPlaying == true) return null;
        tmText = "";
        if (speechText == null) return null;
        string[] subText = speechText.Split('<', '>');

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
            return tag.StartsWith("speed=") || tag.StartsWith("pause=") || tag.StartsWith("emotion=") 
                || tag.StartsWith("event=");
        }
        return tmText;
    }
    private void InitTextureFromSprite()
    {
        if (isPlaying) return;
        var charLeftPath = ResourcePath.GetSpritePath(characterLeft, expressionLeft);
        charLeftSprite = ResourcePath.GetSprite(charLeftPath);
        if(charLeftSprite != null)
        {
            Rect spriteRect = charLeftSprite.rect;
            Texture2D tex = charLeftSprite.texture;
            charLeftTex = tex;
            charLeftRect.x = spriteRect.x / tex.width;
            charLeftRect.y = spriteRect.y / tex.height;
            charLeftRect.width = spriteRect.width / tex.width;
            charLeftRect.height = spriteRect.height / tex.height;
        }
        var charRightPath = ResourcePath.GetSpritePath(characterRight, expressionRight);
        charRightSprite = ResourcePath.GetSprite(charRightPath);
        if (charRightSprite != null)
        {
            Rect spriteRect = charRightSprite.rect;
            Texture2D tex = charRightSprite.texture;
            charRightTex = tex;
            charRightRect.x = spriteRect.x / tex.width;
            charRightRect.y = spriteRect.y / tex.height;
            charRightRect.width = spriteRect.width / tex.width;
            charRightRect.height = spriteRect.height / tex.height;

        }

    }
    private void DrawSprite(ExpressionID expression)
    {
        var pathLeft = ResourcePath.GetSpritePath(characterLeft, expression);
        var pathRight = ResourcePath.GetSpritePath(characterRight, expression);
        var defaultPath = ResourcePath.GetSpritePath(CharacterID.None, ExpressionID.Default);

        switch (activeTalker)
        {
            case ActiveTalker.None:
                charLeftSprite = ResourcePath.GetSprite(defaultPath);
                charRightSprite = ResourcePath.GetSprite(defaultPath);
                break;
            case ActiveTalker.Left:
                charLeftSprite = ResourcePath.GetSprite(pathLeft);
                break;
            case ActiveTalker.Right:
                charRightSprite = ResourcePath.GetSprite(pathRight);
                break;
            case ActiveTalker.Both:
                charLeftSprite = ResourcePath.GetSprite(pathLeft);
                charRightSprite = ResourcePath.GetSprite(pathRight);
                break;
        }

    }
    private void SaveLocalEdit()
    {
        if (localDialogues.Count <= 0) return;
      
        var data = localDialogues[index];       
        data.VoidGameEvent.SetValue(keyVoid, voidEvent);
        data.ParameterGameEvent.SetValue(keyParam, paramEvent, paramData);
        var info1 = characterLeft;
        var info2 = characterRight;
        var info3 = expressionLeft;
        var info4 = expressionRight;
        var info5 = speakerName;
        var info6 = speechText;
        var info7 = activeTalker;
        var info8 = autoSkipAtEnd;
        var info9 = voidEventKey;
        var info10 = paramEventKey;
        var info11 = pitch;
        var info12 = music;
        data.SetDialogue(info1, info3, info2, info4, info7, info5, info6, info8, info9, info10, info11, info12);
    }
    private void ShowData()
    {
        if (localDialogues.Count < 1) return;
        var data = localDialogues[index];
        characterLeft = data.CharacterLeft; 
        characterRight = data.CharacterRight;
        expressionLeft = data.InitialExpressionLeft; 
        expressionRight = data.InitialExpressionRight;
        activeTalker = data.ActiveSpeaker;
        speakerName = data.SpeakerName;
        speechText = data.SpeechText;
        autoSkipAtEnd = data.AutoSkipAtEnd;
        pitch = data.SpeechPitch;
        music = data.Music;

        voidEventKey = data.VoidGameEvent;
        keyVoid = voidEventKey.Key;
        voidEvent = voidEventKey.GameEvent;

        paramEventKey = data.ParameterGameEvent;
        keyParam = paramEventKey.Key;
        paramEvent = paramEventKey.GameEvent;
        paramData = paramEventKey.ParamData;

        InitTextureFromSprite();
        if (dialogueData == null) return;
        eventName = dialogueData.eventName;
    }
    private void AddDialogue()
    {
        if (autoSaveToLocal) SaveLocalEdit();
        localDialogues.Add(new Dialogue());
        voidEventKey = new();
        paramEventKey = new();
        index = localDialogues.Count-1;
        
        ShowData();
    }
    private void InsertPage()
    {
        if (autoSaveToLocal) SaveLocalEdit();
        localDialogues.Insert(index,new Dialogue());
        ShowData();
    }
    private void RemovePage() 
    {
        if(localDialogues.Count <= 0) return; 
        localDialogues.RemoveAt(index);
        index = 0;
        ShowData();
    }
    private void LoadData()
    {
        if (dialogueData == null) return;
        index = 0;
        var data = dialogueData.dialogue;
        localDialogues.Clear();
        for(int i = 0; i < data.Count; i++)
        {
            localDialogues.Add(new Dialogue());
        }
        for(int i = 0; i < localDialogues.Count; i++)
        {
            var info1 = data[i].CharacterLeft;
            var info2 = data[i].CharacterRight;
            var info3 = data[i].InitialExpressionLeft; 
            var info4 = data[i].InitialExpressionRight;
            var info5 = data[i].SpeakerName;
            var info6 = data[i].SpeechText;
            var info7 = data[i].ActiveSpeaker;
            var info8 = data[i].AutoSkipAtEnd;
            var info9 = data[i].VoidGameEvent;
            var info10 = data[i].ParameterGameEvent;
            var info11 = data[i].SpeechPitch;
            var info12 = data[i].Music;

            localDialogues[i].SetDialogue(info1, info3,info2,info4,info7,info5,info6, info8,info9,info10,info11,info12);
        }
        endEventBehaviour = dialogueData.endEventBehaviour;
        endEvent = dialogueData.CustomEndEvent;

        ShowData();

    }
    private void AutoloadSO()
    {
        if(!autoLoadSO) return;
        if(dialogueData != null) return;
        LoadData();
    }
    private void SaveEditToSO()
    {
        if (dialogueData == null) return;
        SaveLocalEdit();
        index = 0;
        var data = dialogueData.dialogue;
        dialogueData.eventName = eventName;
        if (data.Count < localDialogues.Count)
        {
            var gap = localDialogues.Count - data.Count;
            for (int i = 0; i < gap; i++)
            {
                data.Add(new Dialogue());
            }
        }
        else if(data.Count > localDialogues.Count)
        {
            var gap = data.Count - localDialogues.Count;
            for (int i = 0; i < gap; i++)
            {
                data.Remove(data[i]);
            }
        }

        for (int i = 0; i < localDialogues.Count;i++)
        {
            var info1 = localDialogues[i].CharacterLeft;
            var info2 = localDialogues[i].CharacterRight;
            var info3 = localDialogues[i].InitialExpressionLeft;
            var info4 = localDialogues[i].InitialExpressionRight;
            var info5 = localDialogues[i].SpeakerName;
            var info6 = localDialogues[i].SpeechText;
            var info7 = localDialogues[i].ActiveSpeaker;
            var info8 = localDialogues[i].AutoSkipAtEnd;
            var info9 = localDialogues[i].VoidGameEvent;
            var info10 = localDialogues[i].ParameterGameEvent;
            var info11 = localDialogues[i].SpeechPitch;
            var info12 = localDialogues[i].Music;

            data[i].SetDialogue(info1, info3, info2, info4, info7, info5, info6, info8, info9, info10, info11, info12);
        }
        dialogueData.CustomEndEvent = endEvent;
        dialogueData.endEventBehaviour = endEventBehaviour;
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ShowData();
        
    }
    private void CreateNew()
    {
        SaveLocalEdit();
        SO_Dialogue newObject = CreateInstance<SO_Dialogue>();
        newObject.eventName = new string(eventName);
        newObject.endEventBehaviour = endEventBehaviour;
        newObject.CustomEndEvent = endEvent;
        newObject.dialogue = new List<Dialogue>(localDialogues);
        AssetDatabase.CreateAsset(newObject, $"{path}/{eventName}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void Refresh()
    {
        path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        Debug.Log(dialogueData == null);

    }
    private void PageViewLayout()
    {
        pageAreaSection.x = (buttonOffset.width /2) -(pageAreaSection.width / 2);
        pageAreaSection.y = 0;
        pageAreaSection.width = 100;
        pageAreaSection.height = 35;
        var customStyle = new GUIStyle(EditorStyles.label);
        customStyle.richText = true;
        //GUI.DrawTexture(pageAreaSection, charPreviewTex);
        GUILayout.BeginArea(pageAreaSection);
        EditorGUILayout.LabelField("Page");
        if (localDialogues.Count <= 0)
        {
            //EditorGUILayout.LabelField($"<color=yellow>{tabIndex} / {localDialogues.Count}</color>", customStyle);
            localDialogues.Add(new Dialogue());
        }
        EditorGUILayout.LabelField($"{index + 1} / {localDialogues.Count}");

        GUILayout.EndArea();
    }
    private void NavigationButtonLayout()
    {
        //Prev
        buttonPrevSection.x = 15;
        buttonPrevSection.y = 0;
        buttonPrevSection.width = 150 - (buttonPrevSection.x * 2);
        buttonPrevSection.height = buttonOffset.height;
        //GUI.DrawTexture(buttonPrevSection, speechTextDebugTex);

        GUILayout.BeginArea(buttonPrevSection);
        var prevButton = GUILayout.Button("Prev");
        if (prevButton) 
        { 
            PrevPage();
            GUI.FocusControl(null);
        }
       
        GUILayout.EndArea();

        //Prev
        buttonNextSection.x = Screen.width - (buttonNextSection.width + buttonPrevSection.x);
        buttonNextSection.y = 0;
        buttonNextSection.width = buttonPrevSection.width;
        buttonNextSection.height = buttonPrevSection.height;
        //GUI.DrawTexture(buttonNextSection, speechTextDebugTex);

        GUILayout.BeginArea(buttonNextSection);
        var nextButton = GUILayout.Button("Next");
        if (nextButton)
        {
            NextPage();
            GUI.FocusControl(null);

        }

        GUILayout.EndArea();
    }
    private void PrevPage()
    {
        if (index <= 0)
        {
            if(localDialogues.Count <= 0)
            {
                if (autoSaveToLocal) SaveLocalEdit();
                index = 0;
                ShowData();
                return;
            }
            index = localDialogues.Count - 1;
            ShowData();
            return;
        }
        if (autoSaveToLocal) SaveLocalEdit();
        index--;
        ShowData();

    }
    private void NextPage()
    {
        if (index >= localDialogues.Count - 1)
        {
            if (autoSaveToLocal) SaveLocalEdit();
            index = 0;
            ShowData();
            return;
        }
        if (autoSaveToLocal) SaveLocalEdit();
        index++;
        ShowData();
    }
    private void EditorCharacterLayout()
    {
        editorCharacterLeftSection.x = 15;
        editorCharacterLeftSection.y = 5;
        editorCharacterLeftSection.width = 225 -(editorCharacterLeftSection.x * 2);
        editorCharacterLeftSection.height = 75;
        //GUI.DrawTexture(editorCharacterLeftSection, previewDebugTex);  //Remove after done

        editorCharacterRightSection.x = Screen.width - (editorCharacterRightSection.width + editorCharacterLeftSection.x);
        editorCharacterRightSection.y = editorCharacterLeftSection.y;
        editorCharacterRightSection.width = editorCharacterLeftSection.width;
        editorCharacterRightSection.height = editorCharacterLeftSection.height;
        //GUI.DrawTexture(editorCharacterRightSection, previewDebugTex);
        var customStyle = new GUIStyle(EditorStyles.label);
        customStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.BeginArea(editorCharacterLeftSection);
        EditorGUILayout.LabelField("Character", customStyle);
        characterLeft = (CharacterID)EditorGUILayout.EnumPopup(characterLeft);
        EditorExpressionLeftLayout();
        GUILayout.EndArea();


        GUILayout.BeginArea(editorCharacterRightSection);
        EditorGUILayout.LabelField("Character", customStyle);
        characterRight = (CharacterID)EditorGUILayout.EnumPopup(characterRight);
        EditorExpressionRightLayout();
        GUILayout.EndArea();
    }
    private void EditorExpressionLeftLayout()
    {
        expressionLeftSection.x = 0;
        expressionLeftSection.y = editorCharacterLeftSection.height /2;
        expressionLeftSection.width = editorCharacterLeftSection.width;
        expressionLeftSection.height = editorCharacterLeftSection.height / 2;
        //GUI.DrawTexture(expressionLeftSection, speechTextDebugTex);
        var customStyle = new GUIStyle(EditorStyles.label);
        customStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.BeginArea(expressionLeftSection);
        EditorGUILayout.LabelField("Initial Expression", customStyle);
        expressionLeft = (ExpressionID)EditorGUILayout.EnumPopup(expressionLeft);
        GUILayout.EndArea();


    }
    private void EditorExpressionRightLayout()
    {
        expressionRightSection.x = 0;
        expressionRightSection.y = editorCharacterRightSection.height /2;
        expressionRightSection.width = editorCharacterRightSection.width;
        expressionRightSection.height = editorCharacterRightSection.height/2;
        //GUI.DrawTexture(expressionRightSection, speechTextDebugTex);
        var customStyle = new GUIStyle(EditorStyles.label);
        customStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.BeginArea(expressionLeftSection);
        EditorGUILayout.LabelField("Initial Expression", customStyle);
        expressionRight = (ExpressionID)EditorGUILayout.EnumPopup( expressionRight);
        GUILayout.EndArea();

    }
    private void EditorSpeakerNameLayout()
    {
        editorNameSection.x = 10;
        editorNameSection.y = (editorCharacterLeftSection.y + editorCharacterLeftSection.height) + 5;
        editorNameSection.width = Screen.width - (editorNameSection.x * 2);
        editorNameSection.height = 40;
        //GUI.DrawTexture(editorNameSection, nameDebugTex); //Remove after done
        var customStyle = new GUIStyle(EditorStyles.textField);
        customStyle.wordWrap = true;
        GUILayoutOption[] layout =
        {
            GUILayout.Height(20),
            GUILayout.Width(150),
        };
        GUILayoutOption[] layout3 =
 {
            GUILayout.Height(20),
            GUILayout.Width(75),
        };
        GUILayoutOption[] layout2 =
        {
            GUILayout.Width(125),

        };
        GUILayout.BeginArea(editorNameSection);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Name", layout);
        speakerName = EditorGUILayout.TextField(speakerName, customStyle, layout);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Pitch", layout2);
        pitch = EditorGUILayout.FloatField(pitch, customStyle, layout3);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Music", layout2);
        music = EditorGUILayout.TextField(music, customStyle, layout3);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Auto-skip line", layout2);
        autoSkipAtEnd = EditorGUILayout.Toggle(autoSkipAtEnd, layout2);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("End Event Behaviour", layout2);
        endEventBehaviour = (EndEventBehaviour)EditorGUILayout.EnumPopup(endEventBehaviour, layout2);

        EditorGUILayout.EndVertical();
        switch (endEventBehaviour)
        {
            case EndEventBehaviour.None_ToHub:
                break;
            case EndEventBehaviour.None_ToExterminate:
                break;
            case EndEventBehaviour.CustomEvent:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Custom End Event", layout2);
                endEvent = (SO_VoidGameEvent)EditorGUILayout.ObjectField(endEvent, typeof(SO_VoidGameEvent), true, layout2);
                EditorGUILayout.EndVertical();
                break;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();



    }
    private void EditorSpeechTextLayout()
    {
        editorSpeechTextSection.x = 10;
        editorSpeechTextSection.y = (editorNameSection.y + editorNameSection.height);
        editorSpeechTextSection.width = Screen.width - (editorSpeechTextSection.x * 2);
        editorSpeechTextSection.height = 200;
        //GUI.DrawTexture(editorSpeechTextSection, speechTextDebugTex); //Remove after done
        GUILayout.BeginArea(editorSpeechTextSection);

        var customStyle = new GUIStyle(EditorStyles.textArea);
        customStyle.wordWrap = true;
        Rect rect = new(35, 25, Screen.width - 105, 60);
        EditorGUILayout.LabelField("Speech Text");
        speechText = EditorGUI.TextArea(rect,speechText, customStyle);
        previewText = TrimTag();

        GUILayout.EndArea();
    }
    private void EditorActiveTalkerLayout()
    {
        editorActiveTalkerSection.x = (editorOffset.width / 2) - (editorActiveTalkerSection.width / 2);
        editorActiveTalkerSection.y = 20;
        editorActiveTalkerSection.width = 115;
        editorActiveTalkerSection.height = 45;
        //GUI.DrawTexture(editorActiveTalkerSection, charPreviewTex);

        GUILayout.BeginArea(editorActiveTalkerSection);
        EditorGUILayout.LabelField("Active Talker");
        activeTalker = (ActiveTalker)EditorGUILayout.EnumPopup(activeTalker);
        GUILayout.EndArea();

    }
    private void HeaderLayout()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 30;
        //GUI.DrawTexture(headerSection, headerDebugTex);
        GUILayout.BeginArea(headerSection);
        var customStyleBold = new GUIStyle(EditorStyles.label);
        customStyleBold.alignment = TextAnchor.MiddleCenter;
        customStyleBold.fontStyle = FontStyle.Bold;
        customStyleBold.fixedWidth = headerSection.width;
        customStyleBold.fixedHeight = headerSection.height;
        GUILayout.Label("DIALOGUE EDITOR", customStyleBold);
        GUILayout.EndArea();
    }
    private void PreviewLayout()
    {
        previewSection.x = 0;
        previewSection.y = headerSection.height;
        previewSection.width = Screen.width;
        previewSection.height = 350;
        //GUI.DrawTexture(previewSection, previewDebugTex);
        CharacterLeftLayout();
        CharacterRightLayout();
        ActiveSpeaker();
        SpeechLayout();
        NameLayout();

        GUILayout.BeginArea(previewSection);
        //var customStyleBold =new GUIStyle(EditorStyles.label);
        //customStyleBold.alignment = TextAnchor.UpperCenter;
        //customStyleBold.fontStyle = FontStyle.Bold;
        //customStyleBold.fixedWidth = previewSection.width;
        //customStyleBold.fixedHeight = previewSection.height;
        Rect rect = new(previewSection.x, previewSection.y, previewSection.width, previewSection.height);
        //GUI.Label(rect, "PREVIEW AREA", customStyleBold);
        GUILayout.EndArea();
    }
    private void ActiveSpeaker()
    {
        Rect rectLeft = new Rect(15, previewSection.y, 300, 200);
        Rect rectRight = new Rect(Screen.width - 330, previewSection.y,300,200);
        var customStyle = new GUIStyle(EditorStyles.label);
        customStyle.alignment = TextAnchor.UpperRight;
        var customStyle2 = new GUIStyle(EditorStyles.label);
        customStyle2.alignment = TextAnchor.UpperLeft;
        switch (activeTalker)
        {
            case ActiveTalker.None:
                if (charLeftTex != null)
                {
                    GUI.DrawTexture(rectLeft, charInactiveTex);
                    EditorGUI.LabelField(rectLeft, "Inactive", customStyle2);

                }
                if (charRightTex != null)
                {
                    GUI.DrawTexture(rectRight, charInactiveTex);
                    EditorGUI.LabelField(rectRight, "Inactive", customStyle);
                }
                break;
            case ActiveTalker.Left:
                if (charLeftTex != null)
                {
                    EditorGUI.LabelField(rectLeft, "Active", customStyle2);
                }
                if (charRightTex != null)
                {
                    GUI.DrawTexture(rectRight, charInactiveTex);
                    EditorGUI.LabelField(rectRight, "Inactive", customStyle);

                }
                break;
            case ActiveTalker.Right:
                if (charLeftTex != null)
                {
                    GUI.DrawTexture(rectLeft, charInactiveTex);
                    EditorGUI.LabelField(rectLeft, "Inactive", customStyle2);

                }
                if (charRightTex != null)
                {
                    EditorGUI.LabelField(rectRight, "Active", customStyle);
                }
                break;
            case ActiveTalker.Both:
                if (charLeftTex != null)
                {
                    EditorGUI.LabelField(rectLeft, "Active", customStyle2);
                }
                if (charRightTex != null)
                {
                    EditorGUI.LabelField(rectRight, "Active", customStyle);
                }
                break;
        }
    }
    private void CharacterLeftLayout()
    {

        if (charLeftSprite != null)
        {
            charLeftSection.x = 15;
            charLeftSection.y = (previewSection.height + previewSection.y) - charLeftSection.height - speechTextSection.height + 10;
            charLeftSection.width = charLeftSprite.rect.width / 1.5f - (charLeftSection.x * 2);
            charLeftSection.height = charLeftSprite.rect.height / 1.5f;
            GUI.DrawTextureWithTexCoords(charLeftSection, charLeftTex, charLeftRect);            
        }
        else if(charLeftSprite == null)
        {
            charLeftSection.x = 30;
            charLeftSection.y = (previewSection.height + previewSection.y) - charLeftSection.height - speechTextSection.height + 10;
            charLeftSection.width = 300 - (charLeftSection.x * 2);
            charLeftSection.height = 150;
            GUI.DrawTexture(charLeftSection, missingTex);
        }
        GUILayout.BeginArea(charLeftSection);
        var customStyleBold =new GUIStyle();
        customStyleBold.alignment = TextAnchor.UpperCenter;
        customStyleBold.fontStyle = FontStyle.Bold;
        customStyleBold.fixedWidth = charLeftSection.width;
        customStyleBold.fixedHeight = charLeftSection.height;
        Rect rect = new(0, charLeftSection.height / 2f, charLeftSection.width, charLeftSection.height);
        //GUI.Label(rect, "CHARA LEFT", customStyleBold);
        GUILayout.EndArea();
    }
    private void CharacterRightLayout()
    {

        if (charRightSprite != null)
        {
            charRightSection.x = (Screen.width - (charRightSection.width + charLeftSection.x) * -1) - (charRightSection.width + charLeftSection.x);
            charRightSection.y = (previewSection.height + previewSection.y) - charRightSection.height - speechTextSection.height + 10;
            charRightSection.width = -charRightSprite.rect.width / 1.5f + (charLeftSection.x * 2);
            charRightSection.height = charRightSprite.rect.height / 1.5f;
            GUI.DrawTextureWithTexCoords(charRightSection, charRightTex, charRightRect);
        }
        else if(charRightSprite == null)
        {
            charRightSection.x = Screen.width - (charRightSection.width + 30);
            charRightSection.y = (previewSection.height + previewSection.y) - charRightSection.height - speechTextSection.height + 10;
            charRightSection.width = 300;
            charRightSection.height = 150;
            GUI.DrawTexture(charRightSection, missingTex);
        }
        GUILayout.BeginArea(charRightSection);

        var customStyleBold = new GUIStyle();
        customStyleBold.alignment = TextAnchor.UpperCenter;
        customStyleBold.fontStyle = FontStyle.Bold;
        customStyleBold.fixedWidth = charRightSection.width;
        customStyleBold.fixedHeight = charRightSection.height;
        Rect rect = new(0, charRightSection.height / 2f, charRightSection.width, charRightSection.height);
        //GUI.Label(rect, "CHARA RIGHT", customStyleBold);
        GUILayout.EndArea();
    }
    private void SpeechLayout()
    {
        speechTextSection.x = 15;
        speechTextSection.y = (previewSection.height + previewSection.y) - speechTextSection.height;
        speechTextSection.width = Screen.width - (speechTextSection.x * 2);
        speechTextSection.height = 150;
        GUI.DrawTexture(speechTextSection, speechTextDebugTex);
        GUILayout.BeginArea(speechTextSection);
        var customStyleBold = new GUIStyle();
        customStyleBold.alignment = TextAnchor.UpperLeft;
        customStyleBold.fontStyle = FontStyle.Bold;
        customStyleBold.richText = true;
        customStyleBold.fontSize = 20;
        customStyleBold.wordWrap = true;
        customStyleBold.fixedWidth = speechTextSection.width - 15 * 2;
        customStyleBold.fixedHeight = speechTextSection.height - 50;
        Rect rect = new(15, 30, speechTextSection.width - 15, speechTextSection.height - 50);
        if(tmp != null && isPlaying)
        {
            GUI.TextArea(rect, tmp.text, customStyleBold);
        
        }
        else
        {
            GUI.TextArea(rect, previewText, customStyleBold);
        }


        GUILayout.EndArea();
    }
    private void NameLayout()
    {
        nameSection.x = speechTextSection.x + 25;
        nameSection.y = ((previewSection.height + previewSection.y) - nameSection.height) - speechTextSection.height + nameSection.height / 2;
        nameSection.width = 125;
        nameSection.height = 50;
        GUI.DrawTexture(nameSection, nameDebugTex);

        GUILayout.BeginArea(nameSection);

        var customStyleBold = new GUIStyle();
        customStyleBold.alignment = TextAnchor.UpperCenter;
        customStyleBold.fontStyle = FontStyle.Bold;
        customStyleBold.richText = true;
        customStyleBold.normal.textColor = Color.black;
        customStyleBold.fontSize = 18;
        customStyleBold.wordWrap = true;
        customStyleBold.fixedWidth = nameSection.width;
        customStyleBold.fixedHeight = nameSection.height;
        Rect rect = new(0, nameSection.height / 4.5f, nameSection.width, nameSection.height);
        GUI.Label(rect, $"{speakerName}", customStyleBold);
        GUILayout.EndArea();

    }
}

public class DialogeMaker : EditorWindow
{
    SerializedProperty m_eventName;
    SerializedProperty m_dialogues;
    SerializedObject serializedObject;
    public string eventName;
    public string path;


    SO_Dialogue obj;

    private void OnEnable()
    {
        obj = ScriptableObject.CreateInstance<SO_Dialogue>();
        serializedObject = new SerializedObject(obj);
        m_dialogues = serializedObject.FindProperty("dialogue");
        m_eventName = serializedObject.FindProperty("eventName");


    }
    private void OnDisable()
    {
        serializedObject.Dispose();
    }
    [MenuItem("Tools/Obsolete/Dialogue Maker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogeMaker));
    }
    private void OnGUI()
    {
        GUILayout.Label("Dialogue Maker", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_eventName, new GUIContent("Event Name"));
        EditorGUILayout.PropertyField(m_dialogues, new GUIContent("Dialogues"));

        path = EditorGUILayout.TextField("Objects Path", path);
        serializedObject.ApplyModifiedProperties();


        if (GUILayout.Button("Create Dialogue"))
        {
            CreateObjects();
        }
        if (GUILayout.Button("Refresh Path"))
        {
            RefreshPath();
        }
        if (GUILayout.Button("Clear All"))
        {
            ClearAll();
        }
    }
    private void ClearAll()
    {
        m_eventName.stringValue = string.Empty;
        m_dialogues.ClearArray();
        serializedObject.ApplyModifiedProperties();
    }
    private void RefreshPath()
    {
        path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
    }
    private void CreateObjects()
    {
        SO_Dialogue newObject = ScriptableObject.CreateInstance<SO_Dialogue>();
        newObject.eventName = obj.eventName;
        newObject.dialogue = obj.dialogue;
        AssetDatabase.CreateAsset(newObject, $"{path}/{obj.eventName}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

public class BooleanMultiple : EditorWindow
{
    private int numberOfObjects = 4;
    public string path;
    private string entityName;
    private string[] so_name;
    void OnEnable()
    {
        so_name = new string[numberOfObjects];
    }
    [MenuItem("Tools/Create/Variable/Multiple Boolean")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BooleanMultiple));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Multiple Scriptable Objects", EditorStyles.boldLabel);
        entityName = EditorGUILayout.TextField("Entity Name", entityName);
        numberOfObjects = EditorGUILayout.IntField("Number of Objects", numberOfObjects);
        if (so_name == null || so_name.Length != numberOfObjects)
        {
            so_name = new string[numberOfObjects];
        }
        path = EditorGUILayout.TextField("Objects Path", path);
        EditorGUILayout.LabelField("Name of Objects");


        for (int i = 0; i < so_name.Length; i++)
        {
            so_name[i] = EditorGUILayout.TextField("Name " + i, so_name[i]);
        }


        if (GUILayout.Button("Create"))
        {
            CreateObjects();
        }
        if (GUILayout.Button("Refresh Path"))
        {
            RefreshPath();
        }
    }
    private void RefreshPath()
    {
        path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
    }

    private void CreateObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            BooleanVariable newObject = ScriptableObject.CreateInstance<BooleanVariable>();
            AssetDatabase.CreateAsset(newObject, $"{path}/{entityName} {so_name[i]} Boolean.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}

public class FloatMultiple : EditorWindow
{

    private int numberOfObjects = 4;
    private string entityName;
    public string path;
    private string[] so_name;
    void OnEnable()
    {
        so_name = new string[numberOfObjects];
    }
    [MenuItem("Tools/Create/Variable/Multiple Float")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FloatMultiple));
    }



    private void OnGUI()
    {
        GUILayout.Label("Create Multiple Scriptable Objects", EditorStyles.boldLabel);
        entityName = EditorGUILayout.TextField("Entity Name", entityName);
        numberOfObjects = EditorGUILayout.IntField("Number of Objects", numberOfObjects);
        if (so_name == null || so_name.Length != numberOfObjects)
        {
            so_name = new string[numberOfObjects];
        }
        path = EditorGUILayout.TextField("Objects Path", path);
        EditorGUILayout.LabelField("Name of Objects");
        for (int i = 0; i < so_name.Length; i++)
        {
            so_name[i] = EditorGUILayout.TextField("Name " + i, so_name[i]);
        }


        if (GUILayout.Button("Create"))
        {
            CreateObjects();
        }
        if (GUILayout.Button("Refresh Path"))
        {
            RefreshPath();
        }
    }

    private void CreateObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            FloatVariable newObject = ScriptableObject.CreateInstance<FloatVariable>();
            AssetDatabase.CreateAsset(newObject, $"{path}/{entityName} {so_name[i]} Float.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
    private void RefreshPath()
    {
        path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
    }
}

public class BasicStatuses : EditorWindow
{

    private string entityName;
    private string path;

    [MenuItem("Assets/Create/Status Ailment/4 Basic Status")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BasicStatuses));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Multiple Scriptable Objects", EditorStyles.boldLabel);
        entityName = EditorGUILayout.TextField("Entity Name", entityName);

        path = EditorGUILayout.TextField("Objects Path", path);

        if (GUILayout.Button("Create"))
        {
            CreateObjects();
        }
        if (GUILayout.Button("Refresh Path"))
        {
            RefreshPath();
        }
    }

    private void RefreshPath()
    {
        path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
    }
    private void CreateObjects()
    {

            BaseStatusEffect rage = ScriptableObject.CreateInstance<SO_Rage>();
            BaseStatusEffect poison = ScriptableObject.CreateInstance<SO_Poison>();
            BaseStatusEffect stun = ScriptableObject.CreateInstance<SO_Stun>();
            BaseStatusEffect breakself = ScriptableObject.CreateInstance<SO_Break>();

            AssetDatabase.CreateAsset(rage, $"{path}/{entityName} Rage Self.asset");
            AssetDatabase.CreateAsset(poison, $"{path}/{entityName} Poison Self.asset");
            AssetDatabase.CreateAsset(stun, $"{path}/{entityName} Stun Self.asset");
            AssetDatabase.CreateAsset(breakself, $"{path}/{entityName} Break Self.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

    }
}
#endif
