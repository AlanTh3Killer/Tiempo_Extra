using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static event Action<Vector2> OnMove;
    public static event Action OnAttack;
    public static event Action<bool> OnDefend;
    public static event Action OnDash; 

    public bool inputEnabled = true;

    void Update()
    {
        if (!inputEnabled) return;

        HandleMovement();
        HandleAttack();
        HandleDefense();
        HandleDash();

    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) // Presionar Shift izquierdo para dash
        {
            OnDash?.Invoke();
        }
    }

    private void HandleDefense()
    {
        if (Input.GetMouseButtonDown(1)) // Click derecho para defender
        {
            OnDefend?.Invoke(true);
        }
        else if (Input.GetMouseButtonUp(1)) // Soltar click derecho para dejar de defender
        {
            OnDefend?.Invoke(false);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo para atacar
        {
            OnAttack?.Invoke();
        }
    }

    private void HandleMovement()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movementInput.magnitude > 0.1f)
        {
            OnMove?.Invoke(movementInput);
        }
    }
}
