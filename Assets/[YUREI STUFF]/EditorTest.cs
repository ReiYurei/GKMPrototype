using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


public class DialogeMaker : EditorWindow
{
    SerializedProperty m_eventName;
    SerializedProperty m_dialogues;
    SerializedObject serializedObject;
    public string eventName;
    public string path;


    SO_Story_Dialogue obj;

    private void OnEnable()
    {
        obj = ScriptableObject.CreateInstance<SO_Story_Dialogue>();
        serializedObject = new UnityEditor.SerializedObject(obj);
        m_dialogues = serializedObject.FindProperty("dialogue");
        m_eventName = serializedObject.FindProperty("eventName");


    }
    [MenuItem("Tools/Dialogue Maker")]
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
        SO_Story_Dialogue newObject = ScriptableObject.CreateInstance<SO_Story_Dialogue>();
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
    private string path;
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
    private string path;
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
