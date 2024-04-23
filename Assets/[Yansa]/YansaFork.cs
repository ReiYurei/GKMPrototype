using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System.Linq;
using System;
namespace YansaFork
{
    public enum SpellInput //From Yansa
    {
        ButtonX, // 0 for XButton
        ButtonY, // 1 for YButton
        ButtonB, // 2 for BButton
    }
    public abstract class SO_Spell_Data : ScriptableObject
    {
        [field: SerializeField]public Sprite Icon { get; private set; }
        public string SpellName;
        [TextArea] public string Description;
        public float Consumption;
        public float Cooldown;
        public abstract void Execute();
    }
}

