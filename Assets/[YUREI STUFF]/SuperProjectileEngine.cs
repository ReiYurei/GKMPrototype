using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEngine : MonoBehaviour
{
    public List<ProjectileEngine> engines;
    public Transform origin;
    public Vector3 offset;
    public Enemy enemy;
    public bool consecutiveShoot;
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
    public void OnProjectileInitiate()
    {
        transform.position = origin.position + offset;
        StartCoroutine(EngineStart());
    }
    IEnumerator EngineStart()
    {
        if(consecutiveShoot)
        {
            for (int i = 0;i < engines.Count;i++)
            {
                StartCoroutine(engines[i].Shoot());
                if (i == engines.Count - 1)
                {
                    yield return new WaitUntil(() => engines[i].endOfJob);
                    enemy.status.NotifyAttacking(false);
                    yield break;
                }
            }
            yield break;
        }
        for (int i = 0; i < engines.Count; i++)
        {
            yield return StartCoroutine(engines[i].Shoot());
            if (i == engines.Count - 1)
            {
                yield return StartCoroutine(engines[i].Shoot());
                yield break;

            }
        }
        yield break;
    }

}
