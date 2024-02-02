using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class Enemy : MonoBehaviour
{
    [InlineEditor]
    public BaseStatus _status;

    public void Start()
    {
        _status.OnSpawn();
    }
}
