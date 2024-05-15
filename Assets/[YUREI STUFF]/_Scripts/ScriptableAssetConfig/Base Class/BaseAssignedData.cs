using UnityEngine;

public abstract class BaseAssignedData<T> : ScriptableObject
{
    [field: SerializeField] public T Data { get; private set;}
    public void AssignData(T data)
    {
        Data = data;
    }
}
