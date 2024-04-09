using TriInspector;
using UnityEngine;
public abstract class BaseItem : ScriptableObject
{
    [ShowInInspector] public string Name { get; private set; }
    [ShowInInspector] public string Description { get; private set; }
}

[CreateAssetMenu(fileName = "Quest Item", menuName ="Item/Quest Item")]
public class SO_QuestItem : BaseItem
{

}