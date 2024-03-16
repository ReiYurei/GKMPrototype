using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    ParticleSystem ps;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Disable;
    }

}
