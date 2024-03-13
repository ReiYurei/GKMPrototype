using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DEBUGGING : MonoBehaviour
{
    public TextMeshProUGUI textObj;


    public GameObject rageIcon;
    public GameObject poisonIcon;
    public GameObject stunIcon;
    public GameObject breakIcon;
    public Enemy_Status enemyStatus;
    public Enemy enemy;
    public bool limitFramerate;
    public SO_Melee_Attack_Dynamic attack;
    public int FrameRate;

    private void Awake()
    {
        if(limitFramerate)
        {
            Application.targetFrameRate = FrameRate;
        }
        else
        {
            Application.targetFrameRate = 0;
        }
    }

    private void Start()
    {
        rageIcon.SetActive(false);
        poisonIcon.SetActive(false);
        stunIcon.SetActive(false);
        breakIcon.SetActive(false);

    }
    int frameCount;
    float polling = 1f;
    float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        frameCount++;
        if (timer >= polling)
        {
            var frame =Mathf.RoundToInt(frameCount / timer);
            textObj.text = frame.ToString() + "FPS";
            timer -= polling;
            frameCount = 0;
        }

       
       OnStun();
       OnPoison();
       OnRage();
       OnBreak();
    }
    void OnStun()
    {
        if (enemy._statusEffectContainer.appliedStatuses.Any(status => status is SO_Stun))
        {
            stunIcon.SetActive(true);

        }
        else { stunIcon.SetActive(false); }

    }
    void OnPoison()
    {
        if (enemy._statusEffectContainer.appliedStatuses.Any(status => status is SO_Poison))
        {
            poisonIcon.SetActive(true);

        }
        else { poisonIcon.SetActive(false); }
    }
    void OnRage()
    {
        if (enemy._statusEffectContainer.appliedStatuses.Any(status => status is SO_Rage))
        {
            rageIcon.SetActive(true);
        }
        else
        {
            rageIcon.SetActive(false);
        }

    }
    void OnBreak()
    {
        if (enemy._statusEffectContainer.appliedStatuses.Any(status => status is SO_Break))
        {
            breakIcon.SetActive(true);
        }
        else
        {
            breakIcon.SetActive(false);
        }

    }

    public void MoveSet(Enemy enemy)
    {
        StartCoroutine(attack.Execute(enemy));
    }
    public void GainPoison(Enemy_Status status)
    {
        status.AffectPoison(20f);
    }
    public void GainStun(Enemy_Status status)
    {
        status.AffectStun(20f);
    }
    public void GainRage(Enemy_Status status)
    {
        status.AffectRage(100f);
    }
 
    
}

