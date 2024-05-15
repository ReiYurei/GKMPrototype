using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DEBUGGING : MonoBehaviour
{
    public TextMeshProUGUI textObj;
    public void Destroy(GameObject obj)
    {
        Resources.UnloadUnusedAssets();
    }
    public GameObject rageIcon;
    public GameObject poisonIcon;
    public GameObject stunIcon;
    public GameObject breakIcon;
    public SO_EnemyStatus enemyStatus;
    public Enemy enemy;
    public bool limitFramerate;
    public bool vsync;
    public SO_VoidGameEvent projectileEvent;
    public int FrameRate;

    private void Awake()
    {
        if (vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        if(limitFramerate)
        {
            Application.targetFrameRate = FrameRate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }

    public void Shoot()
    {
        projectileEvent.Raise();
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

       
      // OnStun();
      // OnPoison();
      // OnRage();
      // OnBreak();
    }
    void OnStun()
    {
        if (enemy.StatusEffectContainerComponent.appliedStatuses.Any(status => status is SO_Stun))
        {
            stunIcon.SetActive(true);

        }
        else { stunIcon.SetActive(false); }

    }
    void OnPoison()
    {
        if (enemy.StatusEffectContainerComponent.appliedStatuses.Any(status => status is SO_Poison))
        {
            poisonIcon.SetActive(true);

        }
        else { poisonIcon.SetActive(false); }
    }
    void OnRage()
    {
        if (enemy.StatusEffectContainerComponent.appliedStatuses.Any(status => status is SO_Rage))
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
        if (enemy.StatusEffectContainerComponent.appliedStatuses.Any(status => status is SO_Break))
        {
            breakIcon.SetActive(true);
        }
        else
        {
            breakIcon.SetActive(false);
        }

    }

    public void GainPoison(SO_EnemyStatus status)
    {
        status.AffectPoison(20f);
    }
    public void GainStun(SO_EnemyStatus status)
    {
        status.AffectStun(20f);
    }
    public void GainRage(SO_EnemyStatus status)
    {
        status.AffectRage(100f);
    }

}

