using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "Miscellaneous/Dialogue")]
public class SO_Dialogue : ScriptableObject
{
    public string eventName;
    public List<Dialogue> dialogue = new List<Dialogue>();
    public EndEventBehaviour endEventBehaviour;
    [Tooltip("Leave Null for default behaviour")]
    [ShowIf(nameof(endEventBehaviour), EndEventBehaviour.CustomEvent)]public SO_VoidGameEvent CustomEndEvent;
}
public enum EndEventBehaviour
{
    DefaultEvent, None_ToHub, None_ToExterminate, CustomEvent
}