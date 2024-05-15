using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class SpellCardEngineController : MonoBehaviour
{
    [SerializeField] private List<SuperProjectileEngine> _superEngines;
    public void Start()
    {
        var child = GetComponentsInChildren<SuperProjectileEngine>();
        for (int i = 0; i < child.Length; i++)
        {
            _superEngines.Add(child[i]);
        }
    }
    public void OnProjectileInitiate()
    {
        foreach (var superEngine in _superEngines)
        {
            superEngine.OnProjectileInitiate();
        }
    }
    public void DeactivateAll()
    {
        foreach(var superEngine in _superEngines)
        {
            superEngine.DeactiveAllProjectileEngine();
        }
    }
    public void DeactivateAllTemp()
    {
        foreach (var superEngine in _superEngines)
        {
            superEngine.DeactiveTempProjectileEngine();
        }
    }
}
