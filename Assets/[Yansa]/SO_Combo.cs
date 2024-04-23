using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System.Linq;
using System;
namespace YansaFork
{
    [Serializable]
    [CreateAssetMenu(order = 0, fileName = "ComboSO", menuName = "[Yansa]/Combo")]
    public class SO_Combo : ScriptableObject
    {
        [field: SerializeField]
        [PropertySpace(40)]
        public List<SpellInput> Command { get; private set; }
        [field: SerializeField]
        public SO_Spell_Data Spell { get; private set; }

        public bool IsMatch(IEnumerable<int> sequence)
        {
            var sequenceArray = sequence.Take(Command.Count).ToArray();

            if (sequenceArray.Length < Command.Count)
            {
                return false;
            }

            for (int i = 0; i < Command.Count; i++)
            {
                if (sequenceArray[i] != (int)Command[i])
                {
                    return false;
                }
            }
            return true;
        }

        public SO_Spell_Data GetResultingComboCommand()
        {
            return Spell;
        }
    }
}

