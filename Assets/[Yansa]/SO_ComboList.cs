using System.Collections.Generic;
using UnityEngine;
namespace YansaFork
{
    [CreateAssetMenu(fileName = "Combo List", menuName = "[Yansa]/Combo/Combo List")]
    public class SO_ComboList : ScriptableObject
    {
        [SerializeReference] public List<SO_Combo> UnlockedCombo;
    }
}

