using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

#if UNITY_EDITOR
#endif
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField,Required]SO_PlayerInfo playerInfo;
    public Rigidbody2D _rb;
    Vector2 movement;
    public InputActionAsset inputActions;
    InputActionMap input;
    public float moveSpeed;
    public float maxSpeed;
    public bool interactable;
    public BoxCollider2D interactBox;
    public SO_VoidGameEvent InventoryOpenEvent;
    private string inputName = "Hub";
    Vector2 max;
    private void Awake()
    {
        input = inputActions.FindActionMap(inputName);
    }
    private void OnEnable()
    {
        input.FindAction("Interact").performed += Interact;
        input.FindAction("Inventory").performed += Inventory;


    }
    private void OnDisable()
    {
        input.FindAction("Interact").performed -= Interact;
        input.FindAction("Inventory").performed -= Inventory;


    }
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInfo.StoreStaticData(_rb);

        max.x = maxSpeed;
    }
    
    private void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed)return;
        Debug.Log("<color=yellow>Interacting</color>");
        Collider2D[] collission = Physics2D.OverlapBoxAll(interactBox.transform.position, interactBox.bounds.size,0);
        if (collission == null) return;
        for(int i = 0; i < collission.Length; i++)
        {
            if (collission[i].TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract();
            }

        }
    }
    private void Inventory(InputAction.CallbackContext context)
    {
        InventoryOpenEvent.Raise();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactBox.transform.position, interactBox.bounds.size);
        Gizmos.color = Color.yellow;

    }
    void Update()
    {
        playerInfo.StoreDynamicData(transform.position);
        movement.x = input.FindAction("Movement").ReadValue<Vector2>().x;
        if (movement.x == 0)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        _rb.velocity += moveSpeed * movement * Time.deltaTime;
        if (Mathf.Abs(_rb.velocity.x) >= maxSpeed)
        {
            if (movement.x < 0)
            {
                _rb.velocity = max * movement.x;
            }
            else
            {
                _rb.velocity = max;
            }

        }
   
    }
    [Button(ButtonSizes.Large)]
    private void SetupComponent()
    {


        _rb ??= TryGetComponent<Rigidbody2D>(out Rigidbody2D rbComponent) ?
        _rb = rbComponent : _rb = gameObject.AddComponent<Rigidbody2D>();

        TryGetComponent<Animator>(out Animator animatorComponent);
        if (animatorComponent == null) { gameObject.AddComponent<Animator>(); }

    }

    public void OnDamage(float damage)
    {
        Debug.Log("Damaged!");
    }
}
