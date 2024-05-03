using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassThroughDataCollectionComponent : MonoBehaviour
{
    public static PassThroughDataCollectionComponent Instance { get; private set; }
    [field: SerializeField]public List<ScriptableObject> PassThroughData {  get; private set; }
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}

