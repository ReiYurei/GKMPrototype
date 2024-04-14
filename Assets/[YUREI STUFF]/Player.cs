using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

#if UNITY_EDITOR
#endif
public class Player : MonoBehaviour
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
    private string inputName = "Hub";
    Vector2 max;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInfo.StoreStaticData(_rb);
        inputActions.FindActionMap(inputName).Enable();
        input = inputActions.FindActionMap(inputName);
        input.FindAction("Interact").performed += Interact;
        max.x = maxSpeed;
    }
    
    private void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Collider2D collission = Physics2D.OverlapBox(interactBox.transform.position, interactBox.bounds.size,0);
        if (collission == null) return;
        if (collission.gameObject.CompareTag("Interact Box"))
        {
            collission.TryGetComponent(out IInteractable interactable);
            if (interactable == null) return;
            interactable.OnInteract();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(interactBox.transform.position, interactBox.bounds.size);

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
}
