using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "Story Event/Event Dialogue")]
public class SO_Story_Dialogue : ScriptableObject
{
    public string eventName;
    public List<Dialogue> dialogue = new List<Dialogue>();
    public SO_Story_Dialogue(string eventName, List<Dialogue> dialogue) 
    {
        this.eventName = eventName;
        this.dialogue = dialogue;
    }
}
