using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "Boolean Variable", menuName = "Variable/Boolean")]
public class BooleanVariable : ScriptableObject
{
    [SerializeField]public bool value;
}
