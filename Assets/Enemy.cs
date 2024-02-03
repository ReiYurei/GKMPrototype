using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class Enemy : MonoBehaviour
{
    [InlineEditor]
    public BaseStatus _status;
    public EnemyAnimator _enemyAnimator;
    public EnemyBehaviour _enemyBehaviour;

    public void Start()
    {
        _status.OnSpawn();
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

}
