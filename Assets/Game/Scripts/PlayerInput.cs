using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static event Action<Vector2> OnMove;
    public static event Action OnAttack;

    void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movementInput.magnitude > 0.1f)
        {
            OnMove?.Invoke(movementInput);
        }

        if (Input.GetMouseButtonDown(0)) // Click izquierdo para atacar
        {
            OnAttack?.Invoke();
        }
    }
}
