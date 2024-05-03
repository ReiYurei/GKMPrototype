using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public abstract class BaseCompletionObserver<T> : ScriptableObject
{
    public HashSet<T> Completion { get; private set; } = new HashSet<T>();
    [SerializeField] private List<T> _viewData;
    public HashSet<T> TempCompletion { get; private set; } = new HashSet<T>();

    public void AddToCompletion(T item)
    {
        Debug.Log("Added : " + item);
        Completion.Add(item);
        _viewData.Add(item);
    }
    public void AddToTemp(T item)
    {
        Debug.Log("Added" + item + " to Temp");
        TempCompletion.Add(item);
    }
    [Button("Clear Temp")]
    public void ClearTemp()
    {
        TempCompletion.Clear();
    }
    [Button("Clear Data")]
    public void ClearData()
    {
        Debug.Log("Removed : Completion Progression");
        Completion.Clear();
        _viewData.Clear();
        TempCompletion.Clear();
    }
}
