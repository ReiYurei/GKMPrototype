using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DEBUGGING : MonoBehaviour
{
    public GameObject rageIcon;
    public GameObject poisonIcon;
    public GameObject stunIcon;
    public GameObject breakIcon;
    public Enemy_Status enemyStatus;
    public Enemy enemy;
    private void Start()
    {
        rageIcon.SetActive(false);
        poisonIcon.SetActive(false);
        stunIcon.SetActive(false);
        breakIcon.SetActive(false);

    }

    private void Update()
    {
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
