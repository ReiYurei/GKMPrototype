using TriInspector;
using UnityEngine;
public abstract class BaseItem : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: TextArea(3,5)][field: SerializeField] public string Description { get; private set; }
}
