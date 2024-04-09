using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "Story Event/Event Dialogue")]
public class SO_StoryDialogue : ScriptableObject
{
    public string eventName;
    public List<Dialogue> dialogue = new List<Dialogue>();
    public bool triggerEventAtEnd = true;
}
