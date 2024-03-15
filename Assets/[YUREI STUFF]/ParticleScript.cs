using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    [SerializeField]ParticleSystem particleSystems;
    private void Start()
    {
        particleSystems = GetComponent<ParticleSystem>();
        var main = particleSystems.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }

}
