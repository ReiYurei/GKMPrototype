using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScanner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    EnemyStatus status;

    private void OnEnable()
    {
        if (status == null)
        {
            TryGetComponent<Enemy>(out Enemy component);
            if (component == null)
            {
                return;
            }
            status = component.StatusData;

        }
    }

}
