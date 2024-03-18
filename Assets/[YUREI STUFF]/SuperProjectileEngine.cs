using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEngine : MonoBehaviour
{
    public List<ProjectileEngine> engines;
    public Enemy enemy;
    private void OnEnable()
    {
        var child = GetComponentsInChildren<ProjectileEngine>();
        for (int i = 0; i < child.Length; i++)
        {
            engines.Add(child[i]);
        }
        enemy.status.InitiateProjectile += OnProjectileInitiate;
    }
    private void OnDisable()
    {
        enemy.status.InitiateProjectile -= OnProjectileInitiate;

    }
    void OnProjectileInitiate()
    {

    }


}
