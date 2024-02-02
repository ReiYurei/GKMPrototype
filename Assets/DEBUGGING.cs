using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGGING : MonoBehaviour
{
    public SO_EnemyAnimationHandler handler;
    private void OnEnable()
    {
        DoShit(handler);
    }
    private void OnDisable()
    {
        ShitDo(handler);
    }
    public void DoShit(SO_EnemyAnimationHandler handler)
    {
        handler.Kontol();
    }
    public void ShitDo(SO_EnemyAnimationHandler handler)
    {
        handler.Memek();
    }

}
