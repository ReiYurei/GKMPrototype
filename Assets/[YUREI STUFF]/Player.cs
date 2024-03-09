using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

#if UNITY_EDITOR
#endif
public class Player : MonoBehaviour
{
    [SerializeField,Required]SO_PlayerInfo playerInfo;
    public Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInfo.StoreStaticData(_rb);
    }

    void Update()
    {
        playerInfo.StoreDynamicData(transform.position);
    }
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {


        _rb ??= TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent) ?
        _rb = rbComponent : _rb = gameObject.AddComponent<Rigidbody2D>();

        TryGetComponent<Animator>(out Animator animatorComponent);
        if (animatorComponent == null) { gameObject.AddComponent<Animator>(); }

    }
}
